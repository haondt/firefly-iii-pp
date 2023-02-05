import { QueryOperationModel } from "../QueryOperation";

export interface QueryStartJobRequestDto {
    operations: QueryOperationModel[];
}