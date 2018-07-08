namespace AuthenticationService.Model
{
    public class User
    {
        public virtual string Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string NormalizedUsername { get; set; }
        public virtual string Email { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
    }
}