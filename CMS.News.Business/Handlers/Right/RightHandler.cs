namespace CMS.News.Business.Handlers
{
    public class RightHandler : IRightHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenLoginHandler _tokenLoginHandler;
        private readonly ILogger<RightHandler> _logger;

        public RightHandler(IUnitOfWork unitOfWork, IMapper mapper, ITokenLoginHandler tokenLoginHandler, ILogger<RightHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenLoginHandler = tokenLoginHandler;
            _logger = logger;
        }

        public async Task<Response<List<RightQueryResult>>> GetAsync(RightQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<Right> repository = _unitOfWork.GetRepository<Right>();
                var allRightQuery = from a in repository.GetAll() select a;

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allRightQuery = from a in allRightQuery where a.Name.Contains(filter.TextSearch) select a;
                    totalCountByFilter = allRightQuery.Count();
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

                    allRightQuery = allRightQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                List<Right> listAllRight = await allRightQuery.ToListAsync();
                List<RightQueryResult> listRightQueryResult = _mapper.Map<List<Right>, List<RightQueryResult>>(listAllRight);

                int dataCount = listRightQueryResult.Count;
                int totalCount = totalCountByFilter > 0 ? totalCountByFilter : repository.Count();

                return new Response<List<RightQueryResult>>(status: dataCount > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent, message: dataCount > 0 ? "Truy vấn dữ liệu quyền thành công" : "Dữ liệu không tồn tại hoặc đã bị xóa", data: listRightQueryResult, dataCount: dataCount, totalCount: totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<RightQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<RightQueryResult>());
            }
        }

        public async Task<Response<List<RightQueryResult>>> GetByRoleIdToAuthorizeAsync(string jwt, UserQueryResult logedInUser)
        {
            try
            {
                IRepository<User> repositoryUser = _unitOfWork.GetRepository<User>();

                // Người dùng có trạng thái active mới được phép thao tác hệ thống
                User user = repositoryUser.Get(x => x.Id == logedInUser.Id && x.Status == true);
                if (user is null)
                    return new Response<List<RightQueryResult>>(status: HttpStatusCode.NoContent, message: $"Người dùng với id {logedInUser.Id} không tồn tại", data: new List<RightQueryResult>());

                // Kiểm tra người dùng đang đăng nhập liệu có thể đăng nhập trên nhiều session 
                if (!user.IsAllowLoginMultiSession)
                {
                    bool isTokenValid = _tokenLoginHandler.IsTokenExist(jwt).Data;
                    if (!isTokenValid)
                        return new Response<List<RightQueryResult>>(status: HttpStatusCode.Forbidden, message: $"Vui lòng đăng nhập lại vào hệ thống", data: new List<RightQueryResult>());
                }

                List<UserRole> listlUserRoleQuery = await(from u in _unitOfWork.GetRepository<UserRole>().GetAll()
                                                          where u.UserId == logedInUser.Id
                                                          select u).ToListAsync();

                if (listlUserRoleQuery.Count < 1)
                    return new Response<List<RightQueryResult>>(status: HttpStatusCode.NoContent, message: $"Người dùng {logedInUser.Username} chưa được gán bất kì vai trò nào", data: new List<RightQueryResult>());

                List<Guid> listRoleId = new List<Guid>();
                listlUserRoleQuery.ForEach(x => listRoleId.Add(x.RoleId));

                var query = (from a in _unitOfWork.GetRepository<Right>().GetAll()
                             join ra in _unitOfWork.GetRepository<RoleRight>().GetAll()
                             on a.Id equals ra.RightId
                             where listRoleId.Contains(ra.RoleId)
                             select a).Distinct();

                List<Right> data = await query.ToListAsync();
                List<RightQueryResult> listActionDTO = _mapper.Map<List<Right>, List<RightQueryResult>>(data);

                return new Response<List<RightQueryResult>>(status: HttpStatusCode.OK, message: "OK", data: listActionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<RightQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<RightQueryResult>());
            }
        }
    }
}
