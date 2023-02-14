import { HttpClient, HttpResponse } from "@angular/common/http";
import { ServiceResponseModel } from "src/app/models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";

export class HttpClientWrapper {
    constructor(private _client: HttpClient,
        private _baseUrl: string) {
    }

    private httpPipeline<TResponse>(observableGenerator: () => Observable<HttpResponse<TResponse>>): Observable<ServiceResponseModel<TResponse>> {
        return observableGenerator().pipe(map(r => r ? {
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

    public post<TRequest, TResponse>(path: string, requestDto: TRequest): Observable<ServiceResponseModel<TResponse>> {
        return this.httpPipeline(() => this._client.post<TResponse>(this._baseUrl + path, requestDto, { responseType: 'json', observe: 'response' }));
    }

    public postWithoutPayload<TResponse>(path: string): Observable<ServiceResponseModel<TResponse>> {
        return this.httpPipeline(() => this._client.post<TResponse>(this._baseUrl + path, null, { responseType: 'json', observe: 'response' }));
    }

    public get<TResponse>(path: string): Observable<ServiceResponseModel<TResponse>> {
        return this.httpPipeline(() => this._client.get<TResponse>(this._baseUrl + path, { responseType: 'json', observe: 'response' }));
    }
}