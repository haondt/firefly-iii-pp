export interface RunnerStateDto {
    state: string,
    currentPage: number,
    totalPages: number,
    queuedTransactions: number,
    completedTransactions: number,
    totalTransactions: number
}