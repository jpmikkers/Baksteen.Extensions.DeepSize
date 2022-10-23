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
        private static readonly Dictionary<Type, int> PrimitiveSizeLut;
        private static readonly long ReferenceSize = Marshal.SizeOf<nint>();

        static DeepSizeContext()
        {
            PrimitiveSizeLut = new()
            {
                { typeof(bool), 1 },
                { typeof(byte), 1 },
                { typeof(sbyte), 1 },

                { typeof(char), 2 },
                { typeof(short), 2 },
                { typeof(ushort), 2 },

                { typeof(int), 4 },
                { typeof(uint), 4 },

                { typeof(long), 8 },
                { typeof(ulong), 8 },

                { typeof(float), 4 },
                { typeof(double), 8 },
                { typeof(decimal), 16 },

                { typeof(nint), Marshal.SizeOf<nint>() },
                { typeof(nuint), Marshal.SizeOf<nuint>() },
            };
        }

        public long ObjectSize(object? obj, bool includeInObjectGraph)
        {
            if(obj is null) return 0;

            if(includeInObjectGraph)
            {
                if(_visited.Contains(obj)) return 0;
                _visited.Add(obj);
            }

            if(obj is string str) return ReferenceSize +                // pointer to char array
                                     sizeof(int) +                      // array length
                                     sizeof(int) +                      // string length
                                     str.Length * sizeof(char);         // characters

            var type = obj.GetType();

            if(type.IsPrimitive)
            {
                return PrimitiveSizeLut[type];
            }
            else if(type.IsPointer)
            {
                return PrimitiveSizeLut[typeof(nint)];
            }
            else if(type.IsEnum)
            {
                return PrimitiveSizeLut[type.GetEnumUnderlyingType()];
            }
            else if(obj is Delegate)
            {
                return 2*ReferenceSize;     // pointer to object + pointer to method
            }
            else if(type.IsArray)
            {
                var arrayElementType = type.GetElementType()!;
                var arr = (Array)obj;

                // assume an array uses an int for lower and upper bound per dimension
                long result = 2L * sizeof(int) * arr.Rank;

                if(arrayElementType.IsPrimitive)
                {
                    result += arr.LongLength * PrimitiveSizeLut[arrayElementType];
                }
                else if(arrayElementType.IsPointer)
                {
                    result += arr.LongLength * ReferenceSize;
                }
                else if(arrayElementType.IsEnum)
                {
                    result += arr.LongLength * PrimitiveSizeLut[arrayElementType.GetEnumUnderlyingType()];
                }
                else if(arrayElementType.IsValueType)
                {
                    foreach(var el in arr)
                    {
                        result += ObjectSize(el, false);
                    }
                }
                else
                {
                    foreach(var el in arr)
                    {
                        result += ReferenceSize;
                        result += ObjectSize(el, true);
                    }
                }
                return result;
            }
            else // class or value type
            {
                long result = 0;
                foreach(var fieldInfo in DeepFields(type))
                {
                    var fieldType = fieldInfo.FieldType;

                    if(fieldType.IsPrimitive)
                    {
                        result += PrimitiveSizeLut[fieldType];
                    }
                    else if(fieldType.IsPointer)
                    {
                        result += ReferenceSize;
                    }
                    else if(fieldType.IsEnum)
                    {
                        result += PrimitiveSizeLut[fieldType.GetEnumUnderlyingType()];
                    }
                    else if(fieldType.IsValueType)
                    {
                        var fieldValue = fieldInfo.GetValue(obj);
                        if(fieldValue == null) throw new Exception("valuetype should never be null");
                        result += ObjectSize(fieldValue, false);
                    }
                    else
                    {
                        result += ReferenceSize;
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
