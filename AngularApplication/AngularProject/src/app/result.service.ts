import {Inject, Injectable, Optional} from '@angular/core';
import {OperationResult} from "./operation-result";
import {Message} from "primeng/api";
import {GrowlMessageSinkToken} from "./growl-message-sink";

@Injectable()
export class ResultService {

  constructor(private resultConsumer: ResultConsumer) { }

  handle(result: OperationResult)
  {
    if (!result.success) {
      console.log('%o', result);
    }
    this.resultConsumer.display(result);
  }
}

interface ResultConsumer {
  display(result: OperationResult): void
}

export class DefaultResultConsumer implements ResultConsumer{

  private messageSink: MessageSink;

  constructor(messageSink: MessageSink) {
    this.messageSink = messageSink;
  }

  display(result: OperationResult): void {
    if (result.success)
      this.messageSink.messages.push({severity: 'success', summary: 'Operation completed', detail: result.message});
    else {
      this.handleError(result);
    }
  }

  private handleError(result: OperationResult) {
    if (!result.fullResponse || !result.fullResponse.error || !result.fullResponse.error.message) {
      this.messageSink.messages.push({severity: 'error', summary: 'Error', detail: result.message});
      return;
    }

    if (!result.fullResponse.error.errors) {
      this.messageSink.messages.push({severity: 'error', summary: 'Error', detail: result.fullResponse.error.message});
      return;
    }

    const summary = result.fullResponse.error.message;
    this.messageSink.messages.push({severity: 'error', summary: summary, detail: ''});
    result.fullResponse.error.errors.forEach(err =>
      this.messageSink.messages.push({severity: 'error', summary: err.field + ':', detail: err.error})
    )
  }
}

export interface MessageSink {
  messages: Message[];
}

export class ResultServiceFactory {
  constructor(@Inject(GrowlMessageSinkToken) private growlMessageSink: MessageSink) {
  }

  public withMessageSink(messageSink: MessageSink): ResultService {
    return this.createService(messageSink);
  }

  public default(): ResultService {
    return this.createService(this.growlMessageSink);
  }

  private createService(messageSink: MessageSink) {
    return new ResultService(new DefaultResultConsumer(messageSink));
  }
}
