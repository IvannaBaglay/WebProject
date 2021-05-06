using AuthenticationService.Helpers;

namespace AuthenticationService.Models
{
    public class User
    {
        public User Clone() => MemberwiseClone() as User;
        public int id { get; set; }
        public string login { get; set; }
        public byte[] password { get; set; }
        public byte[] passwordSalt { get; set; }
        public string role { get; set; }

        public void FromUserChangeModel(UserChangeModel model)
        {
            this.login = model.login;

            byte[] passwordHash, salt;
            PasswordHelper.CreatePasswordHash(model.password, out passwordHash, out salt);
            this.password = passwordHash;
            this.passwordSalt = salt;
        }
        public static bool operator ==(User u1, User u2) =>
            u1?.login == u2?.login
            && u1?.password == u2?.password
            && u1?.role == u2?.role;

        public static bool operator !=(User u1, User u2) =>
            u1?.login != u2?.login
            || u1?.password != u2?.password
            || u1?.role != u2?.role;
    }
}
