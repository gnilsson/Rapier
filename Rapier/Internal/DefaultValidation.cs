using FluentValidation;
using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapier.Internal
{
    public class DefaultValidation<T> : AbstractValidator<T> // where T : ICommand
    {
        public DefaultValidation()
        {
            //   RuleForEach(x => x.RequestPropertyValues).NotNull();
            //var a = typeof(T).GetProperties();
            //for (int i = 0; i < a.Length; i++)
            //{
            //    RuleFor(x => a.GetValue(i)).NotNull();
            //}
            //RuleForEach(x => a.GetValue(x.)).NotNull();
        }
    }
}
