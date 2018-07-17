import {Component, OnDestroy, OnInit} from "@angular/core";
import {AccountService} from "../../../shared/services/account.service";
import {Subscription} from "rxjs/Rx";
import {Observable} from "rxjs/Observable";

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  private readonly smallScreenWidth = 768;

  private loggedIn: boolean;
  private loginStateSub: Subscription;

  private menuExpanded: boolean;
  private smallScreen: boolean;
  private screenSizeSub: Subscription;

  get showMenu(): boolean
  {
    return !this.smallScreen || this.menuExpanded;
  }

  constructor(private accountService: AccountService) {
  }

  ngOnInit() {
    this.loginStateSub = this.accountService.loggedInChange().subscribe(logged => this.loggedIn = logged);

    // Checks if screen size is less than 768 pixels
    const checkScreenSize = () => document.body.offsetWidth < this.smallScreenWidth;

    // Create observable from window resize event throttled so only fires every 500ms
    const screenSizeChanged$ = Observable.fromEvent(window, 'resize').throttleTime(500).map(checkScreenSize);
    this.screenSizeSub = screenSizeChanged$.subscribe(value => this.smallScreen = value);
  }

  ngOnDestroy() {
    this.loginStateSub.unsubscribe();
    this.screenSizeSub.unsubscribe();
  }

  toggleMenu(): void
  {
    this.menuExpanded = !this.menuExpanded;
  }
}
