from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)
import request_utils
import config
from func_utils import memoize
import json
from log_utils import p_debug, t_debug

# request stuff
def generate_request_header():
    return { "X-API-KEY": config.get_api_key() }

def get_request(path, params=None):
    return request_utils.get_request(config.get_postman_base_url(), path, params=params, headers=generate_request_header())

def post_request(path, data=None, json=None, params=None):
    return request_utils.post_request(config.get_postman_base_url(), path, json=json, data=data, params=params, headers=generate_request_header())

def put_request(path, data=None, json=None, params=None):
    return request_utils.put_request(config.get_postman_base_url(), path, json=json, data=data, params=params, headers=generate_request_header())

# utils

def prepare_collection_for_upload(collection):
    return { "collection": collection }

# work

def download_collection(continue_with):
    '''
    Args:
        continue_with: f(Response, collection)
    Returns:
        f(collection_id)
    '''
    return pipe(
        lambda collection_id: f"/collections/{collection_id}",
        get_request,
        p_if(
            lambda r: r.status_code == 200,
            pipe(p_trace(lambda r: json.loads(r.content)['collection']), p_unpack(continue_with)),
            pipe(f_pipe(p_tee, p_echo, {}), p_unpack(continue_with))
        ))

# TODO: change to "continue_with" format
@memoize
def list_collections_from_cloud():
    return pipe(
        p_echo("/collections", {"workspace": config.get_workspace_id()}),
        p_unpack(get_request),
        lambda x: x.content, json.loads, lambda x: x['collections'])()

def create_or_update_collection_in_cloud(continue_with):
    '''
    Args:
        continue_with: f(Response, collection_info)
    Returns:
        f(indentifier, collection)
    '''
    return pipe(p_trace(pipe(p_discard(list_collections_from_cloud), p_map(lambda c: c['id']), set)),
        p_if(
            lambda x: x[0][0] in x[1],
            lambda x: put_request(f"/collections/{x[0][0]}", json=prepare_collection_for_upload(x[0][1]), params={ "workspace" : config.get_workspace_id() }),
            lambda x: post_request("/collections", json=prepare_collection_for_upload(x[0][1]), params={ "workspace" : config.get_workspace_id() })
        ),
        p_if(
            lambda r: r.status_code == 200,
            pipe(p_trace(lambda r: json.loads(r.content)['collection']), p_unpack(continue_with)),
            pipe(p_trace(lambda r: json.loads(r.content)), p_unpack(continue_with)),
        ))

