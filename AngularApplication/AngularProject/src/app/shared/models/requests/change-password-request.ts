class ChangePasswordRequest {
  constructor(
    public readonly currentPassword: string,
    public readonly newPassword: string,
    public readonly passwordConfirmation: string) {

  }
}
