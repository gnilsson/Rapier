using System.Collections.Generic;

namespace Rapier.External.Models
{
    public class PagedResponse<T>
    {
        public PagedResponse()
        { }

        public PagedResponse(IEnumerable<T> data)
        {
            Data = data;
        }

        public IEnumerable<T> Data { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public string NextPage { get; set; }

        public string PreviousPage { get; set; }

        public int? Total { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
