import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { QueryOptionDto } from "../models/dtos/QueryOption";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";

@Injectable({
    providedIn: 'root'
})
export class FireflyIIIService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    getTransactionData(id: string): Observable<ServiceResponseModel<Object>> {
        return this.client.get(`/firefly_iii/transactions/${id}`);
    }

}