﻿using AutoMapper;
using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System.Linq;

namespace Rapier.Configuration
{
    public class Mapping
    {
        private readonly EntitySettingsContainer _settings;
        public Mapping(EntitySettingsContainer settings) => _settings = settings;
        public IMapper ConfigureMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;

                cfg.CreateMap<IEntity, EntityResponse>()
                .IncludeAllDerived()
                .ForMember(
                    x => x.CreatedDate,
                    o => o.MapFrom(x => x.CreatedDate.ToLongDateTimeString()))
                .ForMember(
                    x => x.UpdatedDate,
                    o => o.MapFrom(x => x.UpdatedDate.ToLongDateTimeString()));

                foreach (var setting in _settings)
                {
                    cfg.CreateMap(setting.EntityType, setting.ResponseType)
                       .ForMembersExplicitExpansion(setting);

                    if (setting.ResponseType.ParentHasInterface(typeof(ISimplified), out var parent))
                        cfg.CreateMap(setting.EntityType, parent).As(setting.ResponseType);
                }
            });

            return config.CreateMapper();
        }
    }

    public static class MappingExtensions
    {
        public static IMappingExpression ForMembersExplicitExpansion(
            this IMappingExpression map, IEntitySettings setting)
        {
            var relational = setting.FieldDescriptions.Value
                .Where(x => x.Category == FieldCategory.Relational);

            if (!setting.AutoExpandMembers)
                foreach (var expand in relational)
                    map = map.ForMember(expand.Name, x => x.ExplicitExpansion());

            return map;
        }
    }
}
