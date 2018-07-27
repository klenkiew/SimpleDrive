import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {OperationResult} from "../../../shared/models/operation-result";
import {MessageSink, ResultService, ResultServiceFactory} from "../../../shared/services/result.service";
import {Message} from "primeng/api";
import {AccountService} from "../../../shared/services/account.service";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

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

  ngOnInit(): void {
  }

  onPasswordChangeSubmit(changePasswordForm): void {
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
