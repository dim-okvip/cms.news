namespace CMS.News.Business.Handlers
{
    public class UpdateRoleRightRequest
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public List<Guid> ListActionId { get; set; }
    }
}
