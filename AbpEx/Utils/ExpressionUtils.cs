using System;
using System.Linq.Expressions;

namespace Abp.Utils
{    
	public static class ExpressionUtils
    {       
        public static Expression<Func<T, bool>> MakePredicate<T>(
            string name, 
            object value, 
            Type typeOfValue = null)
        {            
            var param = Expression.Parameter(typeof(T), typeof(T).Name);
            var memberExp = Expression.Property(param, name);
            var body = Expression.Equal(memberExp, typeOfValue == null ? Expression.Constant(value) : Expression.Constant(value, typeOfValue));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
