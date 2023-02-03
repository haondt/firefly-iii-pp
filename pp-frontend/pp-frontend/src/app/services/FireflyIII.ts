import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";

@Injectable({
    providedIn: 'root'
})
export class FireflyIIIService {
    constructor(private client: HttpClient) {
    }

    getTransactionData(id: string): Observable<ServiceResponseModel<Object>> {
        return this.client.get(`${environment.API_HOST}/firefly_iii/transactions/${id}`, {
            responseType: 'json',
            observe: 'response',
        }).pipe(
            map(r => r ? {
                success: true,
                body: <Object>r.body
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                if (e.status === 404){
                    return of({
                        success: false,
                        error: "Transaction not found"
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