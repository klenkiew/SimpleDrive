import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AccountService} from "../account.service";
import {Router} from "@angular/router";
import {ResultFunc} from "rxjs/observable/GenerateObservable";

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  passwordsDontMatch: boolean = false;
  result: RegisterResult = null;

  constructor(private http: HttpClient, private accountService: AccountService, private router: Router, private ref: ChangeDetectorRef) { }

  ngOnInit() {
  }

  onSubmit(registerForm) {
    this.result = null;
    if (registerForm.value.password !== registerForm.value.passwordConfirmation) {
      this.passwordsDontMatch = true;
      this.ref.detectChanges();
      return;
    }

    this.accountService.register(registerForm.value).subscribe(
      value => {
        console.log("success");
        this.result = new RegisterResult(true, "You have successfully registered.");
      },
      err => {
        console.log("error");
        this.result = new RegisterResult(false, "The registration process failed. Try again later.")
      });
  }

  onPasswordChange(event: any): void {
    this.passwordsDontMatch = false;
  }
}

class RegisterResult {
  success: boolean;
  message: string;

  constructor(success: boolean, message: string) {
    this.success = success;
    this.message = message;
  }
}
