namespace CMS.News.Business.Handlers
{
    public class SiteHandler : ISiteHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleHandler> _logger;

        public SiteHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<List<SiteQueryResult>>> GetAsync(SiteQueryFilterRequest filter)
        {
            try
            {
                int totalCountByFilter = 0;

                IRepository<Site> repository = _unitOfWork.GetRepository<Site>();
                var allSiteQuery = from s in repository.GetAll() select s;

                if (filter.Id.HasValue)
                    allSiteQuery = from s in allSiteQuery where s.Id == filter.Id.Value select s;

                if (!String.IsNullOrEmpty(filter.TextSearch))
                {
                    allSiteQuery = from s in allSiteQuery where s.Name.Contains(filter.TextSearch) || s.Address.Contains(filter.TextSearch) select s;
                    totalCountByFilter = allSiteQuery.Count();
                }

                if (filter.OrderBy.HasValue)
                {
                    switch (filter.OrderBy)
                    {
                        case Order.CREATED_TIME_ASC:
                            allSiteQuery = allSiteQuery.OrderBy(x => x.CreatedTime);
                            break;
                        case Order.CREATED_TIME_DESC:
                            allSiteQuery = allSiteQuery.OrderByDescending(x => x.CreatedTime);
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

                    allSiteQuery = allSiteQuery.Skip(excludedRows).Take(filter.PageSize.Value);
                }

                List<Site> listSite = await allSiteQuery.ToListAsync();
                List<SiteQueryResult> listSiteQueryResult = _mapper.Map<List<SiteQueryResult>>(listSite);

                int dataCount = listSiteQueryResult.Count;
                int totalCount = totalCountByFilter > 0 ? totalCountByFilter : repository.Count();

                if (dataCount > 0)
                    return new Response<List<SiteQueryResult>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu site thành công", data: listSiteQueryResult, dataCount: dataCount, totalCount: totalCount);
                else
                    return new Response<List<SiteQueryResult>>(status: HttpStatusCode.NoContent, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: listSiteQueryResult, dataCount: dataCount, totalCount: totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<SiteQueryResult>>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: new List<SiteQueryResult>());
            }
        }

        public async Task<Response<bool>> CreateAsync(UpsertSiteRequest request)
        {
            try
            {
                IRepository<Site> repository = _unitOfWork.GetRepository<Site>();
                bool isRoleExist = repository.Any(x => x.Name == request.Name);
                if (isRoleExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Site với tên {request.Name} đã tồn tại", data: false);

                Site siteCreate = new Site();
                siteCreate.Id = Guid.NewGuid();
                siteCreate.Name = request.Name;
                siteCreate.Address = request.Address;
                siteCreate.CreatedTime = DateTime.Now;

                repository.Create(siteCreate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Thêm mới site thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> UpdateAsync(UpsertSiteRequest request)
        {
            try
            {
                IRepository<Site> repository = _unitOfWork.GetRepository<Site>();
                Site siteUpdate = repository.Get(x => x.Id == request.Id);
                if (siteUpdate is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Site với id {request.Id} không tồn tại trong hệ thống", data: false);

                bool isSiteExist = repository.Any(x => x.Id != request.Id && x.Name == request.Name);
                if (isSiteExist)
                    return new Response<bool>(status: HttpStatusCode.Conflict, message: $"Site với tên {request.Name} đã tồn tại", data: false);

                siteUpdate.Name = request.Name;
                siteUpdate.Address = request.Address;
                siteUpdate.UpdatedTime = DateTime.Now;

                repository.Update(siteUpdate);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật site thành công", data: true);
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
                IRepository<Site> repository = _unitOfWork.GetRepository<Site>();
                Site siteDelete = repository.Get(x => x.Id == id);
                if (siteDelete is null)
                    return new Response<bool>(status: HttpStatusCode.NoContent, message: $"Site với id {id} không tồn tại trong hệ thống", data: false);
                
                repository.Delete(siteDelete);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Xóa site thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }
    }
}
