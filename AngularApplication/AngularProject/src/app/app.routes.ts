import { Routes } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegistrationComponent} from "./registration/registration.component";
import {AccountComponent} from "./account/account.component";
import {ErrorComponent} from "./error/error.component";
import {AboutComponent} from "./about/about.component";
import {FilesComponent} from "./files/files.component";

export const routes: Routes =
[
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'account', component: AccountComponent },
  { path: 'files', component: FilesComponent },
  { path: 'about', component: AboutComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', component: ErrorComponent }
];
