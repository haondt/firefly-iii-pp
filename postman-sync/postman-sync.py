import json
import requests
import pickle
import functools
from urllib.parse import urljoin
from requests.models import PreparedRequest
import utils

# Generate api key at https://web.postman.co/settings/me/api-keys
# create a workspace for firefly-iii-pp
# get workspace id with Postman API > Workspaces > Get all workspaces

# utils
def memoize(f):
    cache = f.cache = {}
    @functools.wraps(f)
    def wrapper(*args, **kwargs):
        k = pickle.dumps(args, 1) + pickle.dumps(kwargs, 1)
        if k not in cache:
            cache[k] = f(*args, **kwargs)
        return cache[k]
    return wrapper

def pipe(*funcs):
    def _inner(*args, **kwargs):
        def __inner(fs, *args, **kwargs):
            if len(fs) == 0:
                return args
            return __inner(fs[1:], fs[0](*args, **kwargs))
        return __inner(funcs, *args, **kwargs)[0]
    return _inner


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

def list_collection_file_names():
    return [
        '../Postman collections/Firefly-III-pp-tests.postman_collection.json',
        '../Postman collections/Firefly-III-pp.postman_collection.json',
        '../Postman collections/Firefly-III.postman_collection.json'
    ]


# pure
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
sync_collections_from_file_to_cloud()