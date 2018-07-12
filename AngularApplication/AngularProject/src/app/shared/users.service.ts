import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs/Observable";

@Injectable()
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsersByPrefix(prefix: string): Observable<any[]>
  {
    const apiUrl = 'http://localhost:5000/api/users/getUsersByNamePrefix';
    const params = new HttpParams({fromObject: {prefix: prefix}});
    return this.http.get<any[]>(apiUrl, {params: params});
  }
}
