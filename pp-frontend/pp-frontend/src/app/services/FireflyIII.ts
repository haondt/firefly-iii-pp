import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class FireflyIIIService {
    constructor(private client: HttpClient) {
    }

    getTransactionData(id: string): Observable<Object | null> {

        return this.client.get(`${environment.API_HOST}/firefly_iii/transactions/${id}`, {
            responseType: 'json',
            observe: 'response',
        }).pipe(
            catchError(e => {
                if (e.status === 404){
                    return of(null);
                } else {
                    return throwError(() => e);
                }
            }),
            map(r => r ? r.body : null)
        );
    }

    setTestData(id: string, testData: object) {
    }

}