namespace Baksteen.Extensions.DeepSize;
using System.Reflection;
using System.Runtime.InteropServices;

public static class DeepSizeObjectExtensions
{
    /// <summary>
    /// Calculates the size/weight* of an object and everything it contains or references.
    /// * it's approximate because it does not account for the internal memory representation that may differ slightly due to field alignment padding
    /// </summary>
    /// <param name="original">the object to measure</param>
    /// <returns>size of the object in bytes</returns>
    public static long DeepSize(this object? original)
    {
        return new DeepSizeContext().ObjectSize(original, true);
    }

    private class DeepSizeContext
    {
        // to handle object graphs containing cycles, _visited keeps track of instances we've already measured
        private readonly HashSet<object> _visited = new(ReferenceEqualityComparer.Instance);

        public long ObjectSize(object? obj, bool includeInObjectGraph)
        {
            if(obj is null) return 0;

            if(includeInObjectGraph)
            {
                if(_visited.Contains(obj)) return 0;
                _visited.Add(obj);
            }

            if(obj is string str) return Marshal.SizeOf<nint>() +       // pointer to char array
                                     sizeof(int) +                      // array length
                                     sizeof(int) +                      // string length
                                     str.Length * sizeof(char);         // characters

            var type = obj.GetType();

            if(type.IsPrimitive)
            {
                return Marshal.SizeOf(type);
            }
            else if(type.IsPointer)
            {
                return Marshal.SizeOf(typeof(nint));
            }
            else if(type.IsEnum)
            {
                return Marshal.SizeOf(type.GetEnumUnderlyingType());
            }
            else if(obj is Delegate)
            {
                return Marshal.SizeOf<nint>() + Marshal.SizeOf<nint>();
            }
            else if(type.IsArray)
            {
                var arrayElementType = type.GetElementType()!;
                var arr = (Array)obj;

                if(arrayElementType.IsPrimitive)
                {
                    return sizeof(long) + arr.LongLength * Marshal.SizeOf(arrayElementType);
                }
                else if(arrayElementType.IsPointer)
                {
                    return sizeof(long) + arr.LongLength * Marshal.SizeOf<nint>();
                }
                else if(arrayElementType.IsEnum)
                {
                    return sizeof(long) + arr.LongLength * Marshal.SizeOf(arrayElementType.GetEnumUnderlyingType());
                }
                else if(arrayElementType.IsValueType)
                {
                    long result = sizeof(long);
                    foreach(var el in arr)
                    {
                        result += ObjectSize(el, false);
                    }
                    return result;
                }
                else
                {
                    long result = sizeof(long);
                    foreach(var el in arr)
                    {
                        result += Marshal.SizeOf<nint>();
                        result += ObjectSize(el, true);
                    }
                    return result;
                }
            }
            else // class or value type
            {
                long result = 0;
                foreach(var fieldInfo in DeepFields(type))
                {
                    var fieldType = fieldInfo.FieldType;

                    if(fieldType.IsPrimitive)
                    {
                        result += Marshal.SizeOf(fieldType);
                    }
                    else if(fieldType.IsPointer)
                    {
                        result += Marshal.SizeOf<nint>();
                    }
                    else if(fieldType.IsEnum)
                    {
                        result += Marshal.SizeOf(fieldType.GetEnumUnderlyingType());
                    }
                    else if(fieldType.IsValueType)
                    {
                        var fieldValue = fieldInfo.GetValue(obj);
                        if(fieldValue == null) throw new Exception("valuetype should never be null");
                        result += ObjectSize(fieldValue, false);
                    }
                    else
                    {
                        result += Marshal.SizeOf<nint>();
                        var fieldValue = fieldInfo.GetValue(obj);
                        if(fieldValue != null)
                        {
                            result += ObjectSize(fieldValue, true);
                        }
                    }
                }
                return result;
            }

        }

        /// <summary>
        /// From the given type hierarchy (i.e. including all base types), return all fields
        /// </summary>
        /// <param name="typeToReflect"></param>
        /// <returns></returns>
        private static IEnumerable<FieldInfo> DeepFields(Type typeToReflect)
        {
            while(typeToReflect.BaseType != null)
            {
                foreach(var fieldInfo in typeToReflect.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    yield return fieldInfo;
                }
                typeToReflect = typeToReflect.BaseType;
            }
        }
    }
}
