from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)
from request_utils import get_request, put_request, post_request
import config
from func_utils import memoize
import json
from log_utils import p_debug, t_debug

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
