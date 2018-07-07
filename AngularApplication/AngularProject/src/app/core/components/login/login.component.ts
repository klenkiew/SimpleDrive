import {Component, OnInit} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../../../shared/services/account.service";
import {Message} from "primeng/api";
import {MessageSink, ResultService, ResultServiceFactory} from "../../../shared/services/result.service";
import {ActivatedRoute, Router} from "@angular/router";
import {OperationResult} from "../../../shared/models/operation-result";

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
    this.accountService.login(loginForm.value)
      .subscribe(
        value => {
          this.router.navigate(['files']);
        },
        err => {
          const result = new OperationResult(false, "Failed to log in.", err);
          this.resultService.handle(result);
        });
  }
}
