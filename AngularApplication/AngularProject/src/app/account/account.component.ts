import { Component, OnInit } from '@angular/core';
import {AccountService} from "../account.service";
import * as jwtDecode from "jwt-decode"
import {Router} from "@angular/router";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {

  private token: string;

  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit() {
    this.token = JSON.stringify(jwtDecode(JSON.parse(this.accountService.getToken()).token));
  }

  logout(): void
  {
    this.accountService.deleteToken();
    this.router.navigate(['/login']);
  }
}
