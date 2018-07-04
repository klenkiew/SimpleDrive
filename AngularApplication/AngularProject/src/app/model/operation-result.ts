export class OperationResult {
  success: boolean;
  message: string;
  fullResponse: any;

  constructor(success: boolean, message: string, fullResponse?: any) {
    this.success = success;
    this.message = message;
    this.fullResponse = fullResponse;
  }
}
