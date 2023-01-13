
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class MongoDbService {
    constructor(private client: HttpClient) {
    }

    getTestData(id: string): Observable<Object | null> {
        return this.client.get(`http://${environment.api_host}/mongo/tests/${id}`, {
            responseType: 'json',
            observe: 'response'
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