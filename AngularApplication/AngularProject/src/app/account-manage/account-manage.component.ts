import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {OperationResult} from "../operation-result";
import {Message} from "primeng/api";
import {MessageSink, ResultService, ResultServiceFactory} from "../result.service";
import {AccountService} from "../account.service";

@Component({
  selector: 'app-account-manage',
  templateUrl: './account-manage.component.html',
  styleUrls: ['./account-manage.component.css']
})
export class AccountManageComponent implements OnInit {

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  passwordsDontMatch: boolean = false;

  constructor(
    private accountService: AccountService,
    private ref: ChangeDetectorRef,
    resultServiceFactory: ResultServiceFactory)
  {
    this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit() {
  }

  onEmailChangeSubmit(changeEmailForm) {
    this.messageSink.messages = [];
    this.accountService.changeEmail(changeEmailForm.value).subscribe(
      value => {
        const result = new OperationResult(true, `Your e-mail has been changed successfully.`);
        this.resultService.handle(result);
      },
      err => {
        const errorMessage = "The operation failed. Try again later.";
        const result = new OperationResult(false, errorMessage, err);
        this.resultService.handle(result);
      });
  }


  onPasswordChangeSubmit(changePasswordForm) {
    this.messageSink.messages = [];
    if (changePasswordForm.value.newPassword !== changePasswordForm.value.passwordConfirmation) {
      this.passwordsDontMatch = true;
      this.ref.detectChanges();
      return;
    }

    this.accountService.changePassword(changePasswordForm.value).subscribe(
      value => {
        const result = new OperationResult(true, `Your password has been changed successfully.`);
        this.resultService.handle(result);
      },
      err => {
        const errorMessage = "The operation failed. Try again later.";
        const result = new OperationResult(false, errorMessage, err);
        this.resultService.handle(result);
      });
  }

  onPasswordChange(event: any): void {
    this.passwordsDontMatch = false;
  }
}
