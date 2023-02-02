export interface ServiceResponseModel<T> {
    body?: T;
    success: boolean;
    error?: string;
}