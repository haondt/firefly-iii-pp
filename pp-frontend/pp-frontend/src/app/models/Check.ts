import { FolderContentModel } from "./FolderContent";
import { TreeNode } from "./TreeNode";

export class CheckModel implements TreeNode {
    name = "New Check";
    name_mutable = true;
    key: string|undefined;
    value: string|undefined;
    meta: { [key: string]: any; } = {};

    public constructor(init?:Partial<CheckModel>) {
        Object.assign(this, init);
    }
}