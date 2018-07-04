import {BrowserModule} from '@angular/platform-browser';
import {ErrorHandler, NgModule} from '@angular/core';


import {AppComponent} from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {MenuComponent} from "./components/menu/menu.component";
import {RegistrationComponent} from "./components/registration/registration.component";
import {LoginComponent} from "./components/login/login.component";
import {AccountComponent} from "./components/account/account.component";
import {RouterModule} from '@angular/router';
import {routes} from "./app.routes";
import {ErrorComponent} from "./components/error/error.component";
import {AccountService} from "./services/account.service";
import {FormsModule} from "@angular/forms";
import {AboutComponent} from "./components/about/about.component";
import {FilesComponent} from "./components/files/files.component";
import {BrowseFilesComponent} from "./components/browse-files/browse-files.component";
import {AddFileComponent} from "./components/add-file/add-file.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {TableModule} from "primeng/table";
import {ContextMenuModule, FieldsetModule, FileUploadModule, InputTextModule, PasswordModule} from "primeng/primeng";
import {TokenInterceptor} from "./infrastructure/token-interceptor";
import {FilesService} from "./services/files.service";
import {FileDetailsComponent} from "./components/file-details/file-details.component";
import {CardModule} from "primeng/card";
import {MessageModule} from "primeng/message";
import {MessagesModule} from "primeng/messages";
import {MessageService} from "primeng/components/common/messageservice";
import {GrowlModule} from "primeng/growl";
import {MessageSink, ResultServiceFactory} from "./services/result.service";
import {growlMessages, GrowlMessageSinkToken} from "./infrastructure/growl-message-sink";
import {DefaultErrorHandler} from "./infrastructure/error-handler";
import { AccountDetailsComponent } from "./components/account-details/account-details.component";
import { AccountManageComponent } from "./components/account-manage/account-manage.component";
import {PanelModule} from "primeng/panel";
import { NotAuthorizedComponent } from "./components/not-authorized/not-authorized.component";
import {AuthorizationGuardService} from "./infrastructure/authorization-guard.service";

const messageSink: MessageSink = {messages: growlMessages};

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
    AccountDetailsComponent,
    AccountManageComponent,
    NotAuthorizedComponent,
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
    FileUploadModule,
    MessagesModule,
    MessageModule,
    GrowlModule,
    PanelModule,
    FieldsetModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    AccountService,
    FilesService,
    MessageService,
    ResultServiceFactory,
    AuthorizationGuardService,
    {
      provide: GrowlMessageSinkToken,
      useValue: messageSink
    },
    {
      provide: ErrorHandler,
      useClass: DefaultErrorHandler,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
