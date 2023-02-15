import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { ClientInfoDto } from "../models/dtos/ClientInfo";
import { ServiceResponseModel } from "../models/ServiceResponse";
import { Observable, catchError, of, map } from "rxjs";
import { CreateTestCaseRequestDto } from "../models/dtos/CreateTestCaseRequest";
import { HttpClientWrapper } from "../utils/wrappers/HttpClient";

@Injectable({
    providedIn: 'root'
})
export class ThunderService {
    private client: HttpClientWrapper = new HttpClientWrapper(this.httpClient, environment.API_HOST);
    constructor(private httpClient: HttpClient) {
    }

    getClientData(): Observable<ServiceResponseModel<ClientInfoDto>> {
        return this.client.get(`/thunder/clientinfo`);
    }

    sort(): Observable<ServiceResponseModel<Object>> {
        return this.client.postWithoutPayload(`/thunder/sort`);
    }

    getFolderNames(): Observable<ServiceResponseModel<string[]>> {
        return this.client.get(`/thunder/foldernames`);
    }

    createCase(dto: CreateTestCaseRequestDto): Observable<ServiceResponseModel<any>> {
        return this.client.post(`/thunder/testcase`, dto);
    }
}