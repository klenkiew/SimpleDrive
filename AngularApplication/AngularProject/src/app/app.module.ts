import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import {AppService} from "./app.service";
import {HttpClientModule} from "@angular/common/http";
import { MenuComponent } from './menu/menu.component';
import { RegistrationComponent } from './registration/registration.component';
import { LoginComponent } from './login/login.component';
import { AccountComponent } from './account/account.component';
import { RouterModule } from '@angular/router';
import {routes} from "./app.routes";
import { ErrorComponent } from './error/error.component';
import {AccountService} from "./account.service";
import {FormsModule} from "@angular/forms";
import { AboutComponent } from './about/about.component';
import { FilesComponent } from './files/files.component';

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    RegistrationComponent,
    LoginComponent,
    AccountComponent,
    ErrorComponent,
    AboutComponent,
    FilesComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes)
  ],
  providers: [AppService, AccountService],
  bootstrap: [AppComponent]
})
export class AppModule { }
