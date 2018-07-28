import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import "rxjs/add/operator/take";
import "rxjs/add/operator/do";
import {HttpClient} from "@angular/common/http";
import {ReplaySubject} from "rxjs/Rx";
import * as jwtDecode from "jwt-decode"
import {JwtToken} from "../models/jwt-token";
import {environment} from "../../../environments/environment";


@Injectable()
export class AccountService {

  private static readonly usernameClaim: string = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
  private static readonly emailClaim: string = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
  private static readonly idClaim: string = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

  private static readonly baseUrl: string = environment.baseUsersApiUrl;

  private decodedToken: JwtToken = null;
  private loggedIn: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(private http: HttpClient) {

    this.loggedIn.next(this.getEncodedToken() != null);
    if (this.getEncodedToken() != null)
      this.decodeCurrentToken();
  }

  loggedInChange(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  checkTokenValidity(): void {
    if (this.decodedToken && this.decodedToken.expirationDate < new Date()) {
      this.decodedToken = null;
      this.loggedIn.next(false);
    }
  }

  isLoggedIn(): boolean {
    return this.decodedToken && this.decodedToken.expirationDate > new Date();
  }

  register(data: RegisterRequest): Observable<any> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/register';
    return this.http.post(apiUrl, data);
  }

  login(data: LoginRequest): Observable<string> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/createToken';
    console.log('Url: %o', apiUrl);
    return this.downloadToken(apiUrl, data);
  }

  refreshToken(): Observable<Object> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/refreshToken';
    return this.downloadToken(apiUrl, null);
  }

  changeEmail(data: ChangeEmailRequest): Observable<Object> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/changeEmail';
    return this.http.post(apiUrl, data);
  }

  changePassword(data: ChangePasswordRequest): Observable<any> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/changePassword';
    return this.http.post(apiUrl, data);
  }

  saveToken(tokenObj: any): void {
    localStorage['token'] = tokenObj.token;
    this.decodeCurrentToken();
    this.loggedIn.next(true);
  }

  deleteToken(): void {
    localStorage.removeItem('token');
    this.decodedToken = null;
    this.loggedIn.next(false);
  }

  getEncodedToken(): string {
    return localStorage['token'];
  }

  getValidEncodedToken(): string {
    const token = this.getEncodedToken();
    if (!token)
      throw new Error("You don't have a valid token. Please try logging in.");
    return token;
  }

  getToken(): JwtToken {
    return this.getJwtToken();
  }

  getCurrentUserName(): string {
    return this.getToken().username;
  }

  confirmEmail(userId: string, token: string): Observable<Object> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/confirmEmail';
    return this.http.post(apiUrl, {userId: userId, token: token});
  }

  confirmEmailChange(email: string, token: string): Observable<Object> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/confirmEmailChange';
    let sub = this.http.post(apiUrl, {email: email, token: token});
    sub = sub.do(value => this.saveToken(value));
    return sub;
  }

  resendConfirmationEmail(data: any): Observable<Object> {
    const apiUrl = AccountService.baseUrl + 'api/authentication/resendConfirmationEmail';
    return this.http.post(apiUrl, data);
  }

  private downloadToken(apiUrl: string, requestBody: any): Observable<string> {
    let sub = this.http.post<string>(apiUrl, requestBody);
    sub = sub.do(value => this.saveToken(value));
    return sub;
  }

  private decodeCurrentToken(): void {
    this.decodedToken = this.toJwtToken(jwtDecode(this.getValidEncodedToken()));
  }

  private getJwtToken(): JwtToken {
    if (this.decodedToken)
      return this.decodedToken;

    this.decodeCurrentToken();
    return this.decodedToken;
  }

  private toJwtToken(serverToken: any): JwtToken {
    const id = serverToken[AccountService.idClaim];
    const username = serverToken[AccountService.usernameClaim];
    const email = serverToken[AccountService.emailClaim];
    const expirationDate = new Date(serverToken['exp'] * 1000);

    return new JwtToken(id, username, email, expirationDate);
  }
}
