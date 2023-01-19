import { Injectable } from "@angular/core";
import { takeLast } from "rxjs";
import { TestScheduler } from "rxjs/testing";
import { CaseModel } from "../models/Case";
import { CheckModel } from "../models/Check";
import { FolderModel } from "../models/Folder";
import { FolderContentModel } from "../models/FolderContent";
import { TestModel } from "../models/Test";

@Injectable({
    providedIn: 'root'
})
export class TestBuilderService {
    private sampleData = [
        {
            "type": "folder",
            "name": "Destinations",
            "items": [
                {
                    "type": "test",
                    "name": "Dollarama",
                    "checks": [
                        {
                            "name": "Destination name",
                            "key": "destination_name",
                            "value": "AMA"
                        },
                        {
                            "name": "Bill name",
                            "key": "bill_name",
                            "value": "Vehicle Insurance and Registration"
                        }
                    ],
                    "cases": [
                        {
                            "body": {
                                "destination_name": "(no name)",
                                "source_name": "Scotia Momentum VISA Infinite",
                                "description": "AMA CENTRE #52           EDMONTON     AB "
                            }
                        }
                    ]
                }
            ]
        }
    ]

    public build(tests: any): FolderContentModel[] {
        return tests.map((t: any) => this.buildFolderContent(t));
    }

    public buildSample(): FolderContentModel[] {
        return this.sampleData.map(i => this.buildFolderContent(i));
    }

    private buildFolderContent(folderContent: any) : FolderContentModel {
        if (folderContent.type === "folder") {
            return new FolderModel({
                name: folderContent.name,
                items: folderContent.items.map(this.buildFolderContent),
                meta: folderContent.meta ?? {}
            })
        } else if (folderContent.type === "test") {
            return new TestModel({
                name: folderContent.name,
                checks: folderContent.checks.map((c: any) => new CheckModel({
                    name: c.name,
                    key: c.key,
                    value: c.value,
                    meta: c.meta ?? {}
                })),
                cases: folderContent.cases.map((c: any) => new CaseModel({
                    body: Object.entries<string>(c.body).reduce((o: {key: string, value: string}[], [key, value]) => (o.push({
                        key: key,
                        value: value
                    }), o), []),
                    meta: c.meta ?? {}
                })),
                meta: folderContent.meta ?? {}
            })
        } else {
            throw new Error(`Unrecognized folder content type: ${ folderContent.type }`);
        }
    }

    public unBuild(tests: FolderContentModel[]): Object {
        return tests.map(x => this.unBuildFolderContent(x));
    }

    public foo(i: any): any {
        return i;
    }

    private unBuildFolderContent(model: FolderContentModel): Object {
        if (model instanceof FolderModel) {
            return {
                name: model.name,
                items: model.items.map((i: FolderContentModel) => this.unBuildFolderContent(i)) ?? [],
                type: model.type,
                meta: model.meta
            };
        } else if (model instanceof TestModel) {
            return {
                name: model.name,
                checks: model.checks,
                cases: model.cases.map(c => { return {
                    body: c.body.reduce((d: {[k: string]: string}, kvp: {key: string, value: string}) => (d[kvp.key]=kvp.value, d), {}),
                    meta: c.meta
                }; }),
                type: model.type,
                meta: model.meta ?? {}
            }
        } else {
            throw new Error(`Unrecognized folder content type: ${ model.type }`);
        }
    }
}