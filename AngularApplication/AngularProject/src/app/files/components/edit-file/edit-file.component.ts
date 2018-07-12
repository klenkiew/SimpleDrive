import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FilesService} from "../../files.service";
import {ActivatedRoute, Router} from "@angular/router";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";
import {Subscription} from "rxjs/Subscription";
import {Observable} from "rxjs/Observable";
import {BeforeUnload} from "../../../shared/before-unload";
import {TimerObservable} from "rxjs/observable/TimerObservable";

@Component({
  selector: 'app-edit-file',
  templateUrl: './edit-file.component.html',
  styleUrls: ['./edit-file.component.css']
})
export class EditFileComponent implements OnInit, BeforeUnload {

  file: File;
  fileContent: string;
  toggleLockText: string;
  disableLockButton: boolean;
  textareaDisabled: boolean = true;
  currentLock: any;
  checked: boolean;
  private lockActive: boolean = false;
  @ViewChild('file_content_area') textArea: ElementRef;
  private sub: Subscription;

  constructor(private fileService: FilesService, private activatedRoute: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.sub = this.activatedRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      const fileDownloaded: Observable<any> = this.fileService.getFile(id).flatMap(f =>
      {
        this.file = new File(f.id, f.fileName, f.size, f.description, new User(f.ownerId, f.ownerName), f.dateModified);
        return this.fileService.downloadFile(f.id);
      });

      fileDownloaded.flatMap(value => Observable.create(observer =>
      {
        const reader = new FileReader();
        reader.addEventListener("loadend", () => {
          // reader.result contains the contents of blob as a typed array
          this.fileContent = reader.result;
          (this.textArea.nativeElement as HTMLInputElement).focus();
        });
        reader.readAsText(value);
        observer.next();
      }))
        .flatMap(value =>
      {
        return this.fileService.getFileLock(this.file.id);
      })
        .subscribe(value =>
        {
          this.currentLock = value;
          this.handleFileLock();
        })
    })
  }

  private handleFileLock() {
    if (this.currentLock.isLockPresent)
    {
      const ownerByCurentUser: boolean = this.fileService.isLockOwnedByCurrentUser(this.currentLock);
      this.toggleLockText = 'Locked by ' + this.currentLock.lockOwner.username;
      this.textareaDisabled = !ownerByCurentUser;
      this.disableLockButton = !ownerByCurentUser;
      this.checked = ownerByCurentUser;
    }
    else
    {
      this.toggleLockText = '';
      this.textareaDisabled = true;
      this.disableLockButton = false;
      this.checked = false;
    }
  }

  submitEdit()
  {
    this.fileService.updateContent(this.file.id, (this.textArea.nativeElement as HTMLInputElement).value)
      .subscribe(value => this.router.navigate(['browse', 'details', this.file.id], {relativeTo: this.activatedRoute.parent}));
  }

  toggleLock(event)
  {
    this.disableLockButton = true;
      if (event.checked)
      {
        this.lockActive = true;
        TimerObservable.create(0, 1000*50)
          .flatMap(value => this.fileService.lockFile(this.file.id))
          .takeWhile(() => this.lockActive)
          .flatMap(value =>
          {
            return this.fileService.getFileLock(this.file.id)
          })
          .subscribe(value =>
        {
          this.currentLock = value;
          this.handleFileLock();
        });
      }
      else
      {
        this.fileService.unlockFile(this.file.id).subscribe(value =>
        {
          this.lockActive = false;
          this.currentLock = {isLockPresent: false};
          this.handleFileLock();
        });
      }
  }

  beforeUnload(): Observable<boolean> | Promise<boolean> | boolean {
    this.lockActive = false;
    if (this.currentLock && this.currentLock.isLockPresent && this.fileService.isLockOwnedByCurrentUser(this.currentLock))
      return this.fileService.unlockFile(this.file.id).map(value => true).catch(err => {console.log(err); return Observable.of(true)});
    return true;
  }
}
