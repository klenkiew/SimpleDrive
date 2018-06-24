import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../account.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private http: HttpClient, private accountService: AccountService) { }

  ngOnInit() {
  }

  onSubmit(loginForm) {
    const apiUrl = 'http://localhost:5000/api/authentication/createToken';
    this.http.post<string>(apiUrl,loginForm.value)
      .subscribe(
        value => {
          console.log("Login OK");
          this.accountService.saveToken(value);
        },
          err => alert(JSON.stringify(err)));
  }
}
