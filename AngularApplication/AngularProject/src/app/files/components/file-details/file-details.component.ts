import {Component, OnDestroy, OnInit, ViewChild} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {FilesService} from "../../files.service";
import {Subscription} from "rxjs/Rx";
import {UsersService} from "../../../shared/users.service";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";
import {OverlayPanel} from "primeng/primeng";

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

  constructor(private fileService: FilesService, private usersService: UsersService,
              private activatedRoute: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.sub = this.activatedRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      this.fileService.getFile(id).subscribe(f =>
      {
        this.file = new File(f.id, f.fileName, f.size, f.description, f.mimeType, new User(f.owner.id, f.owner.username), f.dateCreated);
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

  close()
  {
    this.router.navigate(['../../..'], {relativeTo: this.activatedRoute});
  }

  onEditSubmit(editForm)
  {
    this.fileService.editFile(this.file.id, editForm.value).subscribe(value =>
    {
      this.file.name = editForm.value.fileName;
      this.file.description = editForm.value.description;
      this.fileService.fileChanged(this.file);
      this.overlayPanel.hide();
    });
  }

  search(event) {
    this.usersService.getUsersByPrefix(event.query).subscribe(data => {
      this.results = data.map(d =>
      {
        return new User(d.id, d.username);
      });
    });
  }
}
