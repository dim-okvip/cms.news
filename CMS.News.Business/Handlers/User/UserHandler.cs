using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace CMS.News.Business.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenLoginHandler _tokenLoginHandler;
        private readonly ILogger<UserHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserHandler(IUnitOfWork unitOfWork, IMapper mapper, ITokenLoginHandler tokenLoginHandler, ILogger<UserHandler> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenLoginHandler = tokenLoginHandler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<Response<UserLoginResult>> LoginAsync(UserLoginRequest user)
        {
            try
            {
                string md5Password = Utils.CreateMD5(user.Password);

                IQueryable<User> users = from u in _unitOfWork.GetRepository<User>().GetAll() where u.Username == user.Username && u.Password == md5Password select u;

                User? userLogin = await users.FirstOrDefaultAsync();
                if (userLogin is null)
                    return new Response<UserLoginResult>(status: HttpStatusCode.Forbidden, message: "Tên đăng nhập hoặc mật khẩu không chính xác", data: new UserLoginResult());

                if (userLogin.Status is false)
                    return new Response<UserLoginResult>(status: HttpStatusCode.Forbidden, message: "Tài khoản của bạn đã bị khóa, liên hệ admin để mở khóa", data: new UserLoginResult());

                UserLoginResult userLoginResult = _mapper.Map<UserLoginResult>(userLogin);

                var listUserRole = await(from ur in _unitOfWork.GetRepository<UserRole>().GetAll()
                                         join r in _unitOfWork.GetRepository<Role>().GetAll()
                                         on ur.RoleId equals r.Id
                                         where ur.UserId == userLogin.Id
                                         select new { RoleName = r.Name }).Distinct().ToListAsync();

                if (listUserRole is not null && listUserRole.Count > 0)
                    listUserRole.ForEach(x => userLoginResult.ListRole.Add(x.RoleName));

                return new Response<UserLoginResult>(HttpStatusCode.OK, message: "Đăng nhập thành công", data: userLoginResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<UserLoginResult>(HttpStatusCode.InternalServerError, message: ex.Message, data: new UserLoginResult());
            }
        }

        public async Task<Response<List<UserQueryResult>>> GetAllAsync(UserQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<User> repositoryUser = _unitOfWork.GetRepository<User>();
                IRepository<UserRole> repositoryUserRole = _unitOfWork.GetRepository<UserRole>();
                IRepository<Role> repositoryRole = _unitOfWork.GetRepository<Role>();

                var allUserQuery = from user in repositoryUser.GetAll() select user;

                totalCountByFilter = allUserQuery.Count();

                if (filter.Id.HasValue)
                    allUserQuery = from user in allUserQuery where user.Id == filter.Id.Value select user;

                if (filter.Status.HasValue)
                {
                    allUserQuery = from u in allUserQuery where u.Status == filter.Status.Value select u;
                    totalCountByFilter = allUserQuery.Count();
                }

                if (filter.IsAllowLoginMultiSession.HasValue)
                {
                    allUserQuery = from u in allUserQuery where u.IsAllowLoginMultiSession == filter.IsAllowLoginMultiSession.Value select u;
                    totalCountByFilter = allUserQuery.Count();
                }

                if (!String.IsNullOrEmpty(filter.RoleName))
                {
                    allUserQuery = from user in allUserQuery
                                   join ur in repositoryUserRole.GetAll()
                                   on user.Id equals ur.UserId
                                   join role in repositoryRole.GetAll()
                                   on ur.RoleId equals role.Id
                                   where role.Name == filter.RoleName
                                   select user;
                    totalCountByFilter = allUserQuery.Count();
                }

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allUserQuery = from u in allUserQuery
                                   where u.Username.Contains(filter.TextSearch) ||
                                         u.Email.Contains(filter.TextSearch) ||
                                         u.Fullname.Contains(filter.TextSearch) ||
                                         u.PhoneNumber.Contains(filter.TextSearch)
                                   select u;
                    totalCountByFilter = allUserQuery.Count();
                }

                if (filter.OrderBy.HasValue)
                {
                    switch (filter.OrderBy)
                    {
                        case Order.CREATED_TIME_ASC:
                            allUserQuery = allUserQuery.OrderBy(x => x.CreatedTime);
                            break;
                        case Order.CREATED_TIME_DESC:
                            allUserQuery = allUserQuery.OrderByDescending(x => x.CreatedTime);
                            break;
                        default:
                            break;
                    }
                }

                if (filter.PageSize.HasValue && filter.PageNumber.HasValue)
                {
                    if (filter.PageSize <= 0)
                        filter.PageSize = 10;

                    if (filter.PageNumber <= 0)
                        filter.PageNumber = 1;

                    int excludedRows = (filter.PageNumber.Value - 1) * (filter.PageSize.Value);
                    if (excludedRows <= 0)
                        excludedRows = 0;

                    allUserQuery = allUserQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                var listAllUserQuery = await allUserQuery.ToListAsync();
                List<UserQueryResult> listResult = new();

                if (filter.IsIncludeRole.HasValue && filter.IsIncludeRole.Value)
                {
                    foreach (var user in listAllUserQuery)
                    {
                        UserQueryResult userQueryResult = _mapper.Map<UserQueryResult>(user);
                        var listUserRole = await(from ur in repositoryUserRole.GetAll()
                                                 join role in repositoryRole.GetAll()
                                                 on ur.RoleId equals role.Id
                                                 where ur.UserId == user.Id
                                                 select new { RoleName = role.Name }).Distinct().ToListAsync();

                        if (listUserRole is not null && listUserRole.Count() > 0)
                            listUserRole.ForEach(item => { userQueryResult.ListRole.Add(item.RoleName); });

                        listResult.Add(userQueryResult);
                    }
                }
                else
                    listResult = _mapper.Map<List<UserQueryResult>>(listAllUserQuery);

                int dataCount = listResult.Count;

                if (dataCount > 0)
                    return new Response<List<UserQueryResult>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu người dùng thành công", data: listResult, dataCount: dataCount, totalCount: totalCountByFilter);
                else
                    return new Response<List<UserQueryResult>>(status: HttpStatusCode.NoContent, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: listResult, dataCount: dataCount, totalCount: totalCountByFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<UserQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<UserQueryResult>());
            }
        }

        public async Task<Response<bool>> CreateAsync(CreateUserRequest request)
        {
            IDbContextTransaction dbContextTransaction = _unitOfWork.BeginTransaction();
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();

                bool isUsernameExist = repository.Any(x => x.Username == request.Username);
                if (isUsernameExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Tên đăng nhập {request.Username} đã tồn tại trong hệ thống", data: false);

                User user = _mapper.Map<User>(request);
                user.Id = Guid.NewGuid();
                user.Password = Utils.CreateMD5(request.Password);

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                user.CreatedBy = logedInUser.Id;

                user.CreatedTime = DateTime.Now;
                repository.Create(user);

                request.ListRoleId.ForEach(roleId =>
                {
                    UserRole userRole = new UserRole();
                    userRole.UserId = user.Id;
                    userRole.RoleId = roleId;
                    userRole.SiteId = request.SiteId.Value;
                    _unitOfWork.GetRepository<UserRole>().Create(userRole);
                });
                await _unitOfWork.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                await dbContextTransaction.DisposeAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Thêm mới thành công", data: true);
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi khi thêm mới", data: false);
            }
        }

        public async Task<Response<bool>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();

                User userUpdate = repository.Get(x => x.Id == request.Id);
                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                bool isUsernameExist = repository.Any(x => x.Id != request.Id && x.Username == request.Username);
                if (isUsernameExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Username {request.Username} đã tồn tại trong hệ thống", data: false);

                if (userUpdate.Username.ToLower() != Constants.ADMIN_USERNAME.ToLower())
                    userUpdate.Username = request.Username;

                userUpdate.Email = request.Email;
                userUpdate.Fullname = request.Fullname;
                userUpdate.PhoneNumber = request.PhoneNumber;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                userUpdate.UpdatedBy = logedInUser.Id;

                userUpdate.UpdatedTime = DateTime.Now;

                repository.Update(userUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdateRoleAsync(UpdateRoleRequest request)
        {
            try
            {
                IRepository<User> repositoryUser = _unitOfWork.GetRepository<User>();
                IRepository<UserRole> repositoryUserRole = _unitOfWork.GetRepository<UserRole>();

                User userUpdate = repositoryUser.Get(x => x.Id == request.Id);
                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                if (userUpdate.Username == Constants.ADMIN_USERNAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: "Không được phép cập nhật vai trò tài khoản admin", data: false);

                repositoryUserRole.DeleteMany(x => x.UserId == request.Id);

                request.ListRoleId.ForEach(roleId =>
                {
                    UserRole userRole = new UserRole();
                    userRole.UserId = request.Id;
                    userRole.RoleId = roleId;
                    userRole.SiteId = request.SiteId.Value;
                    repositoryUserRole.Create(userRole);
                });
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật vai trò thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdateStatusAsync(UpdateStatusRequest request)
        {
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();
                User userUpdate = repository.Get(x => x.Id == request.Id);

                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                if (userUpdate.Username == Constants.ADMIN_USERNAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: "Không được phép cập nhật trạng thái tài khoản admin", data: false);

                userUpdate.Status = request.Status;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                userUpdate.UpdatedBy = logedInUser.Id;

                userUpdate.UpdatedTime = DateTime.Now;

                repository.Update(userUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật trạng thái thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdateStatusSessionLoginAsync(UpdateStatusRequest request)
        {
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();
                User userUpdate = repository.Get(x => x.Id == request.Id);

                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                userUpdate.IsAllowLoginMultiSession = request.Status;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                userUpdate.UpdatedBy = logedInUser.Id;

                userUpdate.UpdatedTime = DateTime.Now;

                repository.Update(userUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật trạng thái thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdatePasswordAsync(UpdatePasswordRequest request)
        {
            try
            {
                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);

                if (request.Id != logedInUser.Id)
                    return new Response<bool>(status: HttpStatusCode.BadRequest, message: $"Không thể cập nhật mật khẩu cho người dùng khác", data: false);

                IRepository<User> repository = _unitOfWork.GetRepository<User>();
                User userUpdate = repository.Get(x => x.Id == request.Id);

                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                if (Utils.CreateMD5(request.OldPassword).ToLower() != userUpdate.Password.ToLower())
                    return new Response<bool>(status: HttpStatusCode.BadRequest, message: "Mật khẩu cũ không chính xác", data: false);

                string newPassword = Utils.CreateMD5(request.NewPassword);
                if (newPassword.ToLower() == userUpdate.Password.ToLower())
                    return new Response<bool>(status: HttpStatusCode.BadRequest, message: "Mật khẩu mới phải khác mật khẩu cũ", data: false);

                userUpdate.Password = newPassword;
                userUpdate.UpdatedBy = logedInUser.Id;
                userUpdate.UpdatedTime = DateTime.Now;

                repository.Update(userUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật mật khẩu thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }


        public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();

                User userUpdate = repository.Get(x => x.Id == request.Id);
                if (userUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {request.Id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                userUpdate.Password = Utils.CreateMD5(request.NewPassword);

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                userUpdate.UpdatedBy = logedInUser.Id;

                userUpdate.UpdatedTime = DateTime.Now;

                repository.Update(userUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Đặt lại mật khẩu thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }


        public async Task<Response<bool>> DeleteAsync(Guid id)
        {
            try
            {
                IRepository<User> repository = _unitOfWork.GetRepository<User>();

                User userDelete = repository.Get(x => x.Id == id);
                if (userDelete is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {id} không tồn tại hoặc đã bị xóa trong hệ thống", data: false);

                if (userDelete.Username == Constants.ADMIN_USERNAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: $"Không được phép xóa tài khoản admin trong hệ thống", data: false);

                repository.Delete(userDelete);
                await _unitOfWork.SaveChangesAsync();

                // Xoá user TokenLogin trong database & memory
                await _tokenLoginHandler.DeleteAsync(id);

                return new Response<bool>(status: HttpStatusCode.OK, message: "Xóa thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

    }
}
