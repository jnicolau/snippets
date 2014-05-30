using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Domain;

namespace Data
{
    internal interface ILinqExpression<T>
    {
        Func<T, Boolean> GetExpression();
    }

    internal static class SpecificationExtension
    {
        internal static Func<T, Boolean> GetLinqExpression<T>(this ISpecification<T> spec)
        {
            return ((ILinqExpression<T>)spec).GetExpression();
        }
    }
}
