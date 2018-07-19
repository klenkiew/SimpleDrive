import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from "@angular/router";
import {AccountService} from "../../../shared/services/account.service";
import {Message} from "primeng/api";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.css']
})
export class EmailConfirmationComponent implements OnInit, OnDestroy {

  inProgress: boolean = true;
  messages: Message[] = [];

  private sub: Subscription;

  constructor(private accountSerivce: AccountService, private activatedRoute: ActivatedRoute, private router: Router) {
  }

  ngOnInit() {
    this.sub = this.activatedRoute.queryParams.subscribe(params => {

      const userId: string = params.userId;
      const token: string = params.token;

      this.accountSerivce.confirmEmail(userId, token)
        .finally(() => this.inProgress = false)
        .subscribe(value => {
          const route = this.router.createUrlTree(['login']).toString();
          this.messages.push({
            summary: 'E-mail confirmed',
            severity: 'success',
            detail: `You e-mail has been successfully confirmed. You can now try to <a href="${route}">log in</a>.`
          });
        }, err => {
          console.log('E-mail confirmation error:\n%o', err);
          this.messages.push({
            summary: 'E-mail confirmation failed',
            severity: 'error',
            detail: 'An error occured during confirming your e-mail. Please try again later or generate a new link.'
          });
        });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

}
