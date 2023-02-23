export interface QueryOptionDto {
    operand: string,
    viewValue: string,
    operators: {
        operator: string,
        viewValue: string,
        type: string,
        options?: {
            result: string,
            viewValue: string
        }[]
    }[]
}