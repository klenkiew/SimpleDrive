import {Component, OnInit} from '@angular/core';
import {OperationResult} from "../../../shared/models/operation-result";
import {MessageSink, ResultService, ResultServiceFactory} from "../../../shared/services/result.service";
import {Message} from "primeng/api";
import {AccountService} from "../../../shared/services/account.service";

@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html',
  styleUrls: ['./change-email.component.css']
})
export class ChangeEmailComponent implements OnInit {

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  constructor(
    private accountService: AccountService,
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
}
