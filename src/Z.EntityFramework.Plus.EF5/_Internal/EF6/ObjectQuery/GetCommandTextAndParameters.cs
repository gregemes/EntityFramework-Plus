﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_CACHE || QUERY_FILTER
#if EF6
using System;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static Tuple<string, DbParameterCollection> GetCommandTextAndParameters(this ObjectQuery objectQuery)
        {
            var stateField = objectQuery.GetType().BaseType.GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
            var state = stateField.GetValue(objectQuery);
            var getExecutionPlanMethod = state.GetType().GetMethod("GetExecutionPlan", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExecutionPlan = getExecutionPlanMethod.Invoke(state, new object[] {null});
            var prepareEntityCommandMethod = getExecutionPlan.GetType().GetMethod("PrepareEntityCommand", BindingFlags.NonPublic | BindingFlags.Instance);

            var sql = "";
            using (var entityCommand = (EntityCommand) prepareEntityCommandMethod.Invoke(getExecutionPlan, new object[] {objectQuery.Context, objectQuery.Parameters}))
            {
                var getCommandDefinitionMethod = entityCommand.GetType().GetMethod("GetCommandDefinition", BindingFlags.NonPublic | BindingFlags.Instance);
                var getCommandDefinition = getCommandDefinitionMethod.Invoke(entityCommand, new object[0]);

                var prepareEntityCommandBeforeExecutionMethod = getCommandDefinition.GetType().GetMethod("PrepareEntityCommandBeforeExecution", BindingFlags.NonPublic | BindingFlags.Instance);
                var prepareEntityCommandBeforeExecution = (DbCommand) prepareEntityCommandBeforeExecutionMethod.Invoke(getCommandDefinition, new object[] {entityCommand});

                sql = prepareEntityCommandBeforeExecution.CommandText;
                var parameters = prepareEntityCommandBeforeExecution.Parameters;

                return new Tuple<string, DbParameterCollection>(sql, parameters);
            }
        }
    }
}

#endif
#endif