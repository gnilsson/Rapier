namespace Rapier.External.Models
{
    public interface IPaginateable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
