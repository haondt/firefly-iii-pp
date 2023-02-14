export interface AutoReconcileDryRunTransferDto {
    source: string,
    destination: string,
    description: string,
    amount: number,
    date: string,
    category: string
};

export interface AutoReconcileDryRunResponseDto {
    transfers: AutoReconcileDryRunTransferDto[];
};