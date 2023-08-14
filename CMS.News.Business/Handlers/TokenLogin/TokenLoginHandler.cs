namespace CMS.News.Business.Handlers
{
    public class TokenLoginHandler : ITokenLoginHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenLoginHandler> _logger;

        public static Dictionary<Guid, TokenLogin> DictionaryTokenLogin = new Dictionary<Guid, TokenLogin>();

        public TokenLoginHandler(IUnitOfWork unitOfWork, ILogger<TokenLoginHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<List<TokenLogin>>> GetAllFromDatabaseAsync()
        {
            try
            {
                IRepository<TokenLogin> repository = _unitOfWork.GetRepository<TokenLogin>();
                List<TokenLogin> listTokenLogin = await repository.GetAll().OrderByDescending(x => x.UpsertedDate).ToListAsync();

                return new Response<List<TokenLogin>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu token từ database thành công", data: listTokenLogin, dataCount: listTokenLogin.Count, totalCount: listTokenLogin.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<TokenLogin>>(HttpStatusCode.InternalServerError, message: ex.Message, data: new List<TokenLogin>());
            }
        }

        public Response<List<KeyValuePair<Guid, TokenLogin>>> GetAllFromMemory()
        {
            try
            {
                List<KeyValuePair<Guid, TokenLogin>> listTokenLogin = DictionaryTokenLogin.OrderByDescending(x => x.Value.UpsertedDate).ToList();
                return new Response<List<KeyValuePair<Guid, TokenLogin>>>(status: HttpStatusCode.OK, message: "Truy vấn dữ liệu token từ memory thành công", data: listTokenLogin, dataCount: listTokenLogin.Count, totalCount: listTokenLogin.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<List<KeyValuePair<Guid, TokenLogin>>>(HttpStatusCode.InternalServerError, message: ex.Message, data: new List<KeyValuePair<Guid, TokenLogin>>());
            }
        }

        public Response<bool> IsTokenExist(string token)
        {
            try
            {
                bool isTokenExist = true;
                TokenLogin? tokenLogin = DictionaryTokenLogin.Values.Where(x => x.Token == token).FirstOrDefault();

                if (tokenLogin is null)
                    isTokenExist = false;

                return new Response<bool>(status: HttpStatusCode.OK, message: String.Empty, data: isTokenExist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(status: HttpStatusCode.InternalServerError, message: String.Empty, data: false);
            }
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> CreateAsync(Guid userId, string token)
        {
            try
            {
                IRepository<TokenLogin> repository = _unitOfWork.GetRepository<TokenLogin>();

                TokenLogin tokenLoginDelete = repository.Get(x => x.UserId == userId);
                if (tokenLoginDelete is not null)
                    repository.Delete(tokenLoginDelete);

                TokenLogin tokenLoginCreate = new() { UserId = userId, Token = token, UpsertedDate = DateTime.Now };
                repository.Create(tokenLoginCreate);

                await _unitOfWork.SaveChangesAsync();

                #region Create in memory
                DictionaryTokenLogin[userId] = tokenLoginCreate;
                #endregion

                return new Response<bool>(HttpStatusCode.OK, message: "Thêm mới thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid userId)
        {
            try
            {
                IRepository<TokenLogin> repository = _unitOfWork.GetRepository<TokenLogin>();
                TokenLogin tokenLoginDelete = repository.Get(x => x.UserId == userId);

                if (tokenLoginDelete is null)
                    return new Response<bool>(HttpStatusCode.OK, message: "Dữ liệu không tồn tại hoặc đã bị xóa", data: false);

                repository.Delete(tokenLoginDelete);
                await _unitOfWork.SaveChangesAsync();

                #region Delete in memory
                if (DictionaryTokenLogin.ContainsKey(userId))
                    DictionaryTokenLogin.Remove(userId);
                #endregion

                return new Response<bool>(HttpStatusCode.OK, message: "Xoá thành công", data: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<bool>(HttpStatusCode.InternalServerError, message: ex.Message, data: false);
            }
        }

    }
}
