import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs/Observable";
import {environment} from "../../environments/environment";

@Injectable()
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsersByPrefix(prefix: string): Observable<any[]> {

    const apiUrl = environment.baseUsersApiUrl + 'api/users/getUsersByNamePrefix';
    const params = new HttpParams({fromObject: {prefix: prefix}});
    return this.http.get<any[]>(apiUrl, {params: params});
  }
}
