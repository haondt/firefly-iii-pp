import json
import requests
from urllib.parse import urljoin
from requests.models import PreparedRequest
import utils
from utils import memoize
from utils import trampoline_pipe as pipe

# Generate api key at https://web.postman.co/settings/me/api-keys
# create a workspace for firefly-iii-pp
# get workspace id with Postman API > Workspaces > Get all workspaces

# utils


# config
@memoize
def get_config():
    return load_collection_from_file("settings.json")

def get_workspace_id():
    return get_config()['workspace_id']

def get_api_key():
    return get_config()['api_key']

def get_postman_base_url():
    return "https://api.getpostman.com"

def get_collection_file_directory():
    return '../Postman collections'

def list_collection_file_names():
    return pipe(list_collection_names, utils.p_map(get_collection_file_path), list)()

def list_collection_names():
    return [
        'Firefly-III',
        'Firefly-III-pp',
        'Firefly-III-pp-tests'
    ]


# pure
def get_collection_file_path(collection_name):
    return f"{get_collection_file_directory()}/{collection_name}.postman_collection.json"

def prepare_url(url, params):
    r = PreparedRequest()
    r.prepare_url(url, params)
    return r.url


def remove_json_object_by_key(obj, path):
    if len(path) == 1:
        return pipe(filter, dict)(lambda kvp: kvp[0] != path[0], obj.items())
    if len(path) == 0:
        return obj
    return pipe(map, dict)(lambda kvp: (kvp[0], remove_json_object_by_key(kvp[1], path[1:]) if kvp[0] == path[0] else kvp[1]), obj.items())

def remove_collection_id(collection):
    return collection['info']['_postman_id'], remove_json_object_by_key(collection, ['info','_postman_id'])

def prepare_collection_for_upload(collection):
    return { "collection": collection }

def format_reponse_success(response: requests.Response, task_title):
    if response.status_code == 200:
        return f"Success: {task_title}"
    return f"Failed: {task_title}\nStatus: {response.status_code}\nResponse: {json.loads(response.content)}"

def sort_collection(collection):
    return pipe(
        utils.p_echo(collection),
        utils.p_tee(
            utils.p_if(lambda x: 'info' in x,
                pipe(lambda x: f"Sorting collection {x['info']['name']}", log))),
        utils.p_if(
            lambda x: 'item' in x,
            pipe(utils.p_fork(
                pipe(
                    utils.f_pipe(
                        utils.p_trace,
                            utils.p_filter,
                                lambda y: y != 'item'),
                    lambda x:
                        map(lambda k:
                            (k, x[0][k]), x[1]),
                    dict),
                lambda x: {
                    'item' : pipe(
                        utils.p_echo(x['item']),
                        utils.p_map(sort_collection),
                        utils.p_sort(lambda z: z['name']))()
            }), lambda t: t[0] | t[1]),
            utils.p_id))()

# queries
@memoize
def list_collections_from_cloud():
    return pipe(get_request, lambda x: x.content, json.loads, lambda x: x['collections'])("/collections", { "workspace": get_workspace_id() })

def get_request(path, params=None):
    return send_request("get", path, params=params)

def load_collection_from_file(fname):
    with open(fname) as f:
        return json.loads(f.read())

# commands
def sync_collections_from_file_to_cloud(file_names=None):
    if file_names is None:
        pipe(list_collection_file_names, sync_collections_from_file_to_cloud)()
    elif len(file_names) > 0:
        sync_collection_from_file_to_cloud(file_names[0])
        sync_collections_from_file_to_cloud(file_names[1:])

def sync_collections_from_cloud_to_file(collection_names=None):
    if collection_names is None:
        pipe(list_collections_from_cloud.cache.clear, utils.p_discard(list_collection_names), sync_collections_from_cloud_to_file)()
    elif len(collection_names) > 0:
        pipe(list_collections_from_cloud,
            utils.p_first(lambda x: x['name'] == collection_names[0]),
            lambda x: sync_collection_from_cloud_to_file(get_collection_file_path(x['name']), x['id'], x['name']))()
        sync_collections_from_cloud_to_file(collection_names[1:])

def sort_collections_in_cloud():
    return pipe(
        sync_collections_from_cloud_to_file,
        utils.p_discard(list_collection_file_names),
        utils.p_iter(pipe(
            utils.p_tee(sort_collection_in_file),
            sync_collection_from_file_to_cloud)))()

def sort_collection_in_file(collection_file_name):
    return pipe(
        utils.p_echo(collection_file_name),
        utils.p_trace(pipe(
            load_collection_from_file,
            sort_collection)),
        utils.p_unpack(save_collection_to_file))()

def sync_collection_from_file_to_cloud(file_name):
        return pipe(load_collection_from_file,
            remove_collection_id,
            utils.p_unpack(lambda i, c: (c['info']['name'], create_or_update_collection_in_cloud(i,c))),
            utils.p_tee(pipe(lambda x: format_reponse_success(x[1], f"Upload collection {x[0]}"), log)),
            lambda x: json.loads(x[1].content)['collection'],
            lambda x: sync_collection_from_cloud_to_file(file_name, x['id'], x['name'])
            )(file_name)

def sync_collection_from_cloud_to_file(file_name, collection_id, collection_name):
    pipe(get_request,
        utils.p_tee(pipe(lambda x: format_reponse_success(x, f"Download collection {collection_name}"), log)),
        utils.p_if(lambda x: x.status_code == 200,
            lambda x: save_collection_to_file(file_name, json.loads(x.content)['collection']),
            lambda x: None)
    )(f"/collections/{collection_id}")


def create_or_update_collection_in_cloud(identifier, collection):
    if identifier in pipe(list_collections_from_cloud,
        utils.p_map(lambda x: x['id']),
        set)():
        return put_request(f"/collections/{identifier}", json=prepare_collection_for_upload(collection), params={ "workspace" : get_workspace_id() })
    return post_request(f"/collections", json=prepare_collection_for_upload(collection), params={ "workspace" : get_workspace_id() })


def post_request(path, data=None, json=None, params=None):
    return send_request("post", path, json=json, data=data, params=params)

def put_request(path, data=None, json=None, params=None):
    return send_request("put", path, json=json, data=data, params=params)

def send_request(request_type, path, data=None, json=None, params=None):
    return requests.request(
        request_type,
        prepare_url(urljoin(get_postman_base_url(), path), params or {}),
        headers={ "X-API-KEY": get_api_key() },
        data=data,
        json=json
    )

def save_collection_to_file(fname, collection):
    with open(fname, 'w') as f:
        f.write(json.dumps(collection, indent=4))

def log(msg, level="info"):
    print(msg)

# work
#sync_collections_from_file_to_cloud()
#sync_collections_from_cloud_to_file()
sort_collections_in_cloud()