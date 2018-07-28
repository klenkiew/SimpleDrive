class RegisterRequest {

  constructor(public readonly email: string, public readonly password: string,
              public readonly passwordConfirmation: string) {
  }
}
