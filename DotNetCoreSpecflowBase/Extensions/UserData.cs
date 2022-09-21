namespace DotNetCoreSpecflowBase.Extensions
{
    public class UserData
    {
        public Usersdetails UsersDetails { get; set; }
    }

    public class Usersdetails
    {
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string surname { get; set; }
        public Identity[] identities { get; set; }
        public Passwordprofile[] PasswordProfile { get; set; }
        public string PasswordPolicies { get; set; }
    }

    public class Identity
    {
        public string signInType { get; set; }
        public string issuerAssignedId { get; set; }
        public string Issuer { get; set; }
    }

    public class Passwordprofile
    {
        public string Password { get; set; }
        public string ForceChangePasswordNextSignIn { get; set; }
    }

}


