from athena.client import Athena, jsonify

def run(athena: Athena):
    base_url = athena.variable('base_url')
    token = athena.variable('token')
    client = athena.client(lambda b: b
                           .base_url(base_url)
                           .header('accept', 'application/vnd.api+json')
                           .header('Content-Type', 'application/json')
                           .auth.bearer(token))
    response = client.get("search/transactions?page=1")
    print(jsonify(athena.traces(), indent=4))
