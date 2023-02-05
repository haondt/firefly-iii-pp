import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { RunnerStateDto } from "../models/dtos/RunnerState";

@Injectable({
    providedIn: 'root'
})
export class RunnerService {
    constructor(private client: HttpClient) {
    }

    getStatus(): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.get<RunnerStateDto>(`${environment.API_HOST}/runner/status`, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: <RunnerStateDto>r.body
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

    stopJob(): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post<RunnerStateDto>(`${environment.API_HOST}/runner/stop`, null, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: <RunnerStateDto>r.body
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
}