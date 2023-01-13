import { FolderContentModel } from './FolderContent';
import { CheckModel } from './Check';
import { CaseModel } from './Case';

export class TestModel implements FolderContentModel {
    type: string = "test";
    name = "New Test";
    name_mutable = true;
    checks: CheckModel[] = [];
    cases: CaseModel[] = [];
    meta: { [key: string]: any; } = {};

    public constructor(init?:Partial<TestModel>) {
        Object.assign(this, init);
    }
}