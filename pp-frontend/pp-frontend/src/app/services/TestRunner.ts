import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { FolderContentModel } from "../models/FolderContent";

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

        if (test.type === 'test') {
            jobs.push({
                type: 'test',
                test: new Observable((subscriber) => {
                    setTimeout(() => {
                        let result = true;
                        if (result) {
                            test.meta['passed'] = true;
                        } else {
                            test.meta['failed'] = true;
                        }
                        subscriber.next(result);
                        subscriber.complete();
                    }, 1000);
                })
            });
        } else if (test.type === 'folder') {
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