import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, iif, map, mergeMap, Observable, of, throwError } from "rxjs";
import { ServiceResponseModel } from "../models/ServiceResponse";

@Injectable({
    providedIn: 'root'
})
export class RunnerService {
    constructor(private client: HttpClient) {
    }
}