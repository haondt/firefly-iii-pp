# Firefly-iii-pp

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/haondt/firefly-iii-pp)](https://github.com/haondt/firefly-iii-pp/releases/latest)
[![Docker Hub release (latest by semver)](https://img.shields.io/docker/v/haumea/fireflyiii-pp?label=docker%20hub&sort=semver)](https://hub.docker.com/r/haumea/fireflyiii-pp)

A set of companion tools for post-processing transactions in Firefly-iii.

## Deployment

### Deploy docker hub images
- Run
    ```shell
    docker compose -f docker-compose.prod.yml up -d
    ```

### Build locally and deploy
- Add [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client) VS Code extension
  - [Enable Git Sync -> Save To Workspace](https://github.com/rangav/thunder-client-support#git-sync)
- Adjust `docker-compose.build.yml` if needed (e.g. `thunder-tests` is elsewhere).
- Run
    ```shell
    ./docker-compose-build.sh
    ```

### Build for Development

#### Setup
- Add Thunder Client VS Code extension

#### Startup
- Ensure docker desktop running
- Start nodered
    - Run node red with docker compose
        ```shell
        docker compose up -f docker-compose.dev.yml -d
        ```
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

#### Build and deploy to docker hub
- Run
    ```shell
    ./docker-builder.sh
    ```

## Workflow
- Create rules in nodered
- Use UI to apply rules to transactions from Firefly-iii
- Use Thunder Client to run test cases against nodered
- Use UI to automatically generate test cases from transactions pulled from Firefly-iii


### Making changes
- changes in the node red repo (rules) can be synced by exporting the flows and saving to `flows.json` in the git repo. Recommended to minify when exporting.
