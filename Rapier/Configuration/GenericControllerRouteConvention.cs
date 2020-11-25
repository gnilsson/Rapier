using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rapier.Configuration.Settings;
using Rapier.Descriptive;
using Rapier.External.Enums;
using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration
{
    public class ActionConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
        }
    }

    public class ParameterConvention : IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
        }
    }

    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        private readonly IEnumerable<IEntitySettings> _settings;
        private ActionIntermediary _actionIntermediary;

        public GenericControllerRouteConvention(IEnumerable<IEntitySettings> settings, ActionIntermediary actionIntermediary)
            => (_settings, _actionIntermediary) = (settings, actionIntermediary);
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType)
                return;

            var setting = _settings.FirstOrDefault(
                x => x.ResponseType == controller.ControllerType.GenericTypeArguments[0]);

            if (string.IsNullOrWhiteSpace(setting.ControllerRoute))
                return;

            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel { Template = setting.ControllerRoute },
            });
            controller.ControllerName = $"{setting.EntityType.Name}Controller";

            foreach (var action in controller.Actions)
            {
                if (setting.AuthorizeableEndpoints.TryGetValue(
                    $"{controller.ControllerType.FullName}.{action.ActionMethod.Name}", out var endpoint))
                    if (endpoint.Category != AuthorizationCategory.None)
                        action.Filters.Add(
                            new AuthorizeFilter(
                                endpoint.Category == AuthorizationCategory.Custom ?
                                endpoint.Policy : string.Empty));

                action.Filters.Add(action.ActionName switch
                {
                    DefaultActions.Get => new ProducesResponseTypeAttribute(typeof(PagedResponse<>)
                            .MakeGenericType(setting.ResponseType), 200),
                    DefaultActions.Create => new ProducesResponseTypeAttribute(setting.ResponseType, 201),
                    DefaultActions.Delete => new ProducesResponseTypeAttribute(typeof(DeleteResponse), 200),
                    _ => new ProducesResponseTypeAttribute(setting.ResponseType, 200)
                });
                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(NotFoundResult), 404));
                _actionIntermediary.ActionDescriptions.Add(
                    new(setting.ResponseType, action.ActionName, controller.ControllerName));
            };
        }
    }
}

