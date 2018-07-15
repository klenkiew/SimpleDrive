import {ChangeDetectorRef, Component, OnDestroy, OnInit} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {FilesService} from "../../files.service";
import {Subscription} from "rxjs/Rx";
import {UsersService} from "../../../shared/users.service";
import {User} from "../../../shared/models/user";
import {File} from "../../../shared/models/file";

@Component({
  selector: 'app-file-details',
  templateUrl: './file-details.component.html',
  styleUrls: ['./file-details.component.css']
})
export class FileDetailsComponent implements OnInit, OnDestroy {

  private file: File = null;
  private sub: Subscription;

  currentUser: User;
  results: User[] = [];
  sharedWith: User[] = [];

  constructor(private fileService: FilesService, private usersService: UsersService,
              private activatetRoute: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.sub = this.activatetRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      this.fileService.getFile(id).subscribe(f =>
      {
        this.file = new File(f.id, f.fileName, f.size, f.description, f.mimeType, new User(f.ownerId, f.ownerName), f.dateCreated);
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
    this.router.navigate(['../../..'], {relativeTo: this.activatetRoute});
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
