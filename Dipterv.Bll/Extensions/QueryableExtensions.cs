using Dipterv.Shared.Enum;
using Dipterv.Shared.Exceptions;
using Dipterv.Shared.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Bll.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> conditionTruePredicate, Expression<Func<TSource, bool>> conditionFalsePredicate = null)
        {
            if (condition)
            {
                return source.Where(conditionTruePredicate);
            }
            else if (conditionFalsePredicate != null)
            {
                return source.Where(conditionFalsePredicate);
            }
            else
            {
                return source;
            }
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool? condition, Expression<Func<TSource, bool>> truePredicate, Expression<Func<TSource, bool>> falsePredicate)
        {
            if (!condition.HasValue)
                return source;

            return condition.Value
                ? source.Where(truePredicate)
                : source.Where(falsePredicate);
        }

        public static IQueryable<T> If<T>(
            this IQueryable<T> source,
            bool condition,
            Func<IQueryable<T>, IQueryable<T>> transform
        )
        {
            return condition ? transform(source) : source;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
            this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, OrderDirection orderDirection)
        {
            return orderDirection == OrderDirection.Descending
                ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(
            this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, OrderDirection orderDirection)
        {
            if (orderDirection == OrderDirection.Descending)
            {
                return source.ThenByDescending(keySelector);
            }

            return source.ThenBy(keySelector);
        }

        //public static IOrderedQueryable<TSource> OrderBy<TSource, TDto>(
        //    this IQueryable<TSource> source, PageRequest pageRequest, Expression<Func<TSource, object>> defaultOrderingSelector, IConfigurationProvider mappings)
        //{
        //    if (!string.IsNullOrEmpty(pageRequest.OrderBy))
        //    {
        //        // A kliens olyan property-re szeretne rendezni,
        //        // amire a lekérdezésben nincs ráhatásunk,
        //        // vagy nem szeretnénk engedni a rendezést
        //        var pi = typeof(TDto).GetProperty(pageRequest.OrderBy);
        //        if (pi is null || !pi.IsSortable())
        //        {
        //            // TODO lokalizáció
        //            throw new ValidationException(pageRequest.OrderBy, "Ezt az oszlopot nem lehet sorba rendezni!");
        //        }
        //    }

        //    Expression<Func<TSource, object>> orderKeySelector = defaultOrderingSelector;

        //    if (!string.IsNullOrEmpty(pageRequest.OrderBy))
        //    {
        //        var expression = mappings.FindTypeMapFor<TSource, TDto>()
        //            ?.PropertyMaps
        //            ?.Where(m => m.CustomMapExpression != null && m.DestinationName == pageRequest.OrderBy)
        //            .FirstOrDefault()
        //            ?.CustomMapExpression;

        //        orderKeySelector = expression != null
        //            ? Expression.Lambda<Func<TSource, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters)
        //            : e => EF.Property<object>(e, pageRequest.OrderBy);
        //    }

        //    return source.OrderBy(orderKeySelector, pageRequest.OrderDirection);
        //}

        public static async Task<PageResponse<TSource>> ToPagedListAsync<TSource>(
            this IQueryable<TSource> source, PageRequest pageRequest, CancellationToken cancellationToken = default)
        {
            var totalCount = await source.CountAsync(cancellationToken);

            return new PageResponse<TSource>(
                await source.Skip((pageRequest.Page - 1) * pageRequest.PageSize).Take(pageRequest.PageSize).ToListAsync(cancellationToken), pageRequest.Page, totalCount);
        }

        public static ValueTask<T> FindByIdAsync<T>(this DbSet<T> dbSet, object key, CancellationToken cancellationToken)
            where T : class
            => dbSet.FindAsync(new[] { key }, cancellationToken);

        public static string ToSql<TEntity>(this IQueryable<TEntity> query)
            where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
