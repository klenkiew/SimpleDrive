import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';


import {AppComponent} from './app.component';
import {GrowlModule} from "primeng/growl";
import {MenuComponent} from "./core/components/menu/menu.component";
import {RouterModule} from "@angular/router";
import {CoreModule} from "./core/core.module";

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent
  ],
  imports: [
    BrowserModule,
    CoreModule,
    GrowlModule,
    RouterModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
