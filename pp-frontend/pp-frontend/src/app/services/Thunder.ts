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

    getClientData(): Observable<ServiceResponseModel<ClientInfoDto>> {
        return this.client.get(`${environment.API_HOST}/thunder/clientinfo`, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: <ClientInfoDto>r.body
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                return of({
                    success: false,
                    error: e.message
                });
            })
        );
    }

    sort(): Observable<ServiceResponseModel<Object>> {
        return this.client.post(`${environment.API_HOST}/thunder/sort`, null, {
            observe: 'response'
        }).pipe(
            map(r => { return { success: true }}),
            catchError(e => {
                return of({
                    success: false,
                    error: e.message
                });
            })
        );
    }

    getFolderNames(): Observable<ServiceResponseModel<string[]>> {
        return this.client.get(`${environment.API_HOST}/thunder/foldernames`, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: <string[]>r.body
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                return of({
                    success: false,
                    error: e.message
                });
            })
        );
    }

    createCase(dto: CreateTestCaseRequestDto): Observable<ServiceResponseModel<any>> {
        return this.client.post(`${environment.API_HOST}/thunder/testcase`, dto, {
            observe: 'response',
            responseType: 'json'
        }).pipe(
            map(r => { return { success: true, body: r.body }}),
            catchError(e => {
                console.log(e);
                return of({
                    success: false,
                    error: e.message
                });
            })
        );
    }
}