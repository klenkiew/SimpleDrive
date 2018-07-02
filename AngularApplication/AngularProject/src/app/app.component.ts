import {Component, Inject} from '@angular/core';
import {GrowlMessageSinkToken} from "./growl-message-sink";
import {Message} from "primeng/api";
import {MessageSink} from "./result.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  messageSink: MessageSink;

  constructor(@Inject(GrowlMessageSinkToken) messageSink: MessageSink) {
    this.messageSink = messageSink;
  }
}
