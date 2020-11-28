using System;
using System.Collections.Generic;

namespace Rapier.External.Models
{
    public interface ICommand
    {
        public string[] IgnoredProperties { get; set; }
        public IDictionary<string, object> RequestPropertyValues { get; }
        public Guid Id { get; }
        public string IncludeNavigation { get; set; }
        public IDictionary<string, Type> RequestForeignEntities { get; }
    }
}
