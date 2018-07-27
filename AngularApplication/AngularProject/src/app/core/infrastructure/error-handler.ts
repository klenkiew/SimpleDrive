import {ErrorHandler, Injectable} from "@angular/core";
import {ResultService, ResultServiceFactory} from "../../shared/services/result.service";
import {OperationResult} from "../../shared/models/operation-result";

@Injectable()
export class DefaultErrorHandler implements ErrorHandler {

  private resultService: ResultService;

  constructor(resultServiceFactory: ResultServiceFactory)
  {
    this.resultService = resultServiceFactory.default();
  }

  handleError(error): void {
    const errorMessage = error && error.status && error.status === 401
      ? 'You are not authorized to perform this operation (401 Forbidden)'
      : 'An unexpected error occured' + (error.message ? ': ' + error.message : '');
    const result = new OperationResult(false, errorMessage, error);
    this.resultService.handle(result);
  }
}
