import { Injectable } from '@angular/core';
import {Observable} from "rxjs/Observable";
import {Subject} from "rxjs/Subject";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class FilesService {

  constructor(private http: HttpClient) { }

  public addFile(file: any): void
  {
    const apiUrl = 'http://localhost:5001/api/files';
    this.http.post(apiUrl, file).subscribe(_ => {});
  }

  public deleteFile(fileId: string): void
  {
    const apiUrl = 'http://localhost:5001/api/files/' + fileId;
    this.http.delete(apiUrl).subscribe(_ => {});
  }

  public getFiles(): Observable<any> {
    const apiUrl = 'http://localhost:5001/api/files';
    return this.http.get<any>(apiUrl);
  }
}
