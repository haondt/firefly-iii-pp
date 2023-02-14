interface viewOption {
    viewValue: string,
    option: string
};

export interface AutoReconcileJoiningStrategyOptionsModel {
    descriptionJoinStrategyOptions: viewOption[],
    dateJoinStrategyOptions: viewOption[],
    categoryJoinStrategyOptions: viewOption[],
    notesJoinStrategyOptions: viewOption[]
};

export interface AutoReconcileRequestOptionsModel {
    joiningStrategyOptions: AutoReconcileJoiningStrategyOptionsModel
};