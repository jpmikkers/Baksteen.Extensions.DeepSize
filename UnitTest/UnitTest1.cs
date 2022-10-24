using Baksteen.Extensions.DeepSize;
using System.Runtime.InteropServices;

namespace UnitTest
{
    [TestClass]
    public class UnitTests
    {
        private const long ArrayRangeSize = 2 * sizeof(int);
        private static long ReferenceSize = Marshal.SizeOf<nint>();

        private enum ByteEnum : byte
        {
            Zero,
        }

        private enum ShortEnum : short
        {
            Zero,
        }

        private enum IntEnum : int
        {
            Zero,
        }

        private enum LongEnum : long
        {
            Zero,
        }

        private class TestClass<T>
        {
            public T Value { get; private set; }

            public TestClass(T value)
            {
                Value = value;
            }

            public TestClass() : this(default!)
            {
            }
        }

        private struct TestStruct<T>
        {
            public T Value { get; private set; }

            public TestStruct(T value)
            {
                Value = value;
            }

            public TestStruct() : this(default!)
            {
            }
        }

        [TestMethod]
        public void PrimitiveSizes()
        {
            Assert.AreEqual(1, new bool().DeepSize());

            Assert.AreEqual(1, new sbyte().DeepSize());
            Assert.AreEqual(1, new byte().DeepSize());

            Assert.AreEqual(2, new char().DeepSize());

            Assert.AreEqual(2, new short().DeepSize());
            Assert.AreEqual(2, new ushort().DeepSize());

            Assert.AreEqual(4, new int().DeepSize());
            Assert.AreEqual(4, new uint().DeepSize());

            Assert.AreEqual(8, new long().DeepSize());
            Assert.AreEqual(8, new ulong().DeepSize());

            Assert.AreEqual(4, new float().DeepSize());
            Assert.AreEqual(8, new double().DeepSize());
            Assert.AreEqual(16, new decimal().DeepSize());

            Assert.AreEqual(ReferenceSize, new nint().DeepSize());
            Assert.AreEqual(ReferenceSize, new nuint().DeepSize());

            Assert.AreEqual(1, new ByteEnum().DeepSize());
            Assert.AreEqual(2, new ShortEnum().DeepSize());
            Assert.AreEqual(4, new IntEnum().DeepSize());
            Assert.AreEqual(8, new LongEnum().DeepSize());
        }

        [TestMethod]
        public void ArraySizeDependsOnRank()
        {
            Assert.AreEqual(1 * ArrayRangeSize,new byte[0].DeepSize());
            Assert.AreEqual(2 * ArrayRangeSize, new byte[0,0].DeepSize());
            Assert.AreEqual(3 * ArrayRangeSize, new byte[0,0,0].DeepSize());
            Assert.AreEqual(4 * ArrayRangeSize, new byte[0,0,0,0].DeepSize());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(11)]
        [DataRow(27)]
        public void PrimitiveArraySize(int len)
        {
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(bool), new bool[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(sbyte), new sbyte[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(byte), new byte[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(char), new char[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(short), new short[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(ushort), new ushort[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(int), new int[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(uint), new uint[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(long), new long[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(ulong), new ulong[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(float), new float[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(double), new double[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * sizeof(decimal), new decimal[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * ReferenceSize, new nint[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * ReferenceSize, new nuint[len].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + len * 1, new ByteEnum[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * 2, new ShortEnum[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * 4, new IntEnum[len].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + len * 8, new LongEnum[len].DeepSize());
        }

        [TestMethod]
        public void ClassWithFields()
        {
            Assert.AreEqual(1, new TestClass<bool>().DeepSize());
            Assert.AreEqual(1, new TestClass<sbyte>().DeepSize());
            Assert.AreEqual(1, new TestClass<byte>().DeepSize());

            Assert.AreEqual(2, new TestClass<char>().DeepSize());
            Assert.AreEqual(2, new TestClass<short>().DeepSize());
            Assert.AreEqual(2, new TestClass<ushort>().DeepSize());

            Assert.AreEqual(4, new TestClass<int>().DeepSize());
            Assert.AreEqual(4, new TestClass<uint>().DeepSize());

            Assert.AreEqual(8, new TestClass<long>().DeepSize());
            Assert.AreEqual(8, new TestClass<ulong>().DeepSize());

            Assert.AreEqual(4, new TestClass<float>().DeepSize());
            Assert.AreEqual(8, new TestClass<double>().DeepSize());
            Assert.AreEqual(16, new TestClass<decimal>().DeepSize());

            Assert.AreEqual(ReferenceSize, new TestClass<nint>().DeepSize());
            Assert.AreEqual(ReferenceSize, new TestClass<nuint>().DeepSize());

            Assert.AreEqual(1, new TestClass<ByteEnum>().DeepSize());
            Assert.AreEqual(2, new TestClass<ShortEnum>().DeepSize());
            Assert.AreEqual(4, new TestClass<IntEnum>().DeepSize());
            Assert.AreEqual(8, new TestClass<LongEnum>().DeepSize());

            // a System.Delegate is a class, so a class/struct containing a delegate field has a ref to the delegate, 
            // and the delegate itself consists of 2 refs
            Assert.AreEqual(ReferenceSize * 3, new TestClass<Action>(()=> { }).DeepSize());
            Assert.AreEqual(ReferenceSize * 1, new TestClass<Action>().DeepSize());
        }

        [TestMethod]
        public void StructWithFields()
        {
            Assert.AreEqual(1, new TestStruct<bool>().DeepSize());
            Assert.AreEqual(1, new TestStruct<sbyte>().DeepSize());
            Assert.AreEqual(1, new TestStruct<byte>().DeepSize());

            Assert.AreEqual(2, new TestStruct<char>().DeepSize());
            Assert.AreEqual(2, new TestStruct<short>().DeepSize());
            Assert.AreEqual(2, new TestStruct<ushort>().DeepSize());

            Assert.AreEqual(4, new TestStruct<int>().DeepSize());
            Assert.AreEqual(4, new TestStruct<uint>().DeepSize());

            Assert.AreEqual(8, new TestStruct<long>().DeepSize());
            Assert.AreEqual(8, new TestStruct<ulong>().DeepSize());

            Assert.AreEqual(4, new TestStruct<float>().DeepSize());
            Assert.AreEqual(8, new TestStruct<double>().DeepSize());
            Assert.AreEqual(16, new TestStruct<decimal>().DeepSize());

            Assert.AreEqual(ReferenceSize, new TestStruct<nint>().DeepSize());
            Assert.AreEqual(ReferenceSize, new TestStruct<nuint>().DeepSize());

            Assert.AreEqual(1, new TestStruct<ByteEnum>().DeepSize());
            Assert.AreEqual(2, new TestStruct<ShortEnum>().DeepSize());
            Assert.AreEqual(4, new TestStruct<IntEnum>().DeepSize());
            Assert.AreEqual(8, new TestStruct<LongEnum>().DeepSize());

            Assert.AreEqual(ReferenceSize * 3, new TestStruct<Action>(() => { }).DeepSize());
            Assert.AreEqual(ReferenceSize * 1, new TestStruct<Action>().DeepSize());
        }
    }
}
