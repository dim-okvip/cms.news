namespace CMS.News.Business.Handlers
{
    public class MenuHandler : IMenuHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MenuHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<List<MenuQueryResult>>> GetAsync(MenuQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<Menu> repository = _unitOfWork.GetRepository<Menu>();
                var allMenuQuery = from s in repository.GetAll() select s;

                if (filter.Id.HasValue)
                {
                    allMenuQuery = from s in allMenuQuery where s.Id == filter.Id.Value select s;
                    totalCountByFilter = allMenuQuery.Count();
                }
                
                if (filter.Status.HasValue)
                {
                    allMenuQuery = from s in allMenuQuery where s.Status == filter.Status.Value select s;
                    totalCountByFilter = allMenuQuery.Count();
                }

                if (filter.SiteId.HasValue)
                {
                    allMenuQuery = from s in allMenuQuery where s.SiteId == filter.SiteId.Value select s;
                    totalCountByFilter = allMenuQuery.Count();
                }

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allMenuQuery = from s in allMenuQuery where s.Name.Contains(filter.TextSearch) select s;
                    totalCountByFilter = allMenuQuery.Count();
                }

                if (filter.OrderBy.HasValue)
                {
                    switch (filter.OrderBy)
                    {
                        case Order.CREATED_TIME_ASC:
                            allMenuQuery = allMenuQuery.OrderBy(x => x.CreatedTime);
                            break;
                        case Order.CREATED_TIME_DESC:
                            allMenuQuery = allMenuQuery.OrderByDescending(x => x.CreatedTime);
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

                    allMenuQuery = allMenuQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                List<Menu> listMenu = await allMenuQuery.ToListAsync();
                List<MenuQueryResult> listMenuQueryResult = _mapper.Map<List<MenuQueryResult>>(listMenu);

                int dataCount = listMenuQueryResult.Count;
                int totalCount = totalCountByFilter > 0 ? totalCountByFilter : repository.Count();

                if (dataCount > 0)
                    return new Response<List<MenuQueryResult>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu menu thành công", data: listMenuQueryResult, dataCount: dataCount, totalCount: totalCount);
                else
                    return new Response<List<MenuQueryResult>>(status: HttpStatusCode.NoContent, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: listMenuQueryResult, dataCount: dataCount, totalCount: totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<MenuQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<MenuQueryResult>());
            }
        }

        public async Task<Response<bool>> CreateAsync(CreateMenuRequest request)
        {
            try
            {
                IRepository<Menu> repository = _unitOfWork.GetRepository<Menu>();
                bool isMenuExist = repository.Any(x => x.SiteId == request.SiteId && x.Name == request.Name);
                if (isMenuExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Menu với tên {request.Name} đã tồn tại", data: false);

                Menu menuCreate = new();
                menuCreate.Id = Guid.NewGuid();
                menuCreate.Name = request.Name;
                menuCreate.Description = request.Description;
                menuCreate.Status = request.Status;
                menuCreate.SiteId = request.SiteId.Value;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                menuCreate.CreatedBy = logedInUser.Id;
                menuCreate.CreatedTime = DateTime.Now;

                repository.Create(menuCreate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Thêm mới thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi khi thêm mới", data: false);
            }
        }

        public async Task<Response<bool>> UpdateAsync(UpdateMenuRequest request)
        {
            try
            {
                IRepository<Menu> repository = _unitOfWork.GetRepository<Menu>();
                Menu menuUpdate = repository.Get(x => x.Id == request.Id);
                if (menuUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu với id {request.Id} không tồn tại", data: false);
                
                bool isMenuExist = repository.Any(x => x.SiteId == request.SiteId && x.Id != request.Id && x.Name == request.Name);
                if (isMenuExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Menu với tên {request.Name} đã tồn tại", data: false);

                menuUpdate.Name = request.Name;
                menuUpdate.Description = request.Description;
                menuUpdate.Status = request.Status;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                menuUpdate.UpdatedBy = logedInUser.Id;
                menuUpdate.UpdatedTime = DateTime.Now;

                repository.Update(menuUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi khi cập nhật", data: false);
            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid id)
        {
            try
            {
                IRepository<Menu> repository = _unitOfWork.GetRepository<Menu>();
                Menu menuDelete = repository.Get(x => x.Id == id);
                if (menuDelete is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu với id {id} không tồn tại", data: false);

                repository.Delete(menuDelete);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Xóa thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi khi xóa", data: false);
            }
        }
    }
}
