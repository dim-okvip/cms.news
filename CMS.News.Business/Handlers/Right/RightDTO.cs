namespace CMS.News.Business.Handlers
{
    public class RightQueryResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RightQueryFilterRequest : QueryFilterRequest
    {
    }
}
