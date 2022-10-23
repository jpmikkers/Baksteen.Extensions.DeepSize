using Baksteen.Extensions.DeepSize;
using System.Runtime.InteropServices;

namespace UnitTest
{
    [TestClass]
    public class UnitTests
    {
        private const long ArrayRangeSize = 2 * sizeof(int);
        private static long ReferenceSize = Marshal.SizeOf<nint>();

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
        public void PrimitiveArraySize(int l)
        {
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(bool), new bool[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(sbyte), new sbyte[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(byte), new byte[l].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(char), new char[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(short), new short[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(ushort), new ushort[l].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(int), new int[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(uint), new uint[l].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(long), new long[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(ulong), new ulong[l].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(float), new float[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(double), new double[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * sizeof(decimal), new decimal[l].DeepSize());

            Assert.AreEqual(1 * ArrayRangeSize + l * ReferenceSize, new nint[l].DeepSize());
            Assert.AreEqual(1 * ArrayRangeSize + l * ReferenceSize, new nuint[l].DeepSize());
        }
    }
}