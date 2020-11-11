using FluentValidation;
using Rapier.Server.Requests;

namespace Rapier.Server.Validation
{
    public class ModifyBlogValidation : AbstractValidator<ModifyBlogRequest>
    {
        public ModifyBlogValidation()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
