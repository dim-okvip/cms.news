namespace CMS.News.DAL.Entities
{
    public class TokenLogin
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime UpsertedDate { get; set; }
    }
}
