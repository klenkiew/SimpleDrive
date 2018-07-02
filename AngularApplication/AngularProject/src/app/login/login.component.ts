import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../account.service";
import {Message} from "primeng/api";
import {MessageSink, ResultService, ResultServiceFactory} from "../result.service";
import {OperationResult} from "../operation-result";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    resultServiceFactory: ResultServiceFactory) {
    this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit() {
  }

  onSubmit(loginForm) {
    const apiUrl = 'http://localhost:5000/api/authentication/createToken';
    this.http.post<string>(apiUrl, loginForm.value)
      .subscribe(
        value => {
          this.accountService.saveToken(value);
          this.router.navigate(['files']);
        },
        err => {
          const result = new OperationResult(false, "Failed to log in.", err);
          this.resultService.handle(result);
        });
  }
}
