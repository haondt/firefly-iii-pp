import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";
import { environment } from "src/environments/environment";

interface PassthroughPayload {
    stringifiedJsonPayload: string
}

@Injectable({
    providedIn: 'root'
})
export class NodeRedService {
    constructor(private client: HttpClient) {
    }

    private performPostRequest<TRequest, TResponse>(url: string, requestDto: TRequest): Observable<ServiceResponseModel<TResponse>> {
        return this.client.post<TResponse>(url, requestDto, {
            responseType: 'json',
            observe: 'response'
        }).pipe(map(r => r ? {
            success: true,
            body: r.body!
        } : {
            success: false,
            error: "Received empty response from backend"
        }),
        catchError(e => {
            if (e.error && e.error.exception && e.error.statusCode) {
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
        }));
    }

    exportFlows(): Observable<ServiceResponseModel<null>> {
        return this.performPostRequest<null, null>(
            `${environment.API_HOST}/node-red/export-flows`,
            null
        );
    }

    sendPassthrough(input: string): Observable<ServiceResponseModel<string>> {
        return this.performPostRequest<PassthroughPayload, PassthroughPayload>(
            `${environment.API_HOST}/node-red/passthrough`,
            {stringifiedJsonPayload: input}
        ).pipe(map(s => { return {
            success: s.success,
            body: s.body?.stringifiedJsonPayload,
            error: s.error
        }}));
    }
}