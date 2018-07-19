import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs/Subscription";
import {Message} from "primeng/api";
import {AccountService} from "../../../shared/services/account.service";

@Component({
  selector: 'app-change-email-confirmation',
  templateUrl: './change-email-confirmation.component.html',
  styleUrls: ['./change-email-confirmation.component.css']
})
export class ChangeEmailConfirmationComponent implements OnInit, OnDestroy {

  inProgress: boolean = true;
  messages: Message[] = [];

  private sub: Subscription;

  constructor(private accountSerivce: AccountService, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {
    this.sub = this.activatedRoute.queryParams.subscribe(params => {

      const email: string = params.email;
      const token: string = params.token;

      this.accountSerivce.confirmEmailChange(email, token).subscribe(value => {
        this.messages.push({
          summary: 'E-mail changed',
          severity: 'success',
          detail: `Your e-mail has been changed successfully.`
        });
      }, err => {
        console.log('E-mail change confirmation error:\n%o', err);
        this.messages.push({
          summary: 'E-mail change confirmation failed',
          severity: 'error',
          detail: 'An error occured during confirming your new e-mail. Please try again later.'
        });
      }, () => this.inProgress = false);
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
