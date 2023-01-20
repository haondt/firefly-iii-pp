import { FolderContentModel } from "./FolderContent";
import { TreeNode } from "./TreeNode";

export class CheckModel {
    name = "New Check";
    key: string|undefined;
    value: string|undefined = "";
    meta: { [key: string]: any; } = {};

    public constructor(init?:Partial<CheckModel>) {
        Object.assign(this, init);
    }
}