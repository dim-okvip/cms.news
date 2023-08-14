namespace CMS.News.Business.Handlers
{
    public class UserQueryResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
        public bool IsAllowLoginMultiSession { get; set; }
        public List<string> ListRole { get; set; } = new();
        public Guid CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserLoginResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
        public bool IsAllowLoginMultiSession { get; set; }
        public List<string> ListRole { get; set; } = new();
    }

    public class UserQueryFilterRequest : QueryFilterRequest
    {
        public Guid? SiteId { get; set; } = null;
        public string SiteName { get; set; } = String.Empty;
        public string RoleName { get; set; } = String.Empty;
        public bool? IsIncludeRole { get; set; } = null;
        public bool? IsAllowLoginMultiSession { get; set; } = null;
    }

    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Số điện thoại tối thiểu 10 chữ số")]
        public string PhoneNumber { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public Guid? SiteId { get; set; }

        [Required]
        public List<Guid> ListRoleId { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public bool IsAllowLoginMultiSession { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Số điện thoại tối thiểu 10 chữ số")]
        public string PhoneNumber { get; set; }
    }

    public class UpdateRoleRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid? SiteId { get; set; }

        [Required]
        public List<Guid> ListRoleId { get; set; }
    }

    public class UpdateStatusRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool Status { get; set; }
    }

    public class UpdatePasswordRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
