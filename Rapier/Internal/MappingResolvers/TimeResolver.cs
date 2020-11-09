using AutoMapper;
using System;

namespace Rapier.Internal.MappingResolvers
{
    //public class TimeResolver : IValueResolver<DateTime, EntityResponse, string>
    //{
    //    public string Resolve(DateTime source, EntityResponse destination, string destMember, ResolutionContext context)
    //    {
    //        return source.ToString("dddd, dd MMMM yyyy HH:mm:ss");
    //    }
    //}

    public class TimeResolver : IValueResolver<object, object, object>
    {
        public object Resolve(
            object source,
            object destination,
            object destMember,
            ResolutionContext context)
           => (
            (DateTime)source).ToString("dddd, dd MMMM yyyy");
    }
}
