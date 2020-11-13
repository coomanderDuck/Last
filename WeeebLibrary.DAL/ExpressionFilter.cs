using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WeeebLibrary.DAL
{
    public static class ExpressionFilter
    {
        public static IOrderedQueryable<T> OrderByExp<T>(this IQueryable<T> source, string property)
        {
            var sortProperty = typeof(T).GetProperty(property);
            var parametrExpression = Expression.Parameter(typeof(T), "x"); //параметр
            Expression propertyExpression = Expression.Property(parametrExpression, property); //свойство параметра

            var lambda = Expression.Lambda(propertyExpression, parametrExpression); //нетепизированная лямбда

            var methods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);//все публичные и статические методы типа Queryable 
            var orderByMethod = methods.Where(m => m.Name == "OrderBy" && m.GetParameters().Count() == 2).First();
            orderByMethod = orderByMethod.MakeGenericMethod(typeof(T), propertyExpression.Type);//типизированный OrderBy

            var rezult = (IOrderedQueryable<T>)orderByMethod.Invoke(null, new object[] { source, lambda });

            return rezult;
        }

        public static IQueryable<T> WhereExp<T>(this IQueryable<T> source, string propertyName, string rule, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "type");
            var propertyExp = Expression.Property(parameterExp, propertyName);           
            var someValue = Expression.Constant(propertyValue, typeof(string));
            Expression<Func<T,bool>> lambda;

            if (rule == "==")
            {
                var equalExpr = Expression.Equal(propertyExp, someValue);
                lambda = Expression.Lambda<Func<T, bool>>(equalExpr, parameterExp);        
            }
            else if (rule == ">")
            {
                var greaterThanExpr = Expression.GreaterThan(propertyExp, someValue);
                lambda = Expression.Lambda<Func<T, bool>>(greaterThanExpr, parameterExp);
            }
            else if (rule == "<")
            {
                var greaterThanExpr = Expression.LessThanOrEqual(propertyExp, someValue);
                lambda = Expression.Lambda<Func<T, bool>>(greaterThanExpr, parameterExp);
            }
            else
            {
                MethodInfo ConMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsMethodExp = Expression.Call(propertyExp, ConMethod, someValue);
                lambda = Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);               
            };

            return source.Where(lambda);
        }
    }
}
