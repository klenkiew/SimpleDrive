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
  PasswordModule
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
  providers: [FilesService, UsersService],
})
export class FilesModule { }
