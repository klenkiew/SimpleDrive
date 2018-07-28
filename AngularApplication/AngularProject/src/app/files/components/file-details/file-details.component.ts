import {Component, OnDestroy, OnInit, ViewChild} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {FilesService} from "../../files.service";
import {Subscription} from "rxjs/Rx";
import {UsersService} from "../../../shared/services/users.service";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";
import {OverlayPanel} from "primeng/primeng";
import {AccountService} from "../../../shared/services/account.service";
import {MessageService} from "primeng/components/common/messageservice";
import {FormGroup} from "@angular/forms";

@Component({
  selector: 'app-file-details',
  templateUrl: './file-details.component.html',
  styleUrls: ['./file-details.component.css']
})
export class FileDetailsComponent implements OnInit, OnDestroy {

  private file: File = null;
  private sub: Subscription;

  @ViewChild("op") overlayPanel: OverlayPanel;

  currentUser: User;
  results: User[] = [];
  sharedWith: User[] = [];

  constructor(private fileService: FilesService, private usersService: UsersService, private accountService: AccountService,
              private messageService: MessageService, private activatedRoute: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.sub = this.activatedRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      this.fileService.getFile(id).subscribe(f =>
      {
        this.file = f;
      });
      this.fileService.getSharedWith(id).subscribe(users =>
      {
        this.sharedWith = users;
      });
    })
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  share(): void {
    if (!this.currentUser)
    {
      this.shareError("A user with such name doesn't exist.");
      return;
    }

    if (this.currentUser.id === this.accountService.getToken().id) {
      this.shareError("You can't share a file with yourself.");
      return;
    }

    if (this.sharedWith.find(u => u.id === this.currentUser.id))
    {
      this.shareError("You've already shared this file with this user.");
      return;
    }

    this.fileService.shareFile(this.file.id, this.currentUser.id).subscribe(() =>
    {
      this.sharedWith.push(this.currentUser);
      this.currentUser = null;
    });
  }

  ownedFile(): boolean
  {
    return !this.file || this.fileService.isOwnedByCurrentUser(this.file);
  }

  unshare(userId: string, index: number): void {
    this.fileService.unshareFile(this.file.id, userId).subscribe(() =>
    {
      this.sharedWith.splice(index, 1);
    });
  }

  close(): void {
    this.router.navigate(['../../..'], {relativeTo: this.activatedRoute});
  }

  onEditSubmit(editForm: FormGroup): void {
    this.fileService.editFile(this.file.id, editForm.value).subscribe(() =>
    {
      this.file.fileName = editForm.value.fileName;
      this.file.description = editForm.value.description;
      this.file.dateModified = new Date();
      this.fileService.fileChanged(this.file);
      this.overlayPanel.hide();
    });
  }

  search(event: any): void {
    this.usersService.getUsersByPrefix(event.query).subscribe(data => this.results = data);
  }

  private shareError(message: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Operation failed',
      detail: message
    });
  }
}
