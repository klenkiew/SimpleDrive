import {Injectable} from "@angular/core";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {User} from "../models/user";

@Injectable()
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsersByPrefix(prefix: string): Observable<User[]> {

    const apiUrl = environment.baseUsersApiUrl + 'api/users/getUsersByNamePrefix';
    const params = new HttpParams({fromObject: {prefix: prefix}});
    return this.http.get<User[]>(apiUrl, {params: params});
  }
}
