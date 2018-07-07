import {Routes} from '@angular/router';
import {LoginComponent} from "./components/login/login.component";
import {RegistrationComponent} from "./components/registration/registration.component";
import {AuthorizationGuardService} from "./infrastructure/authorization-guard.service";
import {AboutComponent} from "./components/about/about.component";
import {NotAuthorizedComponent} from "../shared/components/not-authorized/not-authorized.component";
import {ErrorComponent} from "../shared/components/error/error.component";

export const routes: Routes =
  [
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegistrationComponent},
    {
      path: 'account', canActivate: [AuthorizationGuardService], loadChildren: '../account/account.module#AccountModule'
    },
    {
      path: 'files', canActivate: [AuthorizationGuardService], loadChildren: '../files/files.module#FilesModule'
    },
    {path: 'about', component: AboutComponent},
    {path: '401', component: NotAuthorizedComponent},
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path: '**', component: ErrorComponent}
  ];
