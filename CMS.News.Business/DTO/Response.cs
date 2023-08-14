namespace CMS.News.Business.DTO
{
    public class Response
    {
        public HttpStatusCode Status { get; set; }

        public string Message { get; set; }

        public Response(HttpStatusCode status, string message = null)
        {
            Status = status;
            Message = message;
        }
    }

    public class Response<T> : Response
    {
        public T Data { get; set; }
        public int DataCount { get; set; }
        public int TotalCount { get; set; }

        public Response(HttpStatusCode status, string message = null, T data = default(T))
            : base(status, message)
        {
            Data = data;
            DataCount = 0;
            TotalCount = 0;
        }

        public Response(HttpStatusCode status, string message = null, T data = default(T), int dataCount = 0, int totalCount = 0)
            : base(status, message)
        {
            Data = data;
            DataCount = dataCount;
            TotalCount = totalCount;
        }
    }
}