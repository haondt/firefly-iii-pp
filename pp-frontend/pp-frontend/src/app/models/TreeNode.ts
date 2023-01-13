export interface TreeNode {
    name: string;
    name_mutable: boolean;
    items?: TreeNode[];
    meta: {[key: string]: any};
}