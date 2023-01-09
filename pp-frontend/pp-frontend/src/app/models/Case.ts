export class CaseModel {
    body: object = {};

    public constructor(init?:Partial<CaseModel>) {
        Object.assign(this, init);
    }
}