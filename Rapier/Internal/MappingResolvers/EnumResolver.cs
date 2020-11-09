using AutoMapper;
using System;

namespace Rapier.Internal.MappingResolvers
{
    public class EnumResolver : IValueResolver<object, object, object>
    {
        public object Resolve(
            object source,
            object destination,
            object destMember,
            ResolutionContext context)
           => ((Enum)source).ToString();
    }
}
