using CMS.News.DAL.Infrastructure;

namespace CMS.News.Business.Handlers
{
    public class MenuItemHandler : IMenuItemHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MenuItemHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuItemHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuItemHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<List<MenuItemQueryResult>>> GetAsync(MenuItemQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<MenuItem> repository = _unitOfWork.GetRepository<MenuItem>();
                var allMenuItemQuery = from s in repository.GetAll() select s;

                if (filter.Id.HasValue)
                {
                    allMenuItemQuery = from s in allMenuItemQuery where s.Id == filter.Id.Value select s;
                    totalCountByFilter = allMenuItemQuery.Count();
                }

                if (filter.Status.HasValue)
                {
                    allMenuItemQuery = from s in allMenuItemQuery where s.Status == filter.Status.Value select s;
                    totalCountByFilter = allMenuItemQuery.Count();
                }
                
                if (filter.MenuId.HasValue)
                {
                    allMenuItemQuery = from s in allMenuItemQuery where s.MenuId == filter.MenuId.Value select s;
                    totalCountByFilter = allMenuItemQuery.Count();
                }

                if (filter.ParentId.HasValue)
                {
                    allMenuItemQuery = from s in allMenuItemQuery where s.ParentId == filter.ParentId.Value select s;
                    totalCountByFilter = allMenuItemQuery.Count();
                }

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allMenuItemQuery = from s in allMenuItemQuery where s.Name.Contains(filter.TextSearch) || s.Title.Contains(filter.TextSearch) || s.Url.Contains(filter.TextSearch) select s;
                    totalCountByFilter = allMenuItemQuery.Count();
                }

                if (filter.OrderBy.HasValue)
                {
                    switch (filter.OrderBy)
                    {
                        case Order.CREATED_TIME_ASC:
                            allMenuItemQuery = allMenuItemQuery.OrderBy(x => x.CreatedTime);
                            break;
                        case Order.CREATED_TIME_DESC:
                            allMenuItemQuery = allMenuItemQuery.OrderByDescending(x => x.CreatedTime);
                            break;
                        case Order.ORDER_ASC:
                            allMenuItemQuery = allMenuItemQuery.OrderBy(x => x.Order);
                            break;
                        case Order.ORDER_DESC:
                            allMenuItemQuery = allMenuItemQuery.OrderByDescending(x => x.Order);
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

                    allMenuItemQuery = allMenuItemQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                List<MenuItem> listMenuItem = await allMenuItemQuery.ToListAsync();
                List<MenuItemQueryResult> listMenuItemQueryResult = _mapper.Map<List<MenuItemQueryResult>>(listMenuItem);

                if (filter.IsIncludeChildMenuItem.HasValue && filter.IsIncludeChildMenuItem.Value)
                    listMenuItemQueryResult = RecursiveMenuItem(listMenuItemQueryResult, repository, filter.OrderBy);

                int dataCount = listMenuItemQueryResult.Count;
                int totalCount = totalCountByFilter > 0 ? totalCountByFilter : repository.Count();

                if (dataCount > 0)
                    return new Response<List<MenuItemQueryResult>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu menu item thành công", data: listMenuItemQueryResult, dataCount: dataCount, totalCount: totalCount);
                else
                    return new Response<List<MenuItemQueryResult>>(status: HttpStatusCode.NoContent, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: listMenuItemQueryResult, dataCount: dataCount, totalCount: totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<MenuItemQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<MenuItemQueryResult>());
            }
        }

        private List<MenuItemQueryResult> RecursiveMenuItem(List<MenuItemQueryResult> list, IRepository<MenuItem> repository, Order? orderBy)
        {
            foreach (var item in list)
            {
                List<MenuItem> menuItems = repository.GetMany(x => x.ParentId == item.Id).ToList();
                if (menuItems.Count > 0)
                {
                    item.ListChildMenuItem = _mapper.Map<List<MenuItemQueryResult>>(menuItems);
                    if (orderBy.HasValue)
                    {
                        switch (orderBy)
                        {
                            case Order.CREATED_TIME_ASC:
                                item.ListChildMenuItem = item.ListChildMenuItem.OrderBy(x => x.CreatedTime).ToList();
                                break;
                            case Order.CREATED_TIME_DESC:
                                item.ListChildMenuItem = item.ListChildMenuItem.OrderByDescending(x => x.CreatedTime).ToList();
                                break;
                            case Order.ORDER_ASC:
                                item.ListChildMenuItem = item.ListChildMenuItem.OrderBy(x => x.Order).ToList();
                                break;
                            case Order.ORDER_DESC:
                                item.ListChildMenuItem = item.ListChildMenuItem.OrderByDescending(x => x.Order).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    RecursiveMenuItem(item.ListChildMenuItem, repository, orderBy);
                }
            }
            return list;
        }

        public async Task<Response<bool>> CreateAsync(CreateMenuItemRequest request)
        {
            try
            {
                IRepository<MenuItem> repository = _unitOfWork.GetRepository<MenuItem>();
                bool isMenuItemExist = repository.Any(x => x.MenuId == request.MenuId && x.Name == request.Name);
                if (isMenuItemExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Menu item với tên {request.Name} đã tồn tại", data: false);

                if (request.ParentId.HasValue && request.ParentId.Value != Guid.Empty)
                {
                    bool isParentExist = repository.Any(x => x.Id == request.ParentId.Value);
                    if (!isParentExist)
                       return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu item cha với id {request.ParentId} không tồn tại", data: false);
                }

                MenuItem menuItemCreation = _mapper.Map<MenuItem>(request);
                menuItemCreation.Id = Guid.NewGuid();

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                menuItemCreation.CreatedBy = logedInUser.Id;
                menuItemCreation.CreatedTime = DateTime.Now;

                repository.Create(menuItemCreation);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Thêm mới thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi khi thêm mới", data: false);
            }
        }

        public async Task<Response<bool>> UpdateAsync(UpdateMenuItemRequest request)
        {
            try
            {
                IRepository<MenuItem> repository = _unitOfWork.GetRepository<MenuItem>();
                MenuItem menuItemUpdate = repository.Get(x => x.Id == request.Id);
                if (menuItemUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu item với id {request.Id} không tồn tại", data: false);

                bool isMenuItemExist = repository.Any(x => x.MenuId == request.MenuId && x.Id != request.Id && x.Name == request.Name);
                if (isMenuItemExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Menu item với tên {request.Name} đã tồn tại", data: false);

                if (request.ParentId.HasValue && request.ParentId.Value != Guid.Empty)
                {
                    bool isParentExist = repository.Any(x => x.Id == request.ParentId.Value);
                    if (!isParentExist)
                        return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu item cha với id {request.ParentId} không tồn tại", data: false);
                }

                menuItemUpdate.Name = request.Name;
                menuItemUpdate.Title = request.Title;
                menuItemUpdate.Description = request.Description;
                menuItemUpdate.Url = request.Url;
                menuItemUpdate.Target = request.Target.Value;
                menuItemUpdate.FileUrl = request.FileUrl;
                menuItemUpdate.Order = request.Order;
                menuItemUpdate.Status = request.Status.Value;
                menuItemUpdate.ParentId = request.ParentId.Value;
                menuItemUpdate.MenuId = request.MenuId.Value;

                var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE).Value);
                menuItemUpdate.UpdatedBy = logedInUser.Id;
                menuItemUpdate.UpdatedTime = DateTime.Now;

                repository.Update(menuItemUpdate);
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
                IRepository<MenuItem> repository = _unitOfWork.GetRepository<MenuItem>();
                MenuItem menuItemDeletion = repository.Get(x => x.Id == id);  
                if (menuItemDeletion is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Menu item với id {id} không tồn tại", data: false);

                repository.Delete(menuItemDeletion);
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
