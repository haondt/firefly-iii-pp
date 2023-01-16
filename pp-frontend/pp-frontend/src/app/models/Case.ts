import { FolderContentModel } from "./FolderContent";
import { TreeNode } from "./TreeNode";

export class CaseModel {
    body: {key: string, value:string}[] = [];
    meta: { [key: string]: any; } = {};

    public constructor(init?:Partial<CaseModel>) {
        Object.assign(this, init);
    }
}