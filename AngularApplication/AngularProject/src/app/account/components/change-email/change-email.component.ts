import {Component, OnInit} from '@angular/core';
import {OperationResult} from "../../../shared/models/operation-result";
import {MessageSink, ResultService, ResultServiceFactory} from "../../../shared/services/result.service";
import {Message} from "primeng/api";
import {AccountService} from "../../../shared/services/account.service";
import {Form, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html',
  styleUrls: ['./change-email.component.css']
})
export class ChangeEmailComponent implements OnInit {

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  operationPending: boolean = false;

  constructor(private accountService: AccountService, resultServiceFactory: ResultServiceFactory) {

    this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit(): void {
  }

  onEmailChangeSubmit(changeEmailForm: FormGroup): void {
    this.messageSink.messages = [];
    this.operationPending = true;
    this.accountService.changeEmail(changeEmailForm.value)
      .finally(() => this.operationPending = false)
      .subscribe(value => {
          const result = new OperationResult(true, `A confirmation link has been sent to your new e-mail.`);
          this.resultService.handle(result);
        },
        err => {
          const errorMessage = "The operation failed. Try again later.";
          const result = new OperationResult(false, errorMessage, err);
          this.resultService.handle(result);
        });
  }
}
