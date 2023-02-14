import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { RunnerStateDto } from "../models/dtos/RunnerState";
import { QueryOptionDto } from "../models/dtos/QueryOption";
import { DryRunResponseDto } from "../models/dtos/DryRunResponse";
import { QueryStartJobRequestDto } from "../models/dtos/QueryStartJobRequest";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";

@Injectable({
    providedIn: 'root'
})
export class RunnerService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    getStatus(): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.get('/runner/status');
    }

    stopJob(): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.postWithoutPayload('/runner/stop');
    }

    dryRunJob(requestDto: QueryStartJobRequestDto): Observable<ServiceResponseModel<DryRunResponseDto>> {
        return this.client.post('/runner/query/dry-run', requestDto);
    }

    startQueryJob(requestDto: QueryStartJobRequestDto): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post('/runner/query/start', requestDto);
    }

    startJob(requestDto: {start: string, end: string}): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post('/runner/query/start', requestDto);
    }

    startSingleJob(requestDto: {id: string}): Observable<ServiceResponseModel<RunnerStateDto>> {
        return this.client.post('/runner/single', requestDto);
    }
}