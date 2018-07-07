import {ErrorHandler, NgModule} from '@angular/core';


import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {RouterModule} from '@angular/router';
import {FormsModule} from "@angular/forms";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {TableModule} from "primeng/table";
import {ContextMenuModule, FieldsetModule, FileUploadModule, InputTextModule, PasswordModule} from "primeng/primeng";
import {CardModule} from "primeng/card";
import {MessageModule} from "primeng/message";
import {MessagesModule} from "primeng/messages";
import {MessageService} from "primeng/components/common/messageservice";
import {GrowlModule} from "primeng/growl";
import {PanelModule} from "primeng/panel";
import {MessageSink, ResultServiceFactory} from "../shared/services/result.service";
import {growlMessages, GrowlMessageSinkToken} from "./infrastructure/growl-message-sink";
import {RegistrationComponent} from "./components/registration/registration.component";
import {LoginComponent} from "./components/login/login.component";
import {AboutComponent} from "./components/about/about.component";
import {routes} from "./core.routes";
import {AuthorizationGuardService} from "./infrastructure/authorization-guard.service";
import {DefaultErrorHandler} from "./infrastructure/error-handler";
import {TokenInterceptor} from "./infrastructure/token-interceptor";
import {AccountService} from "../shared/services/account.service";
import {SharedModule} from "../shared/shared.module";
import {CommonModule} from "@angular/common";

const messageSink: MessageSink = {messages: growlMessages};

@NgModule({
  declarations: [
    RegistrationComponent,
    LoginComponent,
    AboutComponent,
  ],
  imports: [
    CommonModule,
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
    SharedModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    MessageService,
    ResultServiceFactory,
    AuthorizationGuardService,
    AccountService,
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
  ]
})
export class CoreModule {
}
