import {Message} from "primeng/api";
import {InjectionToken} from "@angular/core";

export let growlMessages: Message[] = [];

export const GrowlMessageSinkToken = new InjectionToken('growlMessageSink');
