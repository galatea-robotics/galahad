using System;
using System.Globalization;
using System.Text;

namespace Galahad.API
{
    static class TypeParser
    {
        public static object Parse(Type type, string value)
        {
            if (type == typeof(string)) return value;
            else if (type == typeof(bool)) return bool.Parse(value);
            else if (type == typeof(double)) return double.Parse(value, CultureInfo.InvariantCulture);
            else if (type == typeof(short)) return short.Parse(value, CultureInfo.InvariantCulture);
            else if (type == typeof(int)) return int.Parse(value, CultureInfo.InvariantCulture);
            else if (type == typeof(long)) return long.Parse(value, CultureInfo.InvariantCulture);
            else if (type == typeof(float)) return float.Parse(value, CultureInfo.InvariantCulture);

            else throw new NotImplementedException("Type '{type}' not implemented.");
        }

        public static byte[] GetBytes(Type type, object value)
        {
            if (type == typeof(string)) return Encoding.UTF8.GetBytes((string)value);
            else if (type == typeof(bool)) return BitConverter.GetBytes((bool)value);
            else if (type == typeof(double)) return BitConverter.GetBytes((double)value);
            else if (type == typeof(short)) return BitConverter.GetBytes((short)value);
            else if (type == typeof(int)) return BitConverter.GetBytes((int)value);
            else if (type == typeof(long)) return BitConverter.GetBytes((long)value);
            else if (type == typeof(float)) return BitConverter.GetBytes((float)value);

            else throw new NotImplementedException("Type '{type}' not implemented.");
        }
    }
}
