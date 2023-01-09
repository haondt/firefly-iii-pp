import { Injectable } from "@angular/core";
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
    public build(tests: object[]): FolderContentModel[] {
        return tests.map(this.buildFolderContent);
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