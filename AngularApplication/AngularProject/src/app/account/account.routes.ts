import {Routes} from '@angular/router';
import {ErrorComponent} from "../shared/components/error/error.component";
import {AccountManageComponent} from "./components/account-manage/account-manage.component";
import {AccountDetailsComponent} from "./components/account-details/account-details.component";
import {AccountComponent} from "./components/account/account.component";

export const routes: Routes =
  [
    {
      path: '', component: AccountComponent, children: [
        {path: 'details', component: AccountDetailsComponent},
        {path: 'manage', component: AccountManageComponent},
        {path: '', component: AccountComponent, redirectTo: 'details', pathMatch: 'full'},
        {path: '**', component: ErrorComponent}]
    }
  ];
