from func_utils import memoize
import file_utils
from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_map
# config

def get_postman_base_url():
    return "https://api.getpostman.com"

def get_collection_file_directory():
    return '../Postman collections'


def list_collection_names():
    return [
        'Firefly-III',
        'Firefly-III-pp',
        'Firefly-III-pp-tests'
    ]

# helpers

@memoize
def get_config():
    return file_utils.load_json_from_file("settings.json")

def get_workspace_id():
    return get_config()['workspace_id']

def get_api_key():
    return get_config()['api_key']

def get_collection_file_path(collection_name):
    return f"{get_collection_file_directory()}/{collection_name}.postman_collection.json"

def list_collection_file_names():
    return pipe(list_collection_names, p_map(get_collection_file_path), list)()