import {NgModule} from '@angular/core';


import {ErrorComponent} from "./components/error/error.component";
import {NotAuthorizedComponent} from "./components/not-authorized/not-authorized.component";
import {RouterModule} from "@angular/router";
import {CommonModule} from "@angular/common";
import { SizeFormatPipe } from './pipes/size-format.pipe';

@NgModule({
  declarations: [
    ErrorComponent,
    NotAuthorizedComponent,
    SizeFormatPipe
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  providers: [],
  exports: [ErrorComponent, NotAuthorizedComponent, SizeFormatPipe]
})
export class SharedModule { }
