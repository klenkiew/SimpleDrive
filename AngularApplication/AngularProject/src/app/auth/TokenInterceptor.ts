import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import {AccountService} from "../account.service";

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(public auth: AccountService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.auth.getEncodedToken()}`
      }
    });

    return next.handle(request);
  }
}
