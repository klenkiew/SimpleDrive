import {Observable} from "rxjs/Observable";

export interface BeforeUnload {
  beforeUnload(): Observable<boolean> | Promise<boolean> | boolean;
}
