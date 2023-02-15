export interface AutoReconcileDryRunTransferDto {
    source: string,
    destination: string,
    description: string,
    amount: number,
    date: string,
    category: string,
    notes: string,
    warning: string|null
};

export interface AutoReconcileDryRunResultResponseDto {
    transfers: AutoReconcileDryRunTransferDto[];
};