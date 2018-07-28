import {Component, OnInit} from "@angular/core";
import {FilesService} from "../../files.service";
import {Router} from "@angular/router";
import {FormGroup} from "@angular/forms";

@Component({
  selector: 'app-add-file',
  templateUrl: './add-file.component.html',
  styleUrls: ['./add-file.component.css']
})
export class AddFileComponent implements OnInit {

  selectedFile: File = null;

  constructor(private fileService: FilesService, private router: Router) { }

  ngOnInit(): void {
  }

  fileChange(event: any): void
  {
    const files: FileList = event.files;
    if (files.length > 0)
      this.selectedFile = files[0];
    else
      this.selectedFile = null;
  }

  clear(event: any): void {
    this.selectedFile = null;
  }

  onSubmit(data: FormGroup): void
  {
    const formData: FormData = new FormData();
    formData.append('fileName', data.value.fileName);
    formData.append('description', data.value.description);
    formData.append('file', this.selectedFile, this.selectedFile.name);

    this.fileService.addFile(formData).subscribe(() => this.router.navigate(['files']));
  }
}
