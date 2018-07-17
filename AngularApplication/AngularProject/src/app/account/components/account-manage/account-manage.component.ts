import {Component, OnInit} from "@angular/core";

@Component({
  selector: 'app-account-manage',
  templateUrl: './account-manage.component.html',
  styleUrls: ['./account-manage.component.css']
})
export class AccountManageComponent implements OnInit {

  collapsed: boolean = true;

  constructor() {
  }

  ngOnInit() {
    if (window.innerWidth > 992)
      this.collapsed = false;
  }
}
