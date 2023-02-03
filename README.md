## Development workflow

### Setup
- Add VS Code extension [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client)
  - [Enable Git Sync -> Save To Workspace](https://github.com/rangav/thunder-client-support#git-sync)
- Generate postman api key at https://web.postman.co/settings/me/api-keys
- create a workspace for firefly-iii-pp
- get workspace id with Postman API > Workspaces > Get all workspaces
- `cp ./postman-sync/settings.template.json ./postman-sync/settings.json`
- populate `./postman-sync.settings.json` with the values from previous steps
- run `sync_collections_from_file_to_cloud` in `postman-sync/postman-sync.py` to upload the collections to postman
- configure firefly-iii connection settings in vs project `Firefly-iii-pp-Runner.API/appsettings.json`

### Startup
- Ensure docker desktop running
- Run node red with docker compose (`docker compose up -d`) and import flows from `flows.json`.
- Run the vs project (will deploy to docker)

### Workflow
- Use the postman collection `Firefly-III-pp` to interact with vs project.
    - vs project will automatically pull transactions from firefly-iii, run them through the nodered flow to set destination, bill, etc, and update the transactions in firefly-ii.

### Making changes
- changes in the node red repo (rules) can be synced by exporting the flows and saving to `flows.json` in the git repo.
- changes in the postman collection can be pulled into the git repo with `./postman-sync/postman-sync.py`.

### Postman-sync
- Postman sync uses the postman API to help manage the postman collections

| function | does |
|---|---|
| `sync_collections_from_file_to_cloud` | self explanatory |
| `sync_collections_from_cloud_to_file` | self explanatory |
| `sort_collections_in_cloud` | sorts the postman collection alphabetically. DOES NOT COPY CHANGES TO FILE. Changes will need to be copied with the above function before syncing to github |
| `add_test_to_collection_in_cloud` and `easy_generate_...` methods | Quickly and easily add a new test to the postman collection |


Below are some observations I've made while attempting a functional programming style in python.

### Functional limitations of python
- No tail call optimization (TCO)
    - each nested call will add a stack frame, and stack frames are limited
    - function pipes need to be executed iteratively instead of with continuation (see: `cont_pipe` vs `trampoline_pipe`)
    - (at a certain point) iteration needs to be done iteratively instead of recursively (see: `p_iter` vs `p_iter_loop`)
- Lists are not linked lists, so "tail" and operations (like Lisps `cdr`) are O(n) instead of O(1), and iterating through a list while slicing it to emulate this behaviour is O(n^2) instead of O(n) :(
    - This makes it difficult to keep states immutable, since changing a single item in a list without mutating the original requires copying the entire list, which means iterating through the whole thing
- Poor stack tracing, even with trampoline piping
    - stack trace only includes function that failed. Rest of trace that should be the outer function calls are just repeated calls from the pipe function (`cont_pipe`/`trampoline_pipe`)