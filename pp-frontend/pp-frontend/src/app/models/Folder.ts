import { FolderContentModel } from './FolderContent';

export class FolderModel implements FolderContentModel {
    name_mutable = true;
    type: string = "folder";

    name = "New Folder";
    items: FolderContentModel[] = [];
    meta: { [key: string]: any; } = {};

    public constructor(init?: Partial<FolderModel>) {
        Object.assign(this, init);
    }
}