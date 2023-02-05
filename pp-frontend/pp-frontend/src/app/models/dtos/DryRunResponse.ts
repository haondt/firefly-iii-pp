import { QueryOperationModel } from "../QueryOperation";

export interface DryRunResponseDto {
    operations: QueryOperationModel[],
    query: string,
    totalTransactions: number,
    totalPages: number,
    sample: Object
}