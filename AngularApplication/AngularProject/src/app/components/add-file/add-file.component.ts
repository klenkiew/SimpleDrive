import {ChangeDetectorRef, Component, OnInit} from "@angular/core";
import {FilesService} from "../../services/files.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-add-file',
  templateUrl: './add-file.component.html',
  styleUrls: ['./add-file.component.css']
})
export class AddFileComponent implements OnInit {

  selectedFile: File = null;

  constructor(private fileService: FilesService, private router: Router) { }

  ngOnInit() {
  }

  fileChange(event): void
  {
    const files: FileList = event.files;
    if (files.length > 0)
      this.selectedFile = files[0];
    else
      this.selectedFile = null;
  }

  clear(event)
  {
    this.selectedFile = null;
  }

  onSubmit(data): void
  {
    const formData: FormData = new FormData();
    formData.append('fileName', data.value.fileName);
    formData.append('description', data.value.description);
    formData.append('file', this.selectedFile, this.selectedFile.name);

    this.fileService.addFile(formData).subscribe(value => this.router.navigate(['files']));
  }
}
