import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import "rxjs/add/operator/take";
import "rxjs/add/operator/do";
import {HttpClient} from "@angular/common/http";
import {ReplaySubject} from "rxjs/Rx";
import * as jwtDecode from "jwt-decode"
import {JwtToken} from "../models/jwt-token";


@Injectable()
export class AccountService {

  private static readonly usernameClaim: string = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
  private static readonly emailClaim: string = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';

  private decodedToken: JwtToken = null;
  private loggedIn: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(private http: HttpClient) {
    this.loggedIn.next(this.getEncodedToken() != null);
    if (this.getEncodedToken() != null)
      this.decodeCurrentToken();
  }

  public loggedInChange(): Observable<boolean>
  {
    return this.loggedIn.asObservable();
  }


  public isLoggedIn() {
    return this.decodedToken && this.decodedToken.expirationDate > new Date();
  }

  public register(data: RegistrationData): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/register';
    return this.http.post<string>(apiUrl, data);
  }

  public login(data: any): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/createToken';
    return this.downloadToken(apiUrl, data);
  }

  public refreshToken(): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/refreshToken';
    return this.downloadToken(apiUrl, null);
  }

  public changeEmail(data: any): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/changeEmail';
    let sub = this.http.post<string>(apiUrl, data);
    sub = sub.do(value => this.saveToken(value));
    return sub;
  }

  public changePassword(data: any): Observable<any>
  {
    const apiUrl = 'http://localhost:5000/api/authentication/changePassword';
    return this.http.post<string>(apiUrl, data);
  }

  public saveToken(tokenObj: any): void
  {
    localStorage['token'] = tokenObj.token;
    this.decodeCurrentToken();
    this.loggedIn.next(true);
  }

  public deleteToken(): void
  {
    localStorage.removeItem('token');
    this.decodedToken = null;
    this.loggedIn.next(false);
  }

  public getEncodedToken(): string
  {
    return localStorage['token'];
  }

  public getValidEncodedToken(): string
  {
    const token = this.getEncodedToken();
    if (!token)
      throw new Error("You don't have a valid token. Please try logging in.");
    return token;
  }

  public getToken(): JwtToken
  {
    return this.getJwtToken();
  }

  private downloadToken(apiUrl: string, requestBody: any) {
    let sub = this.http.post<string>(apiUrl, requestBody);
    sub = sub.do(value => this.saveToken(value));
    return sub;
  }

  private decodeCurrentToken() {
    this.decodedToken = this.toJwtToken(jwtDecode(this.getValidEncodedToken()));
  }

  private getJwtToken() {
    if (this.decodedToken)
      return this.decodedToken;

    this.decodeCurrentToken();
    return this.decodedToken;
  }

  private toJwtToken(serverToken: any) {
    const username = serverToken[AccountService.usernameClaim];
    const email = serverToken[AccountService.emailClaim];
    const expirationDate = new Date(serverToken['exp'] * 1000);

    return new JwtToken(username, email, expirationDate);
  }
}
