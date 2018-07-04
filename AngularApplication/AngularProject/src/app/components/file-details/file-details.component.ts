import {ChangeDetectorRef, Component, OnDestroy, OnInit} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {FilesService} from "../../services/files.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-file-details',
  templateUrl: './file-details.component.html',
  styleUrls: ['./file-details.component.css']
})
export class FileDetailsComponent implements OnInit, OnDestroy {

  private file: any = null;
  private sub: Subscription;

  constructor(private fileService: FilesService, private activatetRoute: ActivatedRoute, private router: Router,
              private changeDetection: ChangeDetectorRef) { }

  ngOnInit() {
    this.sub = this.activatetRoute.params.subscribe(params =>
    {
      const id: string = params['id'];
      this.fileService.getFiles().map(files => files.find(f => f.id == id)).subscribe(f =>
      {
        const file: any = f;
        this.file = {
          id: file.id,
          description: file.description
        };
        this.changeDetection.detectChanges();
      });
    })
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  public close()
  {
    this.router.navigate(['../../..'], {relativeTo: this.activatetRoute});
  }
}
