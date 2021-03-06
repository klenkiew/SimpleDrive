import { Injectable } from "@angular/core";
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs/Rx";
import {AccountService} from "../../shared/services/account.service";

@Injectable()
export class AuthorizationGuardService implements CanActivate {

  constructor(private router: Router, private accountService: AccountService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    this.accountService.checkTokenValidity();
    if (this.accountService.isLoggedIn())
      return true;

    this.router.navigate(['401']);
    return false;
  }
}
