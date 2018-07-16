import {Component, OnInit} from "@angular/core";
import {MenuItem} from "primeng/api";
import {File} from "../../../shared/models/file";
import {FilesService} from "../../files.service";
import {Observable} from "rxjs/Rx";
import "rxjs/add/operator/map";
import {ActivatedRoute, Router} from "@angular/router";
import {User} from "../../../shared/models/user";
import {MessageService} from "primeng/components/common/messageservice";

@Component({
  selector: 'app-browse-files',
  templateUrl: './browse-files.component.html',
  styleUrls: ['./browse-files.component.css']
})
export class BrowseFilesComponent implements OnInit {

  files: File[];
  cols: any[];
  selectedFile: File;
  items: MenuItem[];

  showOutlet: boolean = false;

  constructor(
    private fileService: FilesService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService) {
  }

  ngOnInit() {
    this.getFiles().subscribe(f => { this.files = f });

    this.cols = [
      { field: 'name', header: 'Name' },
      { field: 'size', header: 'Size', type: 'size' },
      { field: 'ownerName', header: 'Owner'},
      { field: 'dateCreated', header: 'Created', type: 'date' },
    ];

    this.items = [
      { label: 'Details', icon: 'fa-search', command: (event) => this.showDetails(this.selectedFile) },
      { label: 'Edit', icon: 'fa-edit', command: (event) => this.editFile(this.selectedFile) },
      { label: 'Download', icon: 'fa-download', command: (event) => this.download(this.selectedFile) },
      { label: 'Delete', icon: 'fa-close', command: (event) => this.deleteFile(this.selectedFile) }
    ];
  }

  getFiles(): Observable<any>
  {
    return this.fileService.getFiles().map(files => {
      return files.map(f => new File(f.id, f.fileName, f.size, f.description, f.mimeType, new User(f.owner.id, f.owner.username), f.dateCreated));
    });
  }

  onActivate(event : any) {
    this.showOutlet = true;
  }

  onDeactivate(event : any) {
    this.showOutlet = false;
  }

  private showDetails(selectedFile: File) {
    this.router.navigate(['details', this.selectedFile.id], {relativeTo: this.route});
  }

  private download(selectedFile: File) {
    this.fileService.downloadFile(this.selectedFile.id).subscribe(content => {
      const url = window.URL.createObjectURL(content);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.setAttribute('style', 'display: none');
      a.href = url;
      a.download = this.selectedFile.name;
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove(); // remove the element
    }, error => {
      console.log('download error:', JSON.stringify(error));
    }, () => {
      console.log('Completed file download.')
    })
  }

  private deleteFile(selectedFile: File): void {
    let sub: Observable<any>;
    if (this.fileService.isOwnedByCurrentUser(selectedFile))
      sub = this.fileService.deleteFile(selectedFile.id);
    else
      sub = this.fileService.unshareFile(selectedFile.id);

    sub.subscribe(value =>
    {
      this.files.forEach((item, index) =>
      {
        if (item.name === selectedFile.name)
          this.files.splice(index, 1);
      });
    });
  }

  private editFile(selectedFile: File) {
    if (selectedFile.mimeType !== 'text/plain')
      {
        const message = {severity: 'info', summary: 'No editor for this file',
          detail: 'Currently only plain text files can be edited.'};
        this.messageService.add(message);
        return;
      }
    this.router.navigate(['edit', this.selectedFile.id], {relativeTo: this.route.parent});
  }
}

