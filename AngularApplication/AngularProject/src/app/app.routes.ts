import {Routes} from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegistrationComponent} from "./registration/registration.component";
import {AccountComponent} from "./account/account.component";
import {ErrorComponent} from "./error/error.component";
import {AboutComponent} from "./about/about.component";
import {FilesComponent} from "./files/files.component";
import {BrowseFilesComponent} from "./browse-files/browse-files.component";
import {AddFileComponent} from "./add-file/add-file.component";
import {FileDetailsComponent} from "./file-details/file-details.component";
import {AccountDetailsComponent} from "./account-details/account-details.component";
import {AccountManageComponent} from "./account-manage/account-manage.component";
import {NotAuthorizedComponent} from "./not-authorized/not-authorized.component";
import {AuthorizationGuardService} from "./authorization-guard.service";

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
