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
        return tests.map(this.buildFolderContent);
    }

    public buildSample(): FolderContentModel[] {
        return this.sampleData.map(i => this.buildFolderContent(i));
    }

    private buildFolderContent(folderContent: any) : FolderContentModel {
        if (folderContent.type === "folder") {
            return new FolderModel({
                "name": folderContent.name,
                "items": folderContent.items.map(this.buildFolderContent)
            })
        } else if (folderContent.type === "test") {
            return new TestModel({
                "name": folderContent.name,
                "checks": folderContent.checks.map((c: any) => new CheckModel({
                    "name": c.name,
                    "key": c.key,
                    "value": c.value,
                })),
                "cases": folderContent.cases.map((c: any) => new CaseModel({
                    body: c.body
                }))
            })
        } else {
            throw new Error(`Unrecognized folder content type: ${ folderContent.type }`);
        }
    }

}