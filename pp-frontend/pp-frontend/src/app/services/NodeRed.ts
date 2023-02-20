import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";
import { environment } from "src/environments/environment";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";

interface PassthroughPayload {
    stringifiedJsonPayload: string
}

interface ExtractKeyResponseDto {
    key: string
}

@Injectable({
    providedIn: 'root'
})
export class NodeRedService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    exportFlows(): Observable<ServiceResponseModel<null>> {
        return this.client.postWithoutPayload('/node-red/export-flows');
    }

    sendPassthrough(input: string): Observable<ServiceResponseModel<string>> {
        return this.client.post<PassthroughPayload, PassthroughPayload>('/node-red/passthrough', {stringifiedJsonPayload: input})
            .pipe(map(s => { return {
                success: s.success,
                body: s.success ? s.body!.stringifiedJsonPayload : undefined,
                error: s.error
            }}));
    }

    extractKey(field: string, input: string): Observable<ServiceResponseModel<string>> {
        return this.client.post<PassthroughPayload, ExtractKeyResponseDto>(`/node-red/extract-key/${field}`, { stringifiedJsonPayload: input})
            .pipe(map( s=> { return {
                success: s.success,
                body: s.success ? s.body!.key : undefined,
                error: s.error
            }}));
    }
}