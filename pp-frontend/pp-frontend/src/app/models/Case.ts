import { FolderContentModel } from "./FolderContent";
import { TreeNode } from "./TreeNode";

export class CaseModel implements TreeNode {
    name = "New Case";
    name_mutable = true;
    body: {[key: string]: string} = {};

    public constructor(init?:Partial<CaseModel>) {
        Object.assign(this, init);
    }
}