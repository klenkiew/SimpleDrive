import {Component, ContentChild, OnInit} from '@angular/core';
import {MenuItem} from "primeng/api";
import {File} from "../model/file";
import {FilesService} from "../files.service";
import {Observable} from "rxjs/Observable";
import 'rxjs/add/operator/map';
import {ActivatedRoute, Router} from "@angular/router";

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

  constructor(private fileService: FilesService, private router: Router, private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.getFiles().subscribe(f => { this.files = f });

    this.cols = [
      { field: 'name', header: 'Name' },
      { field: 'size', header: 'Size' },
      { field: 'lastModified', header: 'Last modification', type: 'date' },
    ];

    this.items = [
      { label: 'Details', icon: 'fa-search', command: (event) => this.showDetails(this.selectedFile) },
      { label: 'Download', icon: 'fa-download', command: (event) => this.download(this.selectedFile) },
      { label: 'Delete', icon: 'fa-close', command: (event) => this.deleteFile(this.selectedFile) }
    ];
  }

  getFiles(): Observable<any>
  {
    return this.fileService.getFiles().map(files => {
      return files.map(f => new File(f.id, f.fileName, f.size, f.dateModified));
    });
  }

  onActivate(event : any) {
    this.showOutlet = true;
  }

  onDeactivate(event : any) {
    this.showOutlet = false;
  }

  private showDetails(selectedFile: File) {
    console.log("Details: " + selectedFile.name);
    this.router.navigate(['details', this.selectedFile.id], {relativeTo: this.route});
  }

  private download(selectedFile: File) {
    console.log("Download: " + selectedFile.name);
    this.fileService.downloadFile(this.selectedFile.id).subscribe(content => {
      console.log('start download');
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
    console.log("Delete: " + selectedFile.name);
    this.fileService.deleteFile(selectedFile.id).subscribe(value =>
    {
      this.files.forEach((item, index) =>
      {
        if (item.name === selectedFile.name)
          this.files.splice(index, 1);
      });
    });
  }
}

