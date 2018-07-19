import {Routes} from '@angular/router';
import {ErrorComponent} from "../shared/components/error/error.component";
import {AccountManageComponent} from "./components/account-manage/account-manage.component";
import {AccountDetailsComponent} from "./components/account-details/account-details.component";
import {AccountComponent} from "./components/account/account.component";
import {ChangeEmailConfirmationComponent} from "./components/change-email-confirmation/change-email-confirmation.component";

export const routes: Routes =
  [
    {
      path: '', component: AccountComponent, children: [
        {path: 'details', component: AccountDetailsComponent},
        {path: 'confirm-email-change', component: ChangeEmailConfirmationComponent},
        {path: 'manage', component: AccountManageComponent},
        {path: '', component: AccountComponent, redirectTo: 'details', pathMatch: 'full'},
        {path: '**', component: ErrorComponent}]
    }
  ];
