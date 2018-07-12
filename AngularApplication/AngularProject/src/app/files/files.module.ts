import {NgModule} from '@angular/core';


import {FilesService} from "./files.service";
import {RouterModule} from "@angular/router";
import {routes} from "./files.routes";
import {FilesComponent} from "./components/files/files.component";
import {BrowseFilesComponent} from "./components/browse-files/browse-files.component";
import {AddFileComponent} from "./components/add-file/add-file.component";
import {FileDetailsComponent} from "./components/file-details/file-details.component";
import {SharedModule} from "../shared/shared.module";
import {GrowlModule} from "primeng/growl";
import {
  AutoCompleteModule,
  ContextMenuModule, DataScrollerModule,
  FieldsetModule,
  FileUploadModule, InputTextareaModule,
  InputTextModule,
  PasswordModule, ToggleButtonModule
} from "primeng/primeng";
import {PanelModule} from "primeng/panel";
import {TableModule} from "primeng/table";
import {MessagesModule} from "primeng/messages";
import {MessageModule} from "primeng/message";
import {CardModule} from "primeng/card";
import {FormsModule} from "@angular/forms";
import {CommonModule} from "@angular/common";
import {UsersService} from "../shared/users.service";
import { EditFileComponent } from './components/edit-file/edit-file.component';
import {BeforeUnloadGuardService} from "../shared/services/before-unload-guard.service";

@NgModule({
  declarations: [
    FilesComponent,
    BrowseFilesComponent,
    AddFileComponent,
    FileDetailsComponent,
    EditFileComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes),
    AutoCompleteModule,
    DataScrollerModule,
    InputTextareaModule,
    ToggleButtonModule,

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
  providers: [FilesService, UsersService, BeforeUnloadGuardService],
})
export class FilesModule { }
