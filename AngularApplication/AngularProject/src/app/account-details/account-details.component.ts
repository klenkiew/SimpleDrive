import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {AccountService} from "../account.service";
import {JwtToken} from "../model/JwtToken";
import {OperationResult} from "../operation-result";
import {MessageSink, ResultService, ResultServiceFactory} from "../result.service";
import {Message} from "primeng/api";

@Component({
  selector: 'app-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.css']
})
export class AccountDetailsComponent implements OnInit {

  private username: string;
  private email: string;
  private expirationDate: Date;

  private resultService: ResultService;
  private messageSink: MessageSink = {messages: <Message[]>[]};

  constructor(private accountService: AccountService, private router: Router, resultServiceFactory: ResultServiceFactory) {
    this.resultService = resultServiceFactory.withMessageSink(this.messageSink);
  }

  ngOnInit() {
    this.updateAccountInfo();

    // let convertedDateString = token.expirationDate.toLocaleString();
    // convertedDateString = convertedDateString.replace('at ', '');
    // this.expirationDate = new Date(convertedDateString);
  }

  logout(): void {
    this.accountService.deleteToken();
    this.router.navigate(['/login']);
  }

  refreshToken(): void {
    this.accountService.refreshToken().subscribe(value => {
      this.updateAccountInfo();
      const result = new OperationResult(true, "The token has been successfully refreshed", {});
      this.resultService.handle(result);
    }, error => {
      const result = new OperationResult(false, "Failed to refresh the token", error);
      this.resultService.handle(result);
    });
  }

  private updateAccountInfo() {
    const token: JwtToken = this.accountService.getToken();
    this.username = token.username;
    this.email = token.email;
    this.expirationDate = token.expirationDate;
  }
}
