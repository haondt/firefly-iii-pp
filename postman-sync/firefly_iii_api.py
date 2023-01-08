from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)
import request_utils, config
import json

def generate_request_header():
    return {
        "Authorization": "Bearer " + config.get_firefly_iii_api_key(),
        "Accept": "application/vnd.api+json"
    }

def get_request(path, params=None):
    return request_utils.get_request(config.get_firefly_iii_base_url(), path, params=params, headers=generate_request_header())


def download_transaction(continue_with):
    '''
    Args:
        continue_with: f(Response, transaction)
    Returns:
        f(transaction_id)
    '''
    return pipe(
        lambda t_id: f"/api/v1/transactions/{t_id}",
        get_request,
        p_if(
            lambda r: r.status_code == 200,
            pipe(p_trace(lambda r: json.loads(r.content)['data']), p_unpack(continue_with)),
            pipe(f_pipe(p_tee, p_echo, {}), p_unpack(continue_with))
        ))