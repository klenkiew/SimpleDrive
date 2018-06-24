import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';

import 'rxjs/add/operator/map';
import {HttpClient} from "@angular/common/http";

@Injectable()
export class AppService{
  constructor(private http: HttpClient) {

  }
  getGames(): Observable<string[]>
  {
    const apiUrl = 'http://localhost:5000/api/values';
    return this.http.get<string[]>(apiUrl);
  }
}
