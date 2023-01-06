import json

def load_json_from_file(fname):
    with open(fname) as f:
        return json.loads(f.read())

def save_json_to_file(fname, collection):
    with open(fname, 'w') as f:
        f.write(json.dumps(collection, indent=4))
