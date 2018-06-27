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

export const routes: Routes =
  [
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegistrationComponent},
    {path: 'account', component: AccountComponent},
    {
      path: 'files', component: FilesComponent, children:
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
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path: '**', component: ErrorComponent}
  ];
