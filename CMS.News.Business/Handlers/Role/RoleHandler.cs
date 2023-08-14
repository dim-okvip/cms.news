namespace CMS.News.Business.Handlers
{
    public class RoleHandler : IRoleHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleHandler> _logger;

        public RoleHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<List<RoleQueryResult>>> GetAsync(RoleQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<Role> repository = _unitOfWork.GetRepository<Role>();
                var allRoleQuery = from role in repository.GetAll() select role;

                if (filter.Id.HasValue)
                    allRoleQuery = from role in allRoleQuery where role.Id == filter.Id.Value select role;

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allRoleQuery = from role in allRoleQuery where role.Name.Contains(filter.TextSearch) select role;
                    totalCountByFilter = allRoleQuery.Count();
                }

                if (filter.OrderBy.HasValue)
                {
                    switch (filter.OrderBy)
                    {
                        case Order.CREATED_TIME_ASC:
                            allRoleQuery = allRoleQuery.OrderBy(x => x.CreatedTime);
                            break;
                        case Order.CREATED_TIME_DESC:
                            allRoleQuery = allRoleQuery.OrderByDescending(x => x.CreatedTime);
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

                    allRoleQuery = allRoleQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                List<Role> listRole = await allRoleQuery.ToListAsync();
                List<RoleQueryResult> listRoleQueryResult = new();

                if (filter.IsIncludeRight.HasValue && filter.IsIncludeRight.Value)
                {
                    foreach (var role in listRole)
                    {
                        RoleQueryResult roleQueryResult = _mapper.Map<RoleQueryResult>(role);
                        var listActionName = await (from ra in _unitOfWork.GetRepository<RoleRight>().GetAll()
                                                    join a in _unitOfWork.GetRepository<Right>().GetAll()
                                                    on ra.RightId equals a.Id
                                                    where ra.RoleId == role.Id
                                                    select a).ToListAsync();

                        if (listActionName is not null && listActionName.Count > 0)
                            listActionName.ForEach(action => roleQueryResult.ListRight.Add(_mapper.Map<RightQueryResult>(action)));

                        listRoleQueryResult.Add(roleQueryResult);
                    }
                }
                else
                {
                    listRoleQueryResult = _mapper.Map<List<RoleQueryResult>>(listRole);
                }

                int dataCount = listRoleQueryResult.Count;
                int totalCount = totalCountByFilter > 0 ? totalCountByFilter : repository.Count();

                if (dataCount > 0)
                    return new Response<List<RoleQueryResult>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu vai trò thành công", data: listRoleQueryResult, dataCount: dataCount, totalCount: totalCount);
                else
                    return new Response<List<RoleQueryResult>>(status: HttpStatusCode.NoContent, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: listRoleQueryResult, dataCount: dataCount, totalCount: totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<RoleQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<RoleQueryResult>());
            }
        }

        public async Task<Response<bool>> CreateAsync(UpsertRoleRequest request)
        {
            try
            {
                IRepository<Role> repository = _unitOfWork.GetRepository<Role>();
                bool isRoleExist = repository.Any(x => x.Name == request.Name);
                if (isRoleExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Vai trò với tên {request.Name} đã tồn tại", data: false);

                Role roleCreate = new Role();
                roleCreate.Id = Guid.NewGuid();
                roleCreate.Name = request.Name;
                roleCreate.Description = request.Description;
                roleCreate.CreatedTime = DateTime.Now;

                repository.Create(roleCreate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Thêm mới vai trò thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdateAsync(UpsertRoleRequest request)
        {
            try
            {
                IRepository<Role> repository = _unitOfWork.GetRepository<Role>();

                Role roleUpdate = repository.Get(x => x.Id == request.Id);
                if (roleUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Vai trò với id {request.Id} không tồn tại", data: false);

                if (roleUpdate.Name == Constants.ADMIN_ROLE_NAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: $"Không được phép cập nhật vai trò {Constants.ADMIN_ROLE_NAME}", data: false);

                bool isRoleExist = repository.Any(x => x.Id != request.Id && x.Name == request.Name);
                if (isRoleExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Vai trò với tên {request.Name} đã tồn tại", data: false);

                roleUpdate.Name = request.Name;
                roleUpdate.Description = request.Description;
                roleUpdate.UpdatedTime = DateTime.Now;

                repository.Update(roleUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật vai trò thành công", data: true);
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
                IRepository<Role> repository = _unitOfWork.GetRepository<Role>();

                Role roleDelete = repository.Get(x => x.Id == id);
                if (roleDelete is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Vai trò với id {id} không tồn tại", data: false);

                if (roleDelete.Name == Constants.ADMIN_ROLE_NAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: $"Không được phép xóa vai trò {Constants.ADMIN_ROLE_NAME}", data: false);

                repository.Delete(roleDelete);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Xóa vai trò thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }
    }
}
