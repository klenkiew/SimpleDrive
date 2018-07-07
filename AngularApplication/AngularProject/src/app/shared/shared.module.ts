import {NgModule} from '@angular/core';


import {ErrorComponent} from "./components/error/error.component";
import {NotAuthorizedComponent} from "./components/not-authorized/not-authorized.component";
import {RouterModule} from "@angular/router";
import {CommonModule} from "@angular/common";

@NgModule({
  declarations: [
    ErrorComponent,
    NotAuthorizedComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  providers: [],
  exports: [ErrorComponent, NotAuthorizedComponent]
})
export class SharedModule { }
