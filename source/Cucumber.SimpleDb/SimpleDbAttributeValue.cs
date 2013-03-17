using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using Cucumber.SimpleDb.Utilities;
using System.Collections;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents the value of a <c>ISimpleDbAttribute</c>
    /// <para>A SimpleDb attribute can have multiple values that are implicitly typed for query comparison.
    /// By default, a comparison expression evaluates true if any of the values on an attribute compare true to the test value.
    /// If <c>Every()</c> is used, comparison expressions will require true for all values to evaluate true.</para>
    /// </summary>
    public class SimpleDbAttributeValue
    {
        private delegate bool ValueComparer(SimpleDbAttributeValue left, SimpleDbAttributeValue right, ExpressionType operatorType);
        private readonly string[] _values;
        private readonly Type _originalType;
        private ValueComparer _comparer;

        /// <summary>
        /// Gets the underlying literal values.
        /// </summary>
        public ReadOnlyCollection<string> Values
        {
            get { return new ReadOnlyCollection<string>(_values); }
        }

        internal SimpleDbAttributeValue(params string[] values)
        {
            _values = values;
        }
        
        private SimpleDbAttributeValue(ValueComparer comparer, params string[] values)
            : this(values)
        {
            _comparer = comparer;
        }

        private SimpleDbAttributeValue(string value, Type originalType)
            : this(value)
        {
            _originalType = originalType;
        }

        /// <summary>
        /// Gets an instance of the current value which evaluates comparisons against every underlying value instead of any underlying value.
        /// </summary>
        /// <returns></returns>
        public SimpleDbAttributeValue Every()
        {
            return new SimpleDbAttributeValue(EveryValueComparer, _values);
        }

        /// <summary>
        /// Gets whether the value starts with the specified value.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="partialValue"/> is null or empty.</exception>
        /// <param name="partialValue">The partial value to search for.</param>
        /// <returns>True if the value starts with the specified value; otherwise false.</returns>
        public bool StartsWith(string partialValue)
        {
            if (string.IsNullOrEmpty(partialValue))
            {
                throw new ArgumentNullException("partialValue");
            }
            return _values.Any(val => val.StartsWith(partialValue));
        }

        /// <summary>
        /// Gets whether the value ends with the specified value.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="partialValue"/> is null or empty.</exception>
        /// <param name="partialValue">The partial value to search for.</param>
        /// <returns>True if the value ends with the specified value; otherwise false.</returns>
        public bool EndsWith(string partialValue)
        {
            if (string.IsNullOrEmpty(partialValue))
            {
                throw new ArgumentNullException("partialValue");
            }
            return _values.Any(val => val.EndsWith(partialValue));
        }

        /// <summary>
        /// Gets whether the value contains the specified value.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="partialValue"/> is null or empty.</exception>
        /// <param name="partialValue">The partial value to search for.</param>
        /// <returns>True if the value contains the specified value; otherwise false.</returns>
        public bool Contains(string partialValue)
        {
            if (string.IsNullOrEmpty(partialValue))
            {
                throw new ArgumentNullException("partialValue");
            }
            return _values.Any(val => val.Contains(partialValue));
        }

        /// <summary>
        /// Gets whether the value falls between the specified bounds (inclusive).
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when <paramref name="lower"/> is greater than or equal to <paramref name="upper"/>.</exception>
        /// <param name="lower">The inclusive lower bound to test.</param>
        /// <param name="upper">The inclusive upper bound to test.</param>
        /// <returns>True if the value falls between the test values; otherwise false.</returns>
        public bool Between(int lower, int upper)
        {
            if (lower >= upper)
            {
                throw new ArgumentOutOfRangeException("The 'lower' argument must be less than 'upper'", new Exception());
            }
            return LiftValuesToType<int>(_values).Any(v => v >= lower && v <= upper);
        }

        /// <summary>
        /// Gets whether the value falls between the specified bounds (inclusive).
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when <paramref name="lower"/> is greater than or equal to <paramref name="upper"/>.</exception>
        /// <param name="lower">The inclusive lower bound to test.</param>
        /// <param name="upper">The inclusive upper bound to test.</param>
        /// <returns>True if the value falls between the test values; otherwise false.</returns>
        public bool Between(double lower, double upper)
        {
            if (lower >= upper)
            {
                throw new ArgumentOutOfRangeException("The 'lower' argument must be less than 'upper'", new Exception());
            }
            return LiftValuesToType<double>(_values).Any(v => v >= lower && v <= upper);
        }

        public static implicit operator SimpleDbAttributeValue(SimpleDbAttributeValue[] values)
        {
            return new SimpleDbAttributeValue(values.SelectMany(v => v._values).ToArray());
        }

        public static implicit operator SimpleDbAttributeValue(string value)
        {
            return new SimpleDbAttributeValue(value, typeof(string));
        }

        public static implicit operator SimpleDbAttributeValue(int value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(int));
        }

        public static implicit operator SimpleDbAttributeValue(long value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(long));
        }

        public static implicit operator SimpleDbAttributeValue(float value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(float));
        }

        public static implicit operator SimpleDbAttributeValue(double value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(double));
        }

        public static implicit operator SimpleDbAttributeValue(decimal value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(decimal));
        }

        public static implicit operator SimpleDbAttributeValue(DateTime value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(DateTime));
        }

        public static implicit operator SimpleDbAttributeValue(bool value)
        {
            return new SimpleDbAttributeValue(value.ToString(), typeof(bool));
        }

        public override string ToString()
        {
            return _values.Concat(new[] { "" }).First();
        }

        public override int GetHashCode()
        {
            return string.Join ("", this._values).GetHashCode ();
        }

        public override bool Equals(object obj)
        {
            SimpleDbAttributeValue left = this;
            SimpleDbAttributeValue right = obj as SimpleDbAttributeValue;
            if ((object)right != null)
            {
                return GetValueComparer(left, right)(left, right, ExpressionType.Equal);
            }
            return false;
        }

        public static bool operator >(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            if ((object)left != null && (object)right != null)
            {
                return GetValueComparer(left, right)(left, right, ExpressionType.GreaterThan);
            }
            return false;
        }

        public static bool operator <(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            if ((object)left != null && (object)right != null)
            {
                return GetValueComparer(left, right)(left, right, ExpressionType.LessThan);
            }
            return false;
        }

        public static bool operator >=(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            if ((object)left != null && (object)right != null)
            {
                return GetValueComparer(left, right)(left, right, ExpressionType.GreaterThanOrEqual);
            }
            return false;
        }

        public static bool operator <=(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            if ((object)left != null && (object)right != null)
            {
                return GetValueComparer(left, right)(left, right, ExpressionType.LessThanOrEqual);
            }
            return false;
        }

        public static bool operator ==(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            if ((object)left != null)
            {
                return left.Equals(right);
            }
            if ((object)right != null)
            {
                return right.Equals(left);
            }
            return true;
        }

        public static bool operator !=(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            return !(left == right);
        }

        private static ValueComparer GetValueComparer(SimpleDbAttributeValue left, SimpleDbAttributeValue right)
        {
            return left.Try(l => l._comparer) ?? right.Try(l => l._comparer) ?? AnyValueComparer;
        }

        private static bool AnyValueComparer(SimpleDbAttributeValue left, SimpleDbAttributeValue right, ExpressionType operatorType)
        {
            Type type = left._originalType ?? right._originalType;
            foreach (object leftValue in LiftValuesToType(left._values, type))
            {
                foreach (object rightValue in LiftValuesToType(right._values, type))
                {
                    bool match = Expression.Lambda<Func<bool>>(
                        Expression.MakeBinary(operatorType,
                            Expression.Constant(leftValue),
                            Expression.Constant(rightValue))).Compile()();
                    if (match)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool EveryValueComparer(SimpleDbAttributeValue left, SimpleDbAttributeValue right, ExpressionType operatorType)
        {
            Type type = left._originalType ?? right._originalType;
            foreach (object leftValue in LiftValuesToType(left._values, type))
            {
                foreach (object rightValue in LiftValuesToType(right._values, type))
                {
                    bool match = Expression.Lambda<Func<bool>>(
                        Expression.MakeBinary(operatorType,
                            Expression.Constant(leftValue),
                            Expression.Constant(rightValue))).Compile()();
                    if (!match)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static IEnumerable<T> LiftValuesToType<T>(string[] values)
        {
            return LiftValuesToType(values, typeof(T)).OfType<T>();
        }

        private static IEnumerable LiftValuesToType(string[] values, Type type)
        {
            foreach (var value in values)
            {
                object typedValue = null;
                if (TryParse(type, value, out typedValue))
                {
                    yield return typedValue;
                }
            }
        }

        private static bool TryParse(Type type, string value, out object parsedValue)
        {
            parsedValue = null;
            MethodInfo method = type.GetMethod("TryParse", new [] { typeof(string), Type.GetType(type.ToString() + "&") });
            bool success = false;
            if (method != null)
            {
                object[] args = new[] { value, parsedValue };
                success = (bool)method.Invoke(null, args);
                if (success)
                {
                    parsedValue = args[1];
                }
            }
            if (!success)
            {
                try
                {
                    parsedValue = Convert.ChangeType(value, type);
                    success = true;
                }
                catch
                {
                }
            }
            return success;
        }
    }
}
