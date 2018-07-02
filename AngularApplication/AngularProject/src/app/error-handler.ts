import {ErrorHandler, Injectable} from "@angular/core";
import {ResultService, ResultServiceFactory} from "./result.service";
import {OperationResult} from "./operation-result";

@Injectable()
export class DefaultErrorHandler implements ErrorHandler {

  private resultService: ResultService;

  constructor(resultServiceFactory: ResultServiceFactory)
  {
    this.resultService = resultServiceFactory.default();
  }

  handleError(error) {
    const result = new OperationResult(false, "An unexpected error occured", error);
    this.resultService.handle(result);
  }
}
