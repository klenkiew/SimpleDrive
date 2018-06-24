import { Component } from '@angular/core';
import { AppService } from './app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  games: string[];
  constructor(private appService: AppService)
  {
    this.games = [];
    this.populateGames();
  }

  populateGames()
  {
    this.appService.getGames().subscribe(res  => {
      this.games = res;
    });
  }
}
