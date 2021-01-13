using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rapier.Descriptive;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.External.Models.Records;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rapier.Configuration
{
    public class SemanticsDefiner
    {
        //     public IReadOnlyDictionary<string, string> ActionNames { get; private set; }
        private IReadOnlyCollection<object> Actions { get; set; }
        private IReadOnlyCollection<object> Queries { get; set; }
        public SemanticsDefiner(
            IActionDescriptorCollectionProvider actionDescriptors,
            ActionIntermediary actionIntermediary,
            IDictionary<Type, IEnumerable<FieldDescription>> expandeableMembers)
        {
            ConfigureAction(actionDescriptors, actionIntermediary);
            ConfigureQuery(expandeableMembers);
        }

        private void ConfigureAction(
            IActionDescriptorCollectionProvider actionDescriptors,
            ActionIntermediary actionIntermediary)
        {
            var actionNames = new Dictionary<string, string>();
            var actions = new List<object>();

            foreach (var descriptor in actionDescriptors.ActionDescriptors.Items)
            {
                var controller = descriptor.RouteValues[Key.RouteValue.Controller];
                var entity = controller.Split('C')[0];
                var action = descriptor.RouteValues[Key.RouteValue.Action];

                Func<string> newActionName = action switch
                {
                    DefaultAction.Get => () => $"{action}{entity}s",
                    DefaultAction.GetById => () =>
                    {
                        var getById = action.Insert(3, ".").Split('.');
                        return $"{getById[0]}{entity}{getById[1]}";
                    }
                    ,
                    _ => () => $"{action}{entity}"
                };

                descriptor.RouteValues[Key.RouteValue.Action] = newActionName();
                actionNames.Add($"{controller}.{action}", newActionName());
            }

            foreach (var entityActionGroup in actionIntermediary.ActionDescriptions.GroupBy(x => x.ResponseType))
            {
                var actionKeys = entityActionGroup.AsEnumerable().Select(x => $"{x.Controller}.{x.Name}");
                var newActions = new Dictionary<string, string>();

                foreach (var actionKey in actionKeys)
                    if (actionNames.TryGetValue(actionKey, out var newAction))
                        newActions.Add(actionKey.Split('.')[1], newAction);

                actions.Add(ExpressionUtility.CreateConstructor(
                    typeof(Action<>).MakeGenericType(entityActionGroup.Key),
                    typeof(IDictionary<string, string>))(newActions));
            }

            //    ActionNames = new ReadOnlyDictionary<string, string>(actionNames);
            Actions = new ReadOnlyCollection<object>(actions);
        }

        private void ConfigureQuery(IDictionary<Type, IEnumerable<FieldDescription>> fieldDescriptions)
        {
            Queries = new ReadOnlyCollection<object>(fieldDescriptions.Select(x =>
            ExpressionUtility.CreateConstructor(
                    typeof(Query<>).MakeGenericType(typeof(PagedResponse<>).MakeGenericType(x.Key)),
                    typeof(IEnumerable<FieldDescription>))(x.Value)).ToList());
        }

        public Action<T> GetAction<T>()
            => Actions.FirstOrDefault(
                x => x.GetType().GenericTypeArguments[0] == typeof(T)) as Action<T>;

        public Query<T> GetQuery<T>()
            => Queries.FirstOrDefault(
                x => x.GetType().GenericTypeArguments[0] == typeof(T)) as Query<T>;

        public class Action<T>
        {
            public Action(IDictionary<string, string> actionNames)
                => Names = new ReadOnlyDictionary<string, string>(actionNames);
            public IReadOnlyDictionary<string, string> Names { get; }
        }

        public class Query<T>
        {
            public Query(IEnumerable<FieldDescription> members)
                => (DefaultFields, RelationalFields)
                = (new ReadOnlyCollection<string>(members
                       .Where(x => x.Category == FieldCategory.Default)
                       .Select(x => x.Name)
                       .ToList()),
                   new ReadOnlyCollection<string>(members
                       .Where(x => x.Category == FieldCategory.Relational)
                       .Select(x => x.Name)
                       .ToList()));
            public IReadOnlyCollection<string> DefaultFields { get; }
            public IReadOnlyCollection<string> RelationalFields { get; }
        }
    }
}
