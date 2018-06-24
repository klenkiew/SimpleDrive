import { Injectable } from '@angular/core';
import {Observable} from "rxjs/Observable";
import {Subject} from "rxjs/Subject";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class AccountService {

  private loggedIn: Subject<boolean> = new Subject<boolean>();

  constructor(private http: HttpClient) { }

  public isLoggedIn(): Observable<boolean>
  {
    return this.loggedIn.asObservable();
  }

  public register(data: RegistrationData): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/register';
    return this.http.post<string>(apiUrl, data);
  }

  public saveToken(token: string): void
  {
    localStorage['token'] = JSON.stringify(token);
    this.loggedIn.next(true);
  }

  public deleteToken(): void
  {
    localStorage.removeItem('token');
    this.loggedIn.next(false);
  }

  public getToken(): string
  {
    return localStorage['token'];
  }
}
