namespace CMS.News.DAL.Entities
{
    public class Right
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<RoleRight> RoleRights { get; set; }
    }

    public class RightName
    {
        public const string NOT_AUTHORIZATION = "NotAuthorization";
        public const string UPLOAD_FILE = "UploadFile";

        public const string ROLE_MANAGEMENT = "RoleManagement";
        public const string USER_MANAGEMENT = "UserManagement";
        public const string STORYLINE_MANAGEMENT = "StorylineManagement";
    }
}
