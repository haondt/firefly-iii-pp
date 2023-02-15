import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";
import { AutoReconcileRequestDto } from "../models/dtos/AutoReconcileRequest";
import { AutoReconcileDryRunResultResponseDto } from "../models/dtos/AutoReconcileDryRunResultResponse";
import { AutoReconcileStatusDto } from "../models/dtos/AutoReconcileStatus";

@Injectable({
    providedIn: 'root'
})
export class AutoReconcileService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    dryRun(requestDto: AutoReconcileRequestDto): Observable<ServiceResponseModel<AutoReconcileStatusDto>> {
        return this.client.post('/auto-reconcile/dry-run', requestDto);
    }

    run(requestDto: AutoReconcileRequestDto): Observable<ServiceResponseModel<AutoReconcileStatusDto>> {
        return this.client.post('/auto-reconcile/run', requestDto);
    }

    stop(): Observable<ServiceResponseModel<AutoReconcileStatusDto>> {
        return this.client.postWithoutPayload('/auto-reconcile/stop');
    }

    getDryRunResult(): Observable<ServiceResponseModel<AutoReconcileDryRunResultResponseDto>> {
        return this.client.get('/auto-reconcile/dry-run');
    }

    getStatus(): Observable<ServiceResponseModel<AutoReconcileStatusDto>> {
        return this.client.get('/auto-reconcile/status');
    }
}