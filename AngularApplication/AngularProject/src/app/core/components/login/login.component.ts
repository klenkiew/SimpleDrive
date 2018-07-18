import {Component, OnInit, ViewChild} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../../../shared/services/account.service";
import {MenuItem, Message} from "primeng/api";
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

  @ViewChild("loginForm") form;

  items: MenuItem[];

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    resultServiceFactory: ResultServiceFactory) {
    this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit() {
    this.items = [
      {label: 'Resend e-mail confirmation', icon: 'fa-refresh', command: () => {
          this.resendConfirmationEmail(this.form);
        }},
    ];
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

  private resendConfirmationEmail(loginForm) {
    this.accountService.resendConfirmationEmail(loginForm.value)
      .subscribe(
        value => {
          const result = new OperationResult(true, "Confirmation e-mail resend successfully.", value);
          this.resultService.handle(result);
        },
        err => {
          const result = new OperationResult(false, "Failed to request a new confirmation e-mail.", err);
          this.resultService.handle(result);
        });
  }
}
