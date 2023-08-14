namespace CMS.News.Business.JwtAuthentication
{
    public class JwtAuthResponse
    {
        public string? Token { get; set; }
        public UserLoginResult LogedInUser { get; set; }
        public DateTime Expired { get; set; }
    }
}
