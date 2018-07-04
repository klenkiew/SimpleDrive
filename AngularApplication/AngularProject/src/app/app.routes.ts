import {Routes} from '@angular/router';
import {LoginComponent} from "./components/login/login.component";
import {RegistrationComponent} from "./components/registration/registration.component";
import {AccountComponent} from "./components/account/account.component";
import {ErrorComponent} from "./components/error/error.component";
import {AboutComponent} from "./components/about/about.component";
import {FilesComponent} from "./components/files/files.component";
import {BrowseFilesComponent} from "./components/browse-files/browse-files.component";
import {AddFileComponent} from "./components/add-file/add-file.component";
import {FileDetailsComponent} from "./components/file-details/file-details.component";
import {AccountDetailsComponent} from "./components/account-details/account-details.component";
import {AccountManageComponent} from "./components/account-manage/account-manage.component";
import {NotAuthorizedComponent} from "./components/not-authorized/not-authorized.component";
import {AuthorizationGuardService} from "./infrastructure/authorization-guard.service";

export const routes: Routes =
  [
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegistrationComponent},
    {
      path: 'account', component: AccountComponent, canActivate: [AuthorizationGuardService], children:
        [
          {path: 'details', component: AccountDetailsComponent},
          {path: 'manage', component: AccountManageComponent},
          {path: '', redirectTo: 'details', pathMatch: 'full'},
          {path: '**', component: ErrorComponent}
        ]
    },
    {
      path: 'files', component: FilesComponent, canActivate: [AuthorizationGuardService], children:
        [
          {
            path: 'browse', component: BrowseFilesComponent, children:
              [
                {path: 'details/:id', component: FileDetailsComponent}
              ]
          },
          {path: 'add', component: AddFileComponent},
          {path: '', redirectTo: 'browse', pathMatch: 'full'},
          {path: '**', component: ErrorComponent}
        ]
    },
    {path: 'about', component: AboutComponent},
    {path: '401', component: NotAuthorizedComponent},
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path: '**', component: ErrorComponent}
  ];
