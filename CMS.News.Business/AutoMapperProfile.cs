namespace CMS.News.Business
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserLoginResult>();
            CreateMap<User, UserQueryResult>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateProfileRequest, User>();

            CreateMap<Right, RightQueryResult>();

            CreateMap<Role, RoleQueryResult>();
        }
    }
}
