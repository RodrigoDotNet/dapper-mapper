﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperMapper.Mapper
{
    internal class FasterInvoker
    {
        public static Action<T, object> BuildUntypedSetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetSetMethod();

            if (targetType == null || methodInfo == null) return default;


            var exTarget = Expression.Parameter(targetType, "t");
            var exValue = Expression.Parameter(typeof(object), "p");
            var exBody = Expression.Call(exTarget, methodInfo, Expression.Convert(exValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);
            var action = lambda.Compile();

            return action;

        }

        public static Func<T, object> BuildUntypedGetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetGetMethod();

            if (targetType == null || methodInfo == null) return default;

            var exTarget = Expression.Parameter(targetType, "t");
            var exBody = Expression.Call(exTarget, methodInfo);
            var exBody2 = Expression.Convert(exBody, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);
            var action = lambda.Compile();

            return action;
        }
    }
}
