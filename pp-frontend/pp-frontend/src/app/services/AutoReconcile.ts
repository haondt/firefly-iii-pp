import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";
import { AutoReconcileRequestDto } from "../models/dtos/AutoReconcileRequest";
import { AutoReconcileDryRunResponseDto } from "../models/dtos/AutoReconcileDryRunResponse";

@Injectable({
    providedIn: 'root'
})
export class AutoReconcileService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    dryRun(requestDto: AutoReconcileRequestDto): Observable<ServiceResponseModel<AutoReconcileDryRunResponseDto>> {
        return this.client.post('/auto-reconcile/dry-run/', requestDto);
    }

}