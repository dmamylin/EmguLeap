using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLinear
{
    public static class NumericConverter<T>
    {
        static Func<double, T> compiledToDoubleExpression;

        static Func<T, double> compiledFromDoubleExpression;

        static void CompileConvertToDoubleExpression()
        {
            ParameterExpression parameter1 = Expression.Parameter(typeof(double), "d");

            Expression convert = Expression.Convert(
                            parameter1,
                            typeof(T)
                        );

            compiledToDoubleExpression = Expression.Lambda<Func<double, T>>(convert, parameter1).Compile();
        }

        static void CompileConvertFromDoubleExpression()
        {
            ParameterExpression parameter1 = Expression.Parameter(typeof(T), "d");

            Expression convert = Expression.Convert(
                            parameter1,
                            typeof(double)
                        );

            compiledFromDoubleExpression = Expression.Lambda<Func<T, double>>(convert, parameter1).Compile();
        }

        static NumericConverter()
        {
            CompileConvertToDoubleExpression();

            CompileConvertFromDoubleExpression();
        }

        static public double ToDouble(T value)
        {
            return compiledFromDoubleExpression(value);
        }

        static public T FromDouble(double d)
        {
            return compiledToDoubleExpression(d);
        }
    }
}
