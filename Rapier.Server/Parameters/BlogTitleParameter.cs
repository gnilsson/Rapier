using Rapier.External;
using Rapier.QueryDefinitions.Parameters;
using Rapier.Server.Data;
using Rapier.Server.Requests;

namespace Rapier.Server.Parameters
{
    [QueryParameter(nameof(Blog), nameof(GetBlogsRequest.Title))]
    public class BlogTitleParameter : StringParameter
    {
        public BlogTitleParameter(string value)
        {
            base.Set(value);          
            TableReferenceChildren = new[] { nameof(Blog.Title) };
        }
    }
}
