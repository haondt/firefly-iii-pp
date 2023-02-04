## Deployment

### Build locally and deploy
- Add [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client) VS Code extension
  - [Enable Git Sync -> Save To Workspace](https://github.com/rangav/thunder-client-support#git-sync)
- Adjust `docker-compose.build.yml` if needed (e.g. `thunder-tests` is elsewhere).

Run
```shell
./docker-compose-build.sh
```

### Build for Development

#### Setup
- Add Thunder Client VS Code extension

#### Startup
- Ensure docker desktop running
- Start nodered
    - Run node red with docker compose `docker compose up -d -f docker-compose.dev.yml`
    - open up nodered (`http://localhost:1880/`) and import flows from `flows.json`.
- Start api
    - Open up `Firefly-iii-pp-Runner` in Visual studio
    - Start `Firefly-iii-pp-Runner.API` with `Docker` launch settings. It will create a docker container. Check in docker desktop to see which port it is assigned.
- Start UI
    - Check `pp-frontend/pp-frontend/src/environments/environment.ts` to make sure the api url matches that of the api, port included.
    - Run the app. Me personally I have better luck doing this in powershell than wsl
```shell
cd pp-frontend/pp-frontend
npm install
ng serve
```
    - Navigate to `http://localhost:4200/`
- Open thunder client
    - Open environment `firefly-pp-dev` and verify `base_url` is pointing at the correct port for the api

## Workflow
- Create rules in nodered
- Use UI to apply rules to transactions from Firefly-iii
- Use Thunder Client to run test cases against nodered
- Use UI to automatically generate test cases from transactions pulled from Firefly-iii


### Making changes
- changes in the node red repo (rules) can be synced by exporting the flows and saving to `flows.json` in the git repo. Recommended to minify when exporting.