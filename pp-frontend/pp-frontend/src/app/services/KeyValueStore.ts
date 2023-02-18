
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
export class KeyValueStoreService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) { }

    getStores(): Observable<ServiceResponseModel<string[]>> {
        return this.client.get('/lookup/stores');
    }

    autocomplete(store: string, partialValue: string): Observable<ServiceResponseModel<string[]>> {
        return this.client.post(`/lookup/action/${store}/autocomplete-value`, { partialValue: partialValue});
    }

    addKey(store: string, key: string, value: string): Observable<ServiceResponseModel<null>> {
        return this.client.post(`/lookup/action/${store}/add-key`, {
            key: key,
            value: value
        });
    }

    getKeys(store: string, value: string): Observable<ServiceResponseModel<string[]>> {
        return this.client.post(`/lookup/action/${store}/get-keys`, {
            value: value
        });
    }
}