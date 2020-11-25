using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rapier.Descriptive;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration
{
    public class SemanticsDefiner
    {
        public IDictionary<string, string> ActionNames { get; }
        private ICollection<object> Actions { get; }
        public SemanticsDefiner(
            IActionDescriptorCollectionProvider actionDescriptors,
            ActionIntermediary actionIntermediary)
        {
            ActionNames = new Dictionary<string, string>();
            Actions = new List<object>();
            foreach (var descriptor in actionDescriptors.ActionDescriptors.Items)
            {
                var controller = descriptor.RouteValues["controller"];
                var entity = controller.Split('C')[0];
                var action = descriptor.RouteValues["action"];

                Func<string> newAction = action switch
                {
                    DefaultActions.Get => () => $"{action}{entity}s",
                    DefaultActions.GetById => () =>
                    {
                        var getById = action.Insert(3, ".").Split('.');
                        return $"{getById[0]}{entity}{getById[1]}";
                    },
                    _ => () => $"{action}{entity}"
                };

                ActionNames.Add($"{controller}.{action}", newAction());
            }

            foreach (var entityActionGroup in actionIntermediary.ActionDescriptions.GroupBy(x => x.ResponseType))
            {
                var actionKeys = entityActionGroup.AsEnumerable().Select(x => $"{x.Controller}.{x.Name}");
                var newActions = new Dictionary<string, string>();

                foreach (var actionKey in actionKeys)
                    if (ActionNames.TryGetValue(actionKey, out var newAction))
                        newActions.Add(actionKey.Split('.')[1], newAction);

                Actions.Add(ExpressionUtility.CreateConstructor(
                    typeof(Action<>).MakeGenericType(entityActionGroup.Key),
                    typeof(IDictionary<string, string>))(newActions));
            }
        }

        public Action<T> GetAction<T>()
            => Actions.FirstOrDefault(
                x => x.GetType().GenericTypeArguments[0] == typeof(T)) as Action<T>;

        public class Action<T>
        {
            public Action(IDictionary<string, string> actionNames)
            {
                foreach (var property in this.GetType().GetProperties())
                    if (actionNames.TryGetValue(property.Name, out var newAction))
                        property.SetValue(this, newAction);
            }
            public string Get { get; internal set; }
            public string Create { get; internal set; }
            public string GetById { get; internal set; }
            public string Update { get; internal set; }
            public string Delete { get; internal set; }
        }
    }
}
