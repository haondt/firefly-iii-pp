import requests
from urllib.parse import urljoin
from requests.models import PreparedRequest
import requests
import config
import json
from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)
import typing
T = typing.TypeVar('T')

def get_request(base_url, path, params=None, headers={}):
    return send_request(base_url, "get", path, params=params, headers=headers)

def post_request(base_url, path, data=None, json=None, params=None, headers={}):
    return send_request(base_url, "post", path, json=json, data=data, params=params, headers=headers)

def put_request(base_url, path, data=None, json=None, params=None, headers={}):
    return send_request(base_url, "put", path, json=json, data=data, params=params, headers=headers)

def send_request(base_url, request_type, path, data=None, json=None, params=None, headers={}):
    return requests.request(
        request_type,
        prepare_url(urljoin(base_url, path), params or {}),
        headers=headers,
        data=data,
        json=json
    )

def prepare_url(url, params):
    r = PreparedRequest()
    r.prepare_url(url, params)
    return r.url

def format_response_success(response: requests.Response, task_title):
    if response.status_code == 200:
        return f"Success: {task_title}"
    return f"Failed: {task_title}\nStatus: {response.status_code}\nResponse: {json.loads(response.content)}"
