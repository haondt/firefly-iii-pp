export interface AutoReconcileStatusDto {
    state: string,
    totalTransfers: number,
    totalSourceTransactions: number,
    totalDestinationTransactions: number,
    completedTransfers: number
};