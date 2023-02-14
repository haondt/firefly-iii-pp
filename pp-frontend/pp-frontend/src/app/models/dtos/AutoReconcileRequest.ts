import { QueryOperationModel } from "../QueryOperation";

export interface AutoReconcilePairingStrategyDto {
    requireMatchingDescriptions: boolean,
    requireMatchingDates: boolean,
    dateMatchToleranceInDays: number
};

export interface AutoReconcileJoiningStrategyDto {
    descriptionJoinStrategy: string|null,
    dateJoinStrategy: string|null,
    categoryJoinStrategy: string|null,
    notesJoinStrategy: string|null
};

export interface AutoReconcileRequestDto {
    sourceQueryOperations: QueryOperationModel[],
    destinationQueryOperations: QueryOperationModel[],
    pairingStrategy: AutoReconcilePairingStrategyDto,
    joiningStrategy: AutoReconcileJoiningStrategyDto
};