export class CheckModel {
    name: string|undefined;
    key: string|undefined;
    value: string|undefined;

    public constructor(init?:Partial<CheckModel>) {
        Object.assign(this, init);
    }
}