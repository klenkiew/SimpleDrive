import {Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild} from "@angular/core";
import {MenuItem} from "primeng/api";
import {File} from "../../../shared/models/file";
import {FilesService} from "../../files.service";
import {Observable} from "rxjs/Rx";
import "rxjs/add/operator/map";
import {ActivatedRoute, Router} from "@angular/router";
import {User} from "../../../shared/models/user";
import {MessageService} from "primeng/components/common/messageservice";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-browse-files',
  templateUrl: './browse-files.component.html',
  styleUrls: ['./browse-files.component.css']
})
export class BrowseFilesComponent implements OnInit, OnDestroy {

  files: File[];
  selectedFile: File;
  items: MenuItem[];

  @ViewChild('cm') contextMenu: ElementRef;

  showOutlet: boolean = false;
  private getFilesSub: Subscription;
  private fileChangedSub: Subscription;

  constructor(
    private fileService: FilesService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService) {
  }

  ngOnInit(): void {
    this.getFilesSub = this.getFiles().subscribe(f => { this.files = f });
    this.fileChangedSub = this.fileService.onFileChanged().subscribe(f =>
    {
      const changedFile = this.files.find(file => file.id == f.id);
      changedFile.fileName = f.fileName;
      changedFile.description = f.description;
    });

    this.items = [
      { label: 'Details', icon: 'fa fa-info-circle', command: (event) => this.showDetails(this.selectedFile) },
      { label: 'Edit', icon: 'fa fa-align-left', command: (event) => this.editFile(this.selectedFile) },
      { label: 'Download', icon: 'fa fa-download', command: (event) => this.download(this.selectedFile) },
      { label: 'Delete', icon: 'fa fa-close', command: (event) => this.deleteFile(this.selectedFile) }
    ];
  }

  ngOnDestroy(): void {
    this.getFilesSub.unsubscribe();
    this.fileChangedSub.unsubscribe();
  }

  getFiles(): Observable<File[]>
  {
    return this.fileService.getFiles();
  }

  onActivate(event : any): void {
    this.showOutlet = true;
  }

  onDeactivate(event : any): void {
    this.showOutlet = false;
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event: Event): void {
    event.preventDefault();
  }

  iconClassForFile(file): string
  {
    if (file.mimeType === null)
      return 'fa-file';
    else if (file.mimeType.startsWith('text/'))
      return 'fa-file-text';
    else if (file.mimeType.startsWith('image/'))
      return 'fa-file-image-o';
    else if (file.mimeType.startsWith('audio/'))
      return 'fa-file-audio-o';
    else if (file.mimeType.startsWith('video/'))
      return 'fa-file-video-o';
    return 'fa-file';
  }

  fileSelected(file): void {
    this.selectedFile = file;
  }

  private showDetails(selectedFile: File): void {
    this.router.navigate(['details', this.selectedFile.id], {relativeTo: this.route});
  }

  private download(selectedFile: File): void {
    this.fileService.downloadFile(this.selectedFile.id).subscribe(content => {
      const url = window.URL.createObjectURL(content);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.setAttribute('style', 'display: none');
      a.href = url;
      a.download = this.selectedFile.fileName;
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
        if (item.fileName === selectedFile.fileName)
          this.files.splice(index, 1);
      });
    });
  }

  private editFile(selectedFile: File): void {
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

