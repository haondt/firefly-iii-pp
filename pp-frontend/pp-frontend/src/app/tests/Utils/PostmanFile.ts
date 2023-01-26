import { CheckModel } from "src/app/models/Check"
import { FolderContentModel } from "src/app/models/FolderContent"
import { TestModel } from "src/app/models/Test"
import { FolderModel } from "src/app/models/Folder"
import { CaseModel } from "src/app/models/Case"
import { Observable } from "rxjs"

interface PreProcessedFile {
    cases: {
        path: string[],
        body: {key: string, value:string}[]
    }[],
    tests: {[name: string]: {
        path: string[],
        checks: {
            name: string
            key: string,
            value:string
        }[]
    }}
}

interface postmanRoot {
    info: {
        name: string,
        schema: string,
        updatedAt: string
    },
    item: postmanNode[],
    variable: {
        key: string,
        value: string
    }[]
}

interface postmanNode {
    name: string,
    item?: postmanNode[],
    event?: {
        listen: string,
        script?: {
            type: string,
            exec: string[]
        }
    }[],
    request?: {
        method: string,
        body: {
            mode: string,
            raw: string,
            options: {
                raw: {
                    language: string
                }
            }
        },
        url: {
            raw: string,
            host: string[],
            path: string[]
        }
    }
}

export const ingestPostmanFile = function(file: File | null): Observable<FolderContentModel[]> {
    if (file === null) {
        throw new Error('null file provided');
    }

    let reader = new FileReader();
    reader.readAsBinaryString(file);

    return new Observable(s => {
        reader.onloadend = () => {
            if (typeof reader.result === 'string') {
                let pp = preprocessFile(JSON.parse(reader.result) as postmanRoot);
                let root: FolderContentModel[] = [];
                for (let key in  pp.tests) {
                    addTest(root, {
                        name: key,
                        path: pp.tests[key].path,
                        checks: pp.tests[key].checks
                    });
                }

                for (let _case of pp.cases) {
                    addCase(root, _case);
                }
                s.next(root);
            } else {
                throw Error("Failure during import of postman file");
            }

            s.complete();
        }
    })

}

const addCase = function(root: FolderContentModel[], _case: {
    path: string[],
    body: {key: string, value:string}[]
}) {
    let prev = root.find(f => f.name === _case.path[0]);
    for (let p of _case.path.slice(1)) {
        if (prev instanceof FolderModel) {
            prev = prev!.items!.find(f => f.name === p);
        } else {
            throw Error("Failure during import of postman file");
        }
    }

    if (prev instanceof TestModel) {
        prev.cases.push(new CaseModel({
            body: _case.body
        }));
    } else {
        throw Error("Failure during import of postman file");
    }
}

const addTest = function(root: FolderContentModel[], test: {
    name: string,
    path: string[],
    checks: {
        name: string
        key: string,
        value:string
    }[]
    }) {
    let testModel = new TestModel({
        name: test.name,
        checks: test.checks.map(c => new CheckModel(c)),
    });

    if (test.path.length === 0) {
        root.push(testModel);
    } else {
        let prevName = test.path[0];
        let prev = root.find(f => f.name === prevName && f instanceof FolderModel) as FolderModel | undefined;
        if (!prev) {
            prev = new FolderModel({
                name: prevName
            });
            root.push(prev);
        }

        for (let nextName of test.path.slice(1)) {
            let next = prev!.items.find(f => f.name === nextName && f instanceof FolderModel) as FolderModel | undefined;
            if (!next) {
                next = new FolderModel({
                    name: nextName
                });
                prev.items.push(next);
            }

            prev = next;
            prevName = nextName;
        }

        prev.items.push(testModel);
    }
}

const preprocessFile = function(j: postmanRoot): PreProcessedFile {
    let childPreprocesses = j.item.map((x) => _preprocessFile(x, []));
    return childPreprocesses.reduce((p, c) => {
        return {
            cases: p.cases.concat(c.cases),
            tests: Object.assign({}, p.tests, c.tests)
        };
    });
}

const _preprocessFile = function(j: postmanNode, prevPath: string[]): PreProcessedFile {
    let childPreprocesses: PreProcessedFile[] = [];
    if (j.item != null) {
        childPreprocesses = j.item.map((x) => _preprocessFile(x, prevPath.concat([j.name])));
    }

    let result: PreProcessedFile = {
        cases: [],
        tests: {}
    };

    if (j.request) {
        let rawBody = JSON.parse(j.request.body.raw);
        result.cases.push({
            path: prevPath,
            body: Object.entries<string>(rawBody).reduce((o: {key: string, value: string}[], [key, value]) => (o.push({
                key: key,
                value: value
            }), o), []),
        });
    } else if (j.event && j.event.find(e => e.listen === 'test')) {
        let exec = j.event.find(e => e.listen === 'test')?.script?.exec.reduce((s1, s2) => s1 + s2);
        if (exec) {
            const r = /pm\.test\(['"](?<name>[\w\s]+)['"][^{]*{[^}]+pm\.expect\(jsonData\.(?<key>[\w_]+)\).to.eql\(\s*(?<value>.*?)\);/g;
            const matches = exec.matchAll(r);

            result.tests[j.name] = {
                path: prevPath,
                checks: []
            };

            for(let match of matches) {
                if (match && match.groups
                    && 'name' in match.groups && match.groups['name']
                    && 'key' in match.groups && match.groups['key']
                    && 'value' in match.groups && match.groups['value']) {
                    result.tests[j.name].checks.push({
                        name: match.groups['name'],
                        key: match.groups['key'],
                        value: (0, eval)('(' + match.groups['value'] + ')')
                    });
                }
            }
        }
    }

    return [result].concat(childPreprocesses).reduce((p, c) => {
        return {
            cases: p.cases.concat(c.cases),
            tests: Object.assign({}, p.tests, c.tests)
        };
    });
}