import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FilesService} from "../../files.service";
import {ActivatedRoute, Router} from "@angular/router";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-edit-file',
  templateUrl: './edit-file.component.html',
  styleUrls: ['./edit-file.component.css']
})
export class EditFileComponent implements OnInit {

  file: File;
  fileContent: string;
  @ViewChild('file_content_area') textArea: ElementRef;
  private sub: Subscription;

  constructor(private fileService: FilesService, private activatetRoute: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.sub = this.activatetRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      this.fileService.getFile(id).flatMap(f =>
      {
        this.file = new File(f.id, f.fileName, f.size, f.description, new User(f.ownerId, f.ownerName), f.dateModified);
        return this.fileService.downloadFile(f.id);
      }).subscribe(value =>
      {
        console.log('File content: %o', value);
        const reader = new FileReader();
        reader.addEventListener("loadend", () => {
          // reader.result contains the contents of blob as a typed array
          this.fileContent = reader.result;
          (this.textArea.nativeElement as HTMLInputElement).focus();
        });
        reader.readAsText(value);
      })
    })
  }

  submitEdit()
  {
    this.fileService.updateContent(this.file.id, (this.textArea.nativeElement as HTMLInputElement).value)
      .subscribe(value => this.router.navigate(['browse', 'details', this.file.id], {relativeTo: this.activatetRoute.parent}));
  }
}
