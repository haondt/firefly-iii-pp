import json, config, log_utils, request_utils, file_utils
from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)
import postman_api as pm
from log_utils import p_debug, f_debug, t_debug


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
            p_tee(pipe(lambda x: request_utils.format_response_success(x[0], success_log(x[1])), log_utils.log)),
            lambda x: continue_with(x[1])),
        pipe(lambda x: request_utils.format_response_success(x[0], fail_log(x[1])), log_utils.log)
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

# work
#sync_collections_from_file_to_cloud()
#sync_collections_from_cloud_to_file()
#sort_collections_in_cloud()