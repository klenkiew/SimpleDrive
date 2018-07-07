import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class FilesService {

  private filesApiUrl = 'http://localhost:5001/api/files/';

  constructor(private http: HttpClient) {
  }

  public addFile(file: any): Observable<any> {
    const apiUrl = this.filesApiUrl;
    return this.http.post(apiUrl, file);
  }

  public deleteFile(fileId: string): Observable<any> {
    const apiUrl = this.filesApiUrl + fileId;
    return this.http.delete(apiUrl);
  }

  public downloadFile(fileId: string): Observable<any> {
    const apiUrl = this.filesApiUrl + fileId + "/content";
    return this.http.get(apiUrl, {responseType: 'blob' as 'json'});
  }

  public getFiles(): Observable<any> {
    const apiUrl = this.filesApiUrl;
    return this.http.get<any>(apiUrl);
  }
}
