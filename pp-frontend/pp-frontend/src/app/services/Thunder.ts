import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { ClientInfoDto } from "../models/dtos/ClientInfo";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";
import { CreateTestCaseRequestDto } from "../models/dtos/CreateTestCaseRequest";

@Injectable({
    providedIn: 'root'
})
export class ThunderService {
    constructor(private client: HttpClient) {
    }

    private performGetRequest<TResponse>(url: string): Observable<ServiceResponseModel<TResponse>> {
        return this.client.get<TResponse>(url, {
            responseType: 'json',
            observe: 'response'
        }).pipe(map(r => r ? {
            success: true,
            body: r.body!
        } : {
            success: false,
            error: "Received empty response from backend"
        }),
        catchError(e => {
            if (e.error && e.error.exception && e.error.statusCode) {
                return of({
                    success: false,
                    error: `${e.error.exception}: ${e.error.details ?? e.error.statusCode}`
                });
            } else {
                return of({
                    success: false,
                    error: e.message
                });
            }
        }));
    }

    private performPostRequest<TRequest, TResponse>(url: string, requestDto: TRequest): Observable<ServiceResponseModel<TResponse>> {
        return this.client.post<TResponse>(url, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(map(r => r ? {
            success: true,
            body: r.body!
        } : {
            success: false,
            error: "Received empty response from backend"
        }),
        catchError(e => {
            if (e.error && e.error.exception && e.error.statusCode) {
                return of({
                    success: false,
                    error: `${e.error.exception}: ${e.error.details ?? e.error.statusCode}`
                });
            } else {
                return of({
                    success: false,
                    error: e.message
                });
            }
        }));
    }

    getClientData(): Observable<ServiceResponseModel<ClientInfoDto>> {
        return this.performGetRequest(`${environment.API_HOST}/thunder/clientinfo`);
    }

    sort(): Observable<ServiceResponseModel<Object>> {
        return this.performPostRequest(`${environment.API_HOST}/thunder/sort`, null);
    }

    getFolderNames(): Observable<ServiceResponseModel<string[]>> {
        return this.performGetRequest(`${environment.API_HOST}/thunder/foldernames`);
    }

    createCase(dto: CreateTestCaseRequestDto): Observable<ServiceResponseModel<any>> {
        return this.performPostRequest(`${environment.API_HOST}/thunder/testcase`, dto);
    }
}