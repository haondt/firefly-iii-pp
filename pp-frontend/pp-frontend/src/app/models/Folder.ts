import { FolderContentModel } from './FolderContent';

export class FolderModel implements FolderContentModel {
    name = "New Folder";
    name_mutable = true;
    items: FolderContentModel[] = [];
    type: string = "folder";
    meta: { [key: string]: any; } = {};

    public constructor(init?: Partial<FolderModel>) {
        Object.assign(this, init);
    }
}