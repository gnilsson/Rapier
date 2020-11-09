using Rapier.External.Models;

namespace Rapier.QueryDefinitions
{
    public class PaginationQuery : IPaginateable
    {
        public PaginationQuery()
        { }

        public PaginationQuery(IPaginateable pagination)
        {
            PageNumber =
                pagination?.PageNumber < 1 ? _defaultPageNumber
                : pagination.PageNumber;

            PageSize =
                pagination.PageSize < 1 ? _defaultPageSize
                : pagination.PageSize > _maxPageSize ? _maxPageSize
                : pagination.PageSize;

            IsApplied =
                !(pagination.PageSize < 1 && pagination.PageNumber < 1);
        }

        private readonly int _defaultPageNumber = 1;
        private readonly int _defaultPageSize = 20;
        private readonly int _maxPageSize = 50;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool IsApplied { get; }

        public bool ExcludeTotal { get; set; }
    }
}
