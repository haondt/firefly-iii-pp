import json, config, log_utils, file_utils, operator
from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack, p_raise
)
import postman_api as pm
import firefly_iii_api as ff
from log_utils import p_debug, f_debug, t_debug
from models import *
from request_utils import format_response_success

from pipe_utils import create_e_pipe_generator, trampoline_pipe
e_pipe = create_e_pipe_generator(trampoline_pipe)


# json utils

def remove_json_object_by_key(obj, path):
    if len(path) == 1:
        return pipe(filter, dict)(lambda kvp: kvp[0] != path[0], obj.items())
    if len(path) == 0:
        return obj
    return pipe(map, dict)(lambda kvp: (kvp[0], remove_json_object_by_key(kvp[1], path[1:]) if kvp[0] == path[0] else kvp[1]), obj.items())

# collection utils

def remove_collection_id(collection):
    return collection['info']['_postman_id'], remove_json_object_by_key(collection, ['info','_postman_id'])


def sort_collection(collection):
    return pipe(
        p_echo(collection),
        p_tee(
            p_if(lambda x: 'info' in x,
                pipe(lambda x: f"Sorting collection {x['info']['name']}", log_utils.log))),
        p_if(
            lambda x: 'item' in x,
            pipe(p_fork(
                pipe(
                    f_pipe(
                        p_trace,
                            p_filter,
                                lambda y: y != 'item'),
                    lambda x:
                        map(lambda k:
                            (k, x[0][k]), x[1]),
                    dict),
                lambda x: {
                    'item' : pipe(
                        p_echo(x['item']),
                        p_map(sort_collection),
                        p_sort(lambda z: z['name']))()
            }), lambda t: t[0] | t[1]),
            p_id))()

def add_test_to_collection(collection):
    '''
    Args:
        collection: collection to add test to
    Returns:
        f(test) => collection with test
    '''
    return lambda test: pipe(
        p_filter(lambda k: k != 'item'),
        p_map(lambda k: (k, collection[k])),
        dict,
        lambda d: d | {
            'item': [
                pipe(p_filter(lambda k: k != 'item'), p_map(lambda k: (k, collection['item'][0][k])), dict)(collection['item'][0].keys()) | {
                    'item': [test] + collection['item'][0]['item']
                }
            ]
        }
    )(collection.keys())




##################### TODO
# - change the function log_response_and_continute_if_success to call continue_with(json) instead of continue_with(Response)
# - update sync_collections_from_x_to_y() with the appropriate usage of log_response_and_continue_if_success
# - implement sort_collections_in_cloud
# - finish up whatever else is in the string at the bottom of this file


# request utils
def log_response_and_continue_if_success(success_log, fail_log, continue_with):
    '''
    Args:
        sucess_log: f(json) => string
        fail_log: f(json) => string
        continue_with: f(json)
    Returns:
        f(Response)
    '''
    return pipe(p_trace(lambda x: json.loads(x.content)), p_if(
        lambda x: x[0].status_code == 200,
        pipe(
            p_tee(pipe(lambda x: format_response_success(x[0], success_log(x[1])), log_utils.log)),
            lambda x: continue_with(x[1])),
        pipe(lambda x: format_response_success(x[0], fail_log(x[1])), log_utils.log)
    ))

def log_response_and_assert_success(success_log, fail_log):
    '''
    Args:
        sucess_log: f(json) => string
        fail_log: f(json) => string
        continue_with: f(json)
    Returns:
        Response
    '''
    return pipe(p_trace(lambda x: json.loads(x.content)), p_if(
        lambda x: x[0].status_code == 200,
        pipe(
            p_tee(pipe(lambda x: format_response_success(x[0], success_log(x[1])), log_utils.log)),
            lambda x: x[1]),
        p_unpack(lambda r, j: pipe(
            format_response_success(r, fail_log(j)),
            log_utils.log,
            p_discard(lambda: p_raise(Exception(fail_log(j))))
        ))
    ))


# commands
def sync_collections_from_file_to_cloud():
    pipe(config.list_collection_file_names, p_iter(pipe(
        file_utils.load_json_from_file,
        remove_collection_id,
        p_unpack(pm.create_or_update_collection_in_cloud(
            lambda r, c_info: log_response_and_continue_if_success(
                lambda body: f"Uploaded collection {body['collection']['name']}",
                lambda _: f"Failed to upload collection",
                lambda _: pipe(p_echo(c_info['id']), f_pipe(pm.download_collection, p_unpack,
                    lambda r, c: log_response_and_continue_if_success(
                        lambda c: f"Downloaded collection{c['collection']['info']['name']}",
                        f"Failed to download collection with id {c_info['name']}",
                        p_noop
                    )(r)
                ))
            )(r)
        ))
    )))()

def sync_collections_from_cloud_to_file():
    return pipe(pm.list_collections_from_cloud.cache.clear,
        p_discard(config.list_collection_names),
        p_map(lambda n: p_first(lambda y: y['name'] == n)(pm.list_collections_from_cloud())),
        p_iter(pipe(lambda info: info['id'], f_pipe(
            pm.download_collection,
            lambda r, c: log_response_and_continue_if_success(
                lambda c: f"Downloaded collection{c['collection']['info']['name']}",
                lambda _: f"Failed to download collection from {r.url}",
                lambda j: file_utils.save_json_to_file(config.get_collection_file_path(j['collection']['info']['name']), c)
            )(r))
        ))
    )()

def sort_collections_in_cloud():
    return pipe(pm.list_collections_from_cloud.cache.clear,
        p_discard(config.list_collection_names),
        p_map(lambda n: p_first(lambda y: y['name'] == n)(pm.list_collections_from_cloud())),
        p_iter(pipe(lambda info: info['id'], f_pipe(
            pm.download_collection,
            lambda r, c: log_response_and_continue_if_success(
                lambda c: f"Downloaded collection {c['collection']['info']['name']}",
                lambda _: f"Failed to download collection from {r.url}",
                pipe(
                    lambda c: c['collection'],
                    sort_collection,
                    remove_collection_id,
                    p_tee(lambda x: log_utils.log(f"Uploading collection {x[1]['info']['name']}...")),
                    p_unpack(pm.create_or_update_collection_in_cloud(
                        lambda r, c_info: log_response_and_continue_if_success(
                            lambda body: f"Uploaded collection {body['collection']['name']}",
                            lambda _: f"Failed to upload collection",
                            p_noop
                        )(r)
                    ))
                )
            )(r)
        )))
    )()

def add_test_to_collection_in_cloud(collection_name):
    '''
    Args:
        collection_name: name of collection to add test to
    Returns:
        f(test_json_object)
    '''
    return lambda test: pipe(pm.list_collections_from_cloud.cache.clear,
        p_discard(pm.list_collections_from_cloud),
        p_first(lambda info: info['name'] == collection_name),
        lambda info: info['id'],
        f_pipe(
            pm.download_collection,
            lambda r, c: log_response_and_continue_if_success(
                lambda _: f"Downloaded collection {c['info']['name']}",
                lambda _: f"Failed to download collection from {r.url}",
                p_discard(pipe(
                    p_echo(test),
                    add_test_to_collection(c),
                    remove_collection_id,
                    p_tee(lambda x: log_utils.log(f"Uploading collection {x[1]['info']['name']}...")),
                    p_unpack(pm.create_or_update_collection_in_cloud(
                        lambda r, c_info: log_response_and_continue_if_success(
                            lambda body: f"Uploaded collection {body['collection']['name']}",
                            lambda _: f"Failed to upload collection",
                            p_noop
                        )(r)
                    ))
                ))
            )(r)
        )
    )()



def generate_test_requirement_json(requirement):
    return [
        f"pm.test('{requirement.name}', function() {{",
        f"    pm.expect(jsonData.{requirement.key}).to.eql(\"{requirement.expected}\");",
        "});",
    ]

def generate_test_json(test):
    return {
        "name": test.name,
        "item": [{
            "name": test.name + " 1",
            "event": [{
                "listen": "test",
            }],
            "request": {
                "method": "POST",
                "body": {
                    "mode": "raw",
                    "raw": json.dumps(test.request_body, indent=4),
                    "options": {
                        "raw": {
                            "language": "json"
                        }
                    }
                },
                "url": {
                    "raw": "{{base_url}}/apply",
                    "host": [
                        "{{base_url}}"
                    ],
                    "path": [
                        "apply"
                    ]
                }
            }
        }],
        "event": [
            { "listen": "prerequest"},
            {
                "listen": "test",
                "script": {
                    "type": "text/javascript",
                    "exec": [
                        "var jsonData = pm.response.json();",
                    ] + pipe(p_map(generate_test_requirement_json), list, p_reduce(operator.add, []))(test.requirements)
                }
            }
        ]
    }

def generate_test(name, requirements, request_body):
    return TestDto(name, requirements, request_body)

def generate_requirement(name, key, expected):
    return TestRequirementDto(name, key, expected)
def generate_destination_requirement(expected):
    return TestRequirementDto("Destination name", "destination_name", expected)
def generate_bill_requirement(expected):
    return TestRequirementDto("Bill name", "bill_name", expected)

# makes my life easier
def easy_generate_destination_requirement():
    return generate_destination_requirement
def easy_generate_bill_requirement(x):
    return lambda n: generate_bill_requirement(x)
def easy_generate_default_request_body():
    return {
        "destination_name": "(no name)",
        "source_name": "Scotia Momentum VISA Infinite",
        "description": ""
    }

def easy_generate_request_body_from_transaction(transaction_id, *kvp_generators):
    return ff.download_transaction(
        lambda r, t: pipe(
            log_response_and_assert_success(
                lambda _: f"Downloaded transaction {t['attributes']['transactions'][0]['description']}",
                lambda _: f"Failed to download transaction {transaction_id}",
            ),
            p_discard(lambda: e_pipe(
                kvp_generators,
                p_map(lambda g: g(t)),
                dict
            ))
        )(r)
    )(transaction_id)


def generate_kvp_from_transaction(key):
    return pipe(lambda d: d['attributes']['transactions'][0],
        lambda t: (key, t[key]))

def easy_generate_test(name, request_body, *requirement_generators):
    '''
    Args:
        name: string
        requirement_generators: list of f(string) -> TestRequirementDto
    Returns:
        test json object (deserialized)
    '''
    return pipe(
        p_echo(name),
        lambda d: generate_test(d, e_pipe(
            requirement_generators,
            p_map(lambda g: g(d)),
            list
        ), request_body),
        generate_test_json
    )()


# work
#sync_collections_from_file_to_cloud()
sync_collections_from_cloud_to_file()
#sort_collections_in_cloud()
#add_test_to_collection_in_cloud(config.get_test_collection_name())(
#    easy_generate_test("Trapped",
#        #easy_generate_default_request_body(),
#        easy_generate_request_body_from_transaction(2131,
#            generate_kvp_from_transaction("destination_name"),
#            generate_kvp_from_transaction("source_name"),
#            generate_kvp_from_transaction("description")
#        ),
#        easy_generate_destination_requirement(),
#    )
#)