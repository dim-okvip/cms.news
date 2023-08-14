namespace CMS.News.Business.Handlers
{
    public class RoleRightHandler : IRoleRightHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleRightHandler> _logger;

        public RoleRightHandler(IUnitOfWork unitOfWork, ILogger<RoleRightHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<bool>> UpdateAsync(UpdateRoleRightRequest request)
        {
            try
            {
                IRepository<RoleRight> repository = _unitOfWork.GetRepository<RoleRight>();

                Role checkRole = _unitOfWork.GetRepository<Role>().Get(x => x.Id == request.RoleId);
                if (checkRole is null)
                    return new Response<bool>(status: HttpStatusCode.BadRequest, message: $"Mã vai trò {request.RoleId} không tồn tại", data: false);

                if (checkRole.Name == Constants.ADMIN_ROLE_NAME)
                    return new Response<bool>(status: HttpStatusCode.NotModified, message: $"Không thể cập nhật chức năng cho vai trò {Constants.ADMIN_ROLE_NAME}", data: false);

                repository.DeleteMany(x => x.RoleId == request.RoleId);

                List<RoleRight> listRoleAction = new();
                request.ListActionId.ForEach(x => listRoleAction.Add(new RoleRight() { RoleId = request.RoleId, RightId = x }));

                repository.CreateMany(listRoleAction);
                await _unitOfWork.SaveChangesAsync();

                return new Response<bool>(status: HttpStatusCode.OK, message: "Cập nhật thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: "Có lỗi xảy ra khi cập nhật", data: false);
            }
        }
    }
}
