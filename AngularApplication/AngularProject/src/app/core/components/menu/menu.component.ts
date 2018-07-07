import {Component, OnDestroy, OnInit} from "@angular/core";
import {AccountService} from "../../../shared/services/account.service";
import {Subscription} from "rxjs/Rx";

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
    this.loginStateSub = this.accountService.loggedInChange().subscribe(logged => this.loggedIn = logged);
  }

  ngOnDestroy() {
    this.loginStateSub.unsubscribe();
  }
}
