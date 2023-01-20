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

export const ingestPostmanFile = function(file: File | null) {
    if (file === null) {
        throw new Error('null file provided');
    }

    let reader = new FileReader();
    reader.readAsBinaryString(file);

    reader.onloadend = () => {
        if (typeof reader.result === 'string') {
            let x = preprocessFile(JSON.parse(reader.result) as postmanRoot);
            console.log(x);
        }
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