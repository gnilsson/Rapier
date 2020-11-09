using FluentValidation;
using Rapier.CommandDefinitions;
using Rapier.Server.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
