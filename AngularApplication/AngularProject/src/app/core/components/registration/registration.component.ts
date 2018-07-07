import {ChangeDetectorRef, Component, OnInit} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../../../shared/services/account.service";
import {Router} from "@angular/router";
import {MessageSink, ResultService, ResultServiceFactory} from "../../../shared/services/result.service";
import {Message} from "primeng/api";
import {OperationResult} from "../../../shared/models/operation-result";

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  passwordsDontMatch: boolean = false;

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private router: Router,
    private ref: ChangeDetectorRef,
    resultServiceFactory: ResultServiceFactory)
  {
      this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit() {
  }

  onSubmit(registerForm) {
    this.messageSink.messages = [];
    if (registerForm.value.password !== registerForm.value.passwordConfirmation) {
      this.passwordsDontMatch = true;
      this.ref.detectChanges();
      return;
    }

    this.accountService.register(registerForm.value).subscribe(
      value => {
        const route = this.router.createUrlTree(['login']).toString();
        const result = new OperationResult(true, `You have successfully registered. <a href="${route}">Log in</a>`);
        this.resultService.handle(result);
      },
      err => {
        const errorMessage = "The registration process failed. Try again later.";
        const result = new OperationResult(false, errorMessage, err);
        this.resultService.handle(result);
      });
  }

  onPasswordChange(event: any): void {
    this.passwordsDontMatch = false;
  }
}

