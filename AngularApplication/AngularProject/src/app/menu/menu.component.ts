import {Component, OnDestroy, OnInit} from '@angular/core';
import {AccountService} from "../account.service";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  private loggedIn: boolean;
  private loginStateSub: Subscription;

  constructor(private accountService: AccountService) {
  }

  ngOnInit() {
    this.loginStateSub = this.accountService.isLoggedIn().subscribe(logged => this.loggedIn = logged);
  }

  ngOnDestroy() {
    this.loginStateSub.unsubscribe();
  }
}
