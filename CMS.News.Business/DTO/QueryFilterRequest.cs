namespace CMS.News.Business.DTO
{
    public class QueryFilterRequest
    {
        public Guid? Id { get; set; } = null;
        public int? PageSize { get; set; } = null;
        public int? PageNumber { get; set; } = null;
        public string TextSearch { get; set; } = String.Empty;
        public bool? Status { get; set; } = null;
        public Order? OrderBy { get; set; } = null;
    }

    public enum Order
    {
        CREATED_TIME_ASC,
        CREATED_TIME_DESC
    }

    public enum DataSource
    {
        Database,
        Memory
    }
}