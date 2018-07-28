export class OperationResult {

  constructor(public readonly success: boolean, public readonly message: string, public readonly fullResponse?: any) {
  }
}
