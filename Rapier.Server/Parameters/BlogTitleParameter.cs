using Rapier.External;
using Rapier.QueryDefinitions.Parameters;
using Rapier.Server.Data;
using Rapier.Server.Requests;

namespace Rapier.Server.Parameters
{
    // [QueryParameter(string.Join(".",(nameof(Blog),nameof(GetBlogsRequest.Title)))]
 //   [QueryParameter(nameof(Post), nameof(GetBlogsRequest.Title))]
    [QueryParameter(nameof(Blog), nameof(GetBlogsRequest.Title))]
    public class BlogTitleParameter : StringParameter
    {
        public BlogTitleParameter(string value)
        {
            base.Set(value);
            TableReferenceChildren = new[] { nameof(Blog.Title) };
        }
        public override void Set(string value)
        {
            base.Set(value);
            TableReferenceChildren = new[] { nameof(Blog.Title) };
        }
    }
}
