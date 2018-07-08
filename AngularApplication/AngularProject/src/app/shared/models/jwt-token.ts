export class JwtToken {
  constructor(public readonly id: string,public readonly username: string, public readonly email: string,
              public readonly expirationDate: Date) {
  }
}
