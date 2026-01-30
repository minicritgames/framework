using System;
using System.Collections.Generic;
using System.Linq;

namespace Minikit
{
    public static class MKEnumExtensions
    {
        private static void CheckIsEnum<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Type '{typeof(T).FullName}' is not an enum");
            }
        }

        private static void CheckIsFlagsEnum<T>()
        {
            CheckIsEnum<T>();

            if (!IsFlagsEnum<T>())
            {
                throw new ArgumentException($"Type '{typeof(T).FullName}' doesn't have the 'Flags' attribute");
            }
        }
        
        public static bool IsFlagsEnum<T>()
        {
            return IsFlagsEnum(typeof(T));
        }

        public static bool IsFlagsEnum(this Type _type)
        {
            return Attribute.IsDefined(_type, typeof(FlagsAttribute));
        }

        public static bool IsFlagSet<T>(this T _value, T _flag) where T : struct
        {
            CheckIsFlagsEnum<T>();
            long lValue = Convert.ToInt64(_value);
            long lFlag = Convert.ToInt64(_flag);
            return (lValue & lFlag) != 0;
        }

        public static IEnumerable<T> GetFlags<T>(this T _value) where T : struct
        {
            CheckIsFlagsEnum<T>();
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (_value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        public static T SetFlags<T>(this T _value, T _flags, bool _on) where T : struct
        {
            CheckIsFlagsEnum<T>();
            long lValue = Convert.ToInt64(_value);
            long lFlag = Convert.ToInt64(_flags);
            if (_on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static T SetFlags<T>(this T _value, T flags) where T : struct
        {
            return _value.SetFlags(flags, true);
        }

        public static T ClearFlags<T>(this T _value, T _flags) where T : struct
        {
            return _value.SetFlags(_flags, false);
        }

        public static T CombineFlags<T>(this IEnumerable<T> _flags) where T : struct
        {
            CheckIsFlagsEnum<T>();
            long lValue = 0;
            foreach (T flag in _flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }
        
        public static bool HasAnyFlags<T>(this T _value, T _other)
        {
            CheckIsFlagsEnum<T>();
            return (Convert.ToInt32(_value) & Convert.ToInt32(_other)) != 0;
        }
    }
} // Minikit namespace
