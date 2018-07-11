import {Routes} from '@angular/router';
import {BrowseFilesComponent} from "./components/browse-files/browse-files.component";
import {AddFileComponent} from "./components/add-file/add-file.component";
import {FileDetailsComponent} from "./components/file-details/file-details.component";
import {ErrorComponent} from "../shared/components/error/error.component";
import {FilesComponent} from "./components/files/files.component";
import {EditFileComponent} from "./components/edit-file/edit-file.component";

export const routes: Routes =
  [
    {
      path: '', component: FilesComponent, children: [
        {
          path: 'browse', component: BrowseFilesComponent, children:
            [
              {path: 'details/:id', component: FileDetailsComponent}
            ]
        },
        {path: 'edit/:id', component: EditFileComponent},
        {path: 'add', component: AddFileComponent},
        {path: '', redirectTo: 'browse', pathMatch: 'full'},
        {path: '**', component: ErrorComponent}
      ]
    }
  ];
