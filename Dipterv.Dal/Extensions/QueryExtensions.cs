using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Dal.Extensions
{
    public static class QueryExtensions
    {
        public static Expression<Func<TIn, TOut>> Chain<TIn, TInterstitial, TOut>(
            this Expression<Func<TIn, TInterstitial>> inner,
            Expression<Func<TInterstitial, TOut>> outer)
        {
            var visitor = new SwapVisitor(outer.Parameters[0], inner.Body);
            return Expression.Lambda<Func<TIn, TOut>>(visitor.Visit(outer.Body), inner.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp)
            => ChainWithLogicalOperator(exp, newExp, Expression.OrElse);

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp)
            => ChainWithLogicalOperator(exp, newExp, Expression.AndAlso);

        private static Expression<Func<T, bool>> ChainWithLogicalOperator<T>(
            Expression<Func<T, bool>> exp,
            Expression<Func<T, bool>> newExp,
            Func<Expression, Expression, BinaryExpression> operatorFunc)
        {
            var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
            newExp = visitor.Visit(newExp) as Expression<Func<T, bool>>;

            var binExp = operatorFunc(exp.Body, newExp.Body);
            return Expression.Lambda<Func<T, bool>>(binExp, newExp.Parameters);
        }

        private class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression _source;
            private readonly Expression _replacement;

            public SwapVisitor(Expression source, Expression replacement)
            {
                _source = source;
                _replacement = replacement;
            }

            public override Expression Visit(Expression node)
            {
                return node == _source ? _replacement : base.Visit(node);
            }
        }

        private class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }
    }
}
