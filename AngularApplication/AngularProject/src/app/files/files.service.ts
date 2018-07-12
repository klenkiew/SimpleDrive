import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/map';
import {HttpClient, HttpParams} from "@angular/common/http";
import {User} from "../shared/models/user";
import {File} from "../shared/models/file";
import {AccountService} from "../shared/services/account.service";

@Injectable()
export class FilesService {

  private filesApiUrl = 'http://localhost:5001/api/files/';
  private sharesApiUrl = 'http://localhost:5001/api/shares/';

  constructor(private http: HttpClient, private accountService: AccountService) {
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

  public getFile(id: string): Observable<any> {
    const apiUrl = this.filesApiUrl + id;
    return this.http.get<any>(apiUrl);
  }

  public getSharedWith(fileId: string): Observable<User[]> {
    const apiUrl = this.sharesApiUrl + fileId;
    return this.http.get<User[]>(apiUrl).map(users => users.map(u => new User(u.id, u.username)));
  }

  shareFile(fileId: string, userId: string) {
    const apiUrl = this.sharesApiUrl;
    return this.http.post(apiUrl, {fileId: fileId, shareWithUserId: userId});
  }

  unshareFile(fileId: string, userId?: string) {
    const apiUrl = this.sharesApiUrl;
    const params = new HttpParams({fromObject: {fileId: fileId, userId: userId || this.accountService.getToken().id}});
    return this.http.delete(apiUrl, {params: params});
  }

  isOwnedByCurrentUser(file: File) {
    return this.accountService.getCurrentUserName() == file.owner.username;
  }

  updateContent(fileId: string, content: string) {
    const apiUrl = this.filesApiUrl + fileId + "/content";
    return this.http.put(apiUrl, {fileId: fileId, content: content});

  }

  getFileLock(fileId: string) {
    const apiUrl = this.filesApiUrl + fileId + "/lock";
    return this.http.get(apiUrl);
  }

  lockFile(fileId: string) {
    const apiUrl = this.filesApiUrl + fileId + "/lock";
    return this.http.post(apiUrl, null);
  }

  unlockFile(fileId: string) {
    const apiUrl = this.filesApiUrl + fileId + "/lock";
    return this.http.delete(apiUrl);
  }

  isLockOwnedByCurrentUser(fileLock: any) {
    return this.accountService.getCurrentUserName() == fileLock.lockOwner.username;
  }
}
