import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { ClientInfoDto } from "../models/dtos/ClientInfo";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ThunderService {
    constructor(private client: HttpClient) {
    }

    getClientData(): Observable<ServiceResponseModel<ClientInfoDto>> {
        return this.client.get(`${environment.API_HOST}/thunder/clientinfo`, {
            responseType: 'json',
            observe: 'response'
        }).pipe(
            map(r => r ? {
                success: true,
                body: <ClientInfoDto>r.body
            } : {
                success: false,
                error: "Received empty response from backend"
            }),
            catchError(e => {
                return of({
                    success: false,
                    error: e.message
                });
            })
        );
    }
}