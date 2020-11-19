﻿using AutoMapper.Internal;
using Rapier.External;
using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rapier.Internal
{
    using static Expression;
    public static class ExpressionFactory
    {
        private static class ModifierImplementation<TEntity>
        {
            private static readonly MemberInfo _idMember;
            private static readonly MemberInfo _createdMember;
            private static readonly MemberInfo _updatedMember;
            static ModifierImplementation()
            {
                _idMember = typeof(TEntity).GetMember(nameof(IEntity.Id))[0];
                _createdMember = typeof(TEntity).GetMember(nameof(IEntity.CreatedDate))[0];
                _updatedMember = typeof(TEntity).GetMember(nameof(IEntity.UpdatedDate))[0];
            }
            internal static List<MemberAssignment> CreateParameters { get; set; }
            internal static List<Expression> UpdateParameters { get; set; }
            internal static ParameterExpression UpdateEntityParameter { get; set; }
            internal static class CreateImplementation
            {
                internal static Func<TEntity> Create { get; }
                static CreateImplementation()
                {
                    var now = Constant(DateTime.UtcNow);
                    CreateParameters.AddRange(
                        new[]
                        {
                            Bind(_idMember, Constant(Guid.NewGuid())),
                            Bind(_createdMember, now),
                            Bind(_updatedMember, now)
                        });

                    Create = Lambda<Func<TEntity>>(
                        MemberInit(
                            New(typeof(TEntity)), CreateParameters))
                        .Compile();
                }
            }

            internal static class UpdateImplementation
            {
                internal static Action<TEntity> Update { get; }
                static UpdateImplementation()
                {
                    UpdateParameters.Add(
                        Assign(
                        Property(UpdateEntityParameter, _updatedMember.Name),
                        Constant(DateTime.UtcNow)));

                    Update = Lambda<Action<TEntity>>(
                        Block(UpdateParameters),
                        UpdateEntityParameter)
                        .Compile();
                }
            }
        }

        public static Func<TEntity> Create<TEntity>(List<MemberAssignment> exprs)
            where TEntity : IEntity
        {
            ModifierImplementation<TEntity>.CreateParameters = exprs;
            return ModifierImplementation<TEntity>.CreateImplementation.Create;
        }
        public static Action<TEntity> Update<TEntity>(List<Expression> exprs, ParameterExpression entityParameter)
            where TEntity : IEntity
        {
            ModifierImplementation<TEntity>.UpdateParameters = exprs;
            ModifierImplementation<TEntity>.UpdateEntityParameter = entityParameter;
            return ModifierImplementation<TEntity>.UpdateImplementation.Update;
        }
    }
}
