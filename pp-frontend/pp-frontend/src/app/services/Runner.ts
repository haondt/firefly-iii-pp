import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { RunnerStateDto } from "../models/dtos/RunnerState";
import { QueryOptionDto } from "../models/dtos/QueryOption";
import { DryRunResponseDto } from "../models/dtos/DryRunResponse";
import { QueryStartJobRequestDto } from "../models/dtos/QueryStartJobRequest";

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
                body: r.body!
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
                body: r.body!
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

    getQueryOptions(): Observable<ServiceResponseModel<QueryOptionDto[]>> {
        return this.client.get<QueryOptionDto[]>(`${environment.API_HOST}/runner/query-options`, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: r.body!
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

    dryRunJob(requestDto: QueryStartJobRequestDto): Observable<ServiceResponseModel<DryRunResponseDto>> {
        return this.client.post<DryRunResponseDto>(`${environment.API_HOST}/runner/query/dry-run`, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: r.body!
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                if (e.error
                    && e.error.exception
                    && e.error.statusCode) {
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
            })
        );
    }

    startQueryJob(requestDto: QueryStartJobRequestDto): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post<RunnerStateDto>(`${environment.API_HOST}/runner/query/start`, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: r.body!
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                if (e.error
                    && e.error.exception
                    && e.error.statusCode) {
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
            })
        );
    }

    startJob(requestDto: {start: string, end: string}): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post<RunnerStateDto>(`${environment.API_HOST}/runner/start`, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: r.body!
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                if (e.error
                    && e.error.exception
                    && e.error.statusCode) {
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
            })
        );
    }

    startSingleJob(requestDto: {id: string}): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post<RunnerStateDto>(`${environment.API_HOST}/runner/single`, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: r.body!
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                if (e.error
                    && e.error.exception
                    && e.error.statusCode) {
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
            })
        );
    }

}