export class JwtToken {
  constructor(public readonly username: string, public readonly email: string, public readonly expirationDate: Date) {
  }
}
