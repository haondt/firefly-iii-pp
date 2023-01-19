import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { forkJoin, Observable, map } from "rxjs";
import { environment } from "src/environments/environment";
import { CheckModel } from "../models/Check";
import { FolderModel } from "../models/Folder";
import { FolderContentModel } from "../models/FolderContent";
import { TestModel } from "../models/Test";

interface Job {
    type: string|null,
    test?: Observable<boolean>,
    task?: Function
};

interface TestResult {
    passed: number,
    failed: number
}

@Injectable({
    providedIn: 'root'
})
export class TestRunnerService {
    constructor(private client: HttpClient) {
    }

    public prepareTests(tests: FolderContentModel[]): Job[] {
        return tests.map((x) => this.prepareTest(x)).flat();
    }

    private prepareTest(test: FolderContentModel): Job[] {
        test.meta['running'] = false;
        test.meta['passed'] = false;
        test.meta['failed'] = false;
        test.meta['waiting'] = true;

        let jobs: Job[]  = [{
            type: 'task',
            task: () => {
                test.meta['running'] = true;
                test.meta['waiting'] = false;
            }
        }];

        if (test instanceof TestModel) {
            jobs.push({
                type: 'test',
                test: new Observable((subscriber) => {
                    let cases = test.cases.slice();
                    let requests = test.cases.map(c =>
                        this.client.post<{[k: string]: Object}>(`${environment.NODERED_HOST}/apply`,
                        c.body.reduce((d: {[k: string]: string}, kvp: {key: string, value: string}) => (d[kvp.key]=kvp.value, d), {}),
                        {
                            responseType: 'json',
                            observe: 'response'
                        }).pipe(map(r => { return { response: (r ? r.body : null), setter: (k: string, v: Object | null) => c.meta[k] = v }})));
                    let allRequests = forkJoin(requests);
                    allRequests.subscribe(responses => {
                        let testPassed = true;
                        for (let r of responses) {
                            r.setter('response', r.response);
                            if (r.response !== null) {
                                for (let i=0;i<test.checks.length;i++) {
                                    let check: CheckModel = test.checks[i];
                                    if (!check.key || !check.value) {
                                        continue;
                                    }

                                    let checkPassed = true;
                                    let reason: string | null = null;
                                    if (!(check.key in r.response)) {
                                        checkPassed = false;
                                        reason = `Key "${check.key}" not found in response`;
                                    } else if (r.response[check.key] !== check.value) {
                                        checkPassed = false;
                                        reason = `Expected value "${check.value}" for key "${check.key}", but found "${r.response[check.key]}".`;
                                    }

                                    if (checkPassed) {
                                        r.setter('passed', true);
                                    } else {
                                        r.setter('failed', true);
                                        r.setter('reason', reason);
                                        testPassed = false;
                                    }
                                }

                            } else {
                                r.setter('failed', true);
                                r.setter('reason', 'No response');
                                testPassed = false;
                            }
                        }

                        if (testPassed) {
                            test.meta['passed'] = true;
                        } else {
                            test.meta['failed'] = true;
                        }

                        subscriber.next(testPassed);
                        subscriber.complete();
                    });
                })
            });
        } else if (test instanceof FolderModel) {
            if (test.items !== null && test.items !== undefined) {
                for (let item of test.items) {
                    jobs = jobs.concat(this.prepareTest(item));
                }
            }
        }

        jobs.push({
            type: 'task',
            task: () => { test.meta['running'] = false; }
        });

        return jobs;
    }

    public runTests(jobs: Job[]): Observable<TestResult> {
        return new Observable((subscriber) => {
            this._runTests(jobs, { passed: 0, failed: 0 }, r => {
                subscriber.next(r);
                subscriber.complete();
            })
        });
    }

    private _runTests(jobs: Job[], currentResult: TestResult, then: (r: TestResult) => any): void {
        let job = jobs.shift();
        if (job !== undefined) {
            if (job.type === 'task') {
                job.task?.();
                this._runTests(jobs, currentResult, then);
            } else if (job.type === 'test') {
                job.test?.subscribe(r => {
                    if (r) {
                        currentResult.passed++;
                    } else {
                        currentResult.failed++;
                    }
                    this._runTests(jobs, currentResult, then);
                });
            }
        } else {
            then(currentResult);
        }
    }
}