import { FolderContentModel } from './FolderContent';

export class FolderModel implements FolderContentModel {
    name: string = "";
    items: FolderContentModel[] = [];
    type: string = "folder";

    public constructor(init?: Partial<FolderModel>) {
        Object.assign(this, init);
    }
}