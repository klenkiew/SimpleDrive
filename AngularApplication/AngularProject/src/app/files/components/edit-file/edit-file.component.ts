import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FilesService} from "../../files.service";
import {ActivatedRoute, Router} from "@angular/router";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";
import {Subscription} from "rxjs/Subscription";
import {Observable} from "rxjs/Observable";
import {BeforeUnload} from "../../../shared/before-unload";
import {TimerObservable} from "rxjs/observable/TimerObservable";
import {HubConnectionBuilder, HubConnection} from '@aspnet/signalr';

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
  content: string;
  private lockActive: boolean = false;
  @ViewChild('file_content_area') textArea: ElementRef;
  textAreaNativeElement: HTMLInputElement;
  private sub: Subscription;
  private socket: WebSocket;

  constructor(private fileService: FilesService, private activatedRoute: ActivatedRoute, private router: Router) {
  }

  private connection: HubConnection;

  ngOnInit() {
    this.textAreaNativeElement = this.textArea.nativeElement;
    this.sub = this.activatedRoute.params.subscribe(params => {
      const id: string = params['id'];
      const fileDownloaded: Observable<any> = this.fileService.getFile(id).flatMap(f => {
        this.file = f;
        return this.fileService.downloadFile(f.id);
      });

      fileDownloaded.flatMap(value => Observable.create(observer => {
        const reader = new FileReader();
        reader.addEventListener("loadend", () => {
          // reader.result contains the contents of blob as a typed array
          this.fileContent = reader.result;
          this.textAreaNativeElement.focus();
        });
        reader.readAsText(value);
        observer.next();
      }))
        .flatMap(value => {
          return this.fileService.getFileLock(this.file.id);
        })
        .subscribe(value => {
          this.currentLock = value;
          this.handleFileLock();
          this.setupFileLockNotifications();
          this.setupFileContentChangeNotifications();
        });
    })
  }

  private setupFileContentChangeNotifications() {
    this.connection = new HubConnectionBuilder().withUrl("http://localhost:5001/contentChangesHub").build();

    this.connection.on("OnContentChange", (contentChangeInfo) => {
      console.log('Content change: %o', contentChangeInfo);
      if (!contentChangeInfo)
        return;

      if (contentChangeInfo.contentChange && contentChangeInfo.contentChange.newContent) {
        this.textAreaNativeElement.value = contentChangeInfo.contentChange.newContent;
      }

      if (contentChangeInfo.caretChange && contentChangeInfo.caretChange.newSelectionStart) {
        this.textAreaNativeElement.selectionStart = contentChangeInfo.caretChange.newSelectionStart;
        this.textAreaNativeElement.selectionEnd = contentChangeInfo.caretChange.newSelectionEnd;
      }
    });

    this.connection.start()
      .then(() => this.connection.invoke('Subscribe', this.file.id))
      .catch(err => console.error(err.toString()));
  }

  private setupFileLockNotifications() {
    this.socket = new WebSocket("ws://127.0.0.1:5001/ws/fileLocks");
    this.socket.onopen = event => {
      console.log('open');
      this.socket.send(JSON.stringify({fileId: this.file.id}));
    };
    this.socket.onmessage = event => {
      console.log('message: ' + event.data);
      this.currentLock = JSON.parse(event.data).newLock;
      this.handleFileLock()
    };
    this.socket.onclose = event => console.log('close');
  }

  textareaInputChange(event) {
    const newContent: string = this.textAreaNativeElement.value;
    this.connection.invoke("Notify", {FileId: this.file.id, ContentChange: {NewContent: newContent}});
  }

  textareaCaretChange(event) {
    if (this.textareaDisabled)
      return;
    const selectionStart: number = this.textAreaNativeElement.selectionStart;
    const selectionEnd: number = this.textAreaNativeElement.selectionEnd;
    this.connection.invoke("Notify", {
      FileId: this.file.id,
      CaretChange: {NewSelectionStart: selectionStart, NewSelectionEnd: selectionEnd}
    });
  }

  private handleFileLock() {
    if (this.currentLock.isLockPresent) {
      const ownerByCurentUser: boolean = this.fileService.isLockOwnedByCurrentUser(this.currentLock);
      this.toggleLockText = 'Locked by ' + this.currentLock.lockOwner.username;
      this.textareaDisabled = !ownerByCurentUser;
      this.disableLockButton = !ownerByCurentUser;
      this.checked = ownerByCurentUser;
    }
    else {
      this.toggleLockText = '';
      this.textareaDisabled = true;
      this.disableLockButton = false;
      this.checked = false;
    }
  }

  submitEdit() {
    this.fileService.updateContent(this.file.id, this.textAreaNativeElement.value)
      .subscribe(value => {
        this.lockActive = false;
        this.currentLock = {isLockPresent: false};
        this.router.navigate(['browse', 'details', this.file.id], {relativeTo: this.activatedRoute.parent});
      });
  }

  toggleLock(event) {
    this.disableLockButton = true;
    if (event.checked) {
      this.lockActive = true;
      TimerObservable.create(0, 1000 * 50)
        .takeWhile(() => this.lockActive)
        .flatMap(value => this.fileService.lockFile(this.file.id))
        .subscribe(value => {
        });
    }
    else {
      this.lockActive = false;
      this.fileService.unlockFile(this.file.id).subscribe((value) => {
      });
    }
  }

  beforeUnload(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.socket && this.socket.OPEN)
      this.socket.close();

    this.lockActive = false;
    if (this.currentLock && this.currentLock.isLockPresent && this.fileService.isLockOwnedByCurrentUser(this.currentLock))
      return this.fileService.unlockFile(this.file.id).map(value => true).catch(err => {
        console.log(err);
        return Observable.of(true)
      });
    return true;
  }
}
