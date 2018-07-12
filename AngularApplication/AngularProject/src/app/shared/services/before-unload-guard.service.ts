import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs/Observable";
import {BeforeUnload} from '../before-unload'

@Injectable()
export class BeforeUnloadGuardService implements CanDeactivate<BeforeUnload> {

  constructor() { }

  canDeactivate(
    component: BeforeUnload,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState: RouterStateSnapshot
  ): Observable<boolean>|Promise<boolean>|boolean {
    return component.beforeUnload();
  }
}
