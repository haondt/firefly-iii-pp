# Firefly-iii-pp

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/haondt/firefly-iii-pp)](https://github.com/haondt/firefly-iii-pp/releases/latest)
[![Docker Hub release (latest by semver)](https://img.shields.io/docker/v/haumea/fireflyiii-pp?label=docker%20hub&sort=semver)](https://hub.docker.com/r/haumea/fireflyiii-pp)

A set of companion tools for post-processing transactions in Firefly-iii.

## About

I love [Firefly-iii](https://github.com/firefly-iii/firefly-iii), but the only problem is it has a very weak rule system. You can apply changes to transactions by filtering through a set of rules, but the operators are limited to `is`, `is not`, `greater than` and `less than`. Furthermore, you can only join a list of conditions with an `and` or an `or`. For my purposes I needed at least nested conditions and regular expressions.

This project provides an interface to create rules in [Node-Red](https://nodered.org/), which not only satisfies my requirements, but allows even more complexity such as loops and external tools.

![](./assets/nr.png)

With such complex rules, it's easy to make a breaking change and permanently ruin your transactions. So, I've added a framework built around [Thunder Client](https://www.thunderclient.com/) for regression testing of rules. This includes a tool to automatically build a test case from an existing Firefly-iii transaction and send it to Thunder Client.

![](./assets/tndr.png)

Once the rules are created and regression tested, this project also includes a tool for running the rules on a set of transactions. The transactions to run on can be filtered by transaction ID, by a time period, or by a query formed by a set of conditions. The query filtering leverages Firefly-iii's [search](https://docs.firefly-iii.org/firefly-iii/pages-and-features/search/) feature, and can be dry run to test the query before starting the rule-running process.

![](./assets/ff3.png)

## Bonus Features

### Auto Reconciliation

Due to the way my banks export transactions to csv, sometimes I end up in situations where a transfer is imported as two seperate transactions on each account. Numerically it adds up, but I would like them to be reconciled into a single transaction.

![](./assets/rec1.png)

That is exactly what this feature can do. This tool will find pairs of transactions with the same amount, and reconcile them into a transfer. There are many configuration options to determine which transactions are eligible for reconciliation, how to pair them up, and how to merge them into a single transfer. There is also a dry run option that will show you what the new transfers will look like before actually running the job.

![](./assets/rec2.png)

The final result is a single transfer between accounts.

![](./assets/rec3.png)

### Key Value Lookup

As I built my flows in Node-Red, I found myself invariably winding up with a giant subflow that was essentially a giant key-value lookup. Node-Red starts to chug a bit with this many nodes, so I needed to extract this functionality to something more well suited.

![](./assets/kv1.png)

I think the best idea would be to sub in a database and have Node-Red make calls to it. This is doable by just adding one to the docker compose stack. However, I wanted something that could be backed up as a file to git, with a structure that is git-friendly. This feature is more of a key-value-valuevalue store, where many keys map to a single value, and that value maps to a single "valuevalue". The "valuevalue" contains attribute information. Despite my efforts to make it flexible, this feature is still pretty niche and not necessary to the rest of the suite. Just something that matches my workflow.

![](./assets/kv2.png)

## Deployment

### Deploy via Docker Hub
- Add [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client) VS Code extension
  - [Enable Git Sync -> Save To Workspace](https://github.com/rangav/thunder-client-support#git-sync)
- Create a docker compose file using the contents of `docker-compose.prod.yml`
- Ensure your directory has the following structure
    ```
    .
    ├── docker-compose.yml
    ├── flows.json
    ├── flows_cred.json
    └── thunder-tests
        ├── thunderActivity.json
        ├── thunderCollection.json
        ├── thunderEnvironment.json
        └── thunderclient.json
    ```
- Run
    ```shell
    docker compose up -d
    ```
- Navigate to Node-Red at `http://localhost:1880/`
- Navigate to Firefly-iii-pp at `http://localhost:9091/`

### Build locally and deploy
- Run
    ```shell
    docker compose -f docker-compose.build.yml up -d
    ```

### Build for Development

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
    - Optionally, you can run `FireflyIIIpp.Mock.API` instead. It will mock all the external services (except Thunder Client). Good for testing UI changes.
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
    ./docker-builder.sh [tag]
    ```

## Notes
- The api will send requests to Node-Red at the following endpoints, so they should be used as the entry points for your Node-Red flows.
  - `/apply`: Transactions will be sent here for rule evaluation
    - Can return `200` to indicate the transaction should be updated, or `304` to indicate there are no changes.
  - `/extract-key/{field}`: Transactions will be sent here for a key to be extracted from the given field. This can be the key used in the key-value lookup.
    - e.g. `/extract-key/description`
- Importing flows to Node-Red should be done with the Node-Red UI
- Exporting flows from Node-Red can be done through the pp frontend