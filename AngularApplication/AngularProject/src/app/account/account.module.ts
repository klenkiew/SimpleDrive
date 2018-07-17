import {NgModule} from '@angular/core';


import {AccountComponent} from "./components/account/account.component";
import {AccountDetailsComponent} from "./components/account-details/account-details.component";
import {AccountManageComponent} from "./components/account-manage/account-manage.component";
import {RouterModule} from "@angular/router";
import {routes} from "./account.routes";
import {SharedModule} from "../shared/shared.module";
import {GrowlModule} from "primeng/growl";
import {ContextMenuModule, FieldsetModule, FileUploadModule, InputTextModule, PasswordModule} from "primeng/primeng";
import {PanelModule} from "primeng/panel";
import {TableModule} from "primeng/table";
import {MessagesModule} from "primeng/messages";
import {MessageModule} from "primeng/message";
import {CardModule} from "primeng/card";
import {FormsModule} from "@angular/forms";
import {CommonModule} from "@angular/common";
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { ChangeEmailComponent } from './components/change-email/change-email.component';

@NgModule({
  declarations: [
    AccountComponent,
    AccountDetailsComponent,
    AccountManageComponent,
    ChangePasswordComponent,
    ChangeEmailComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes),

    FormsModule,
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
    FieldsetModule
  ],
  providers: [],
})
export class AccountModule { }
