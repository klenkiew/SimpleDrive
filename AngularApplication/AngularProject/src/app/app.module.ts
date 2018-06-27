import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
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
import { BrowseFilesComponent } from './browse-files/browse-files.component';
import { AddFileComponent } from './add-file/add-file.component';
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {TableModule} from "primeng/table";
import {ContextMenuModule, InputTextModule, PasswordModule} from "primeng/primeng";
import {TokenInterceptor} from "./auth/TokenInterceptor";
import {FilesService} from "./files.service";
import { FileDetailsComponent } from './file-details/file-details.component';
import {CardModule} from "primeng/card";

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
    BrowseFilesComponent,
    AddFileComponent,
    FileDetailsComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    TableModule,
    ContextMenuModule,
    PasswordModule,
    InputTextModule,
    CardModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    AccountService,
    FilesService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
