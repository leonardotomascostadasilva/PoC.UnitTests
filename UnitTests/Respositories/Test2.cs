//[assembly: CollectionBehavior(Parallelization = true)]
using Moq;
using PoC.UnitTests.Wrapper;
using System.Runtime.CompilerServices;

namespace UnitTests.Respositories
{
   // [Collection("MySerialTests")]
    public class MyTests1
    {
        private readonly Mock<IDapperWrapper> dapperWrapper;

        public MyTests1()
        {
            this.dapperWrapper = new Mock<IDapperWrapper>();
        }

        [Fact]
        public void Test1()
        {
            Console.WriteLine("Test1 START");
            Thread.Sleep(3000);
            Console.WriteLine("Test1 END");
        }


        [Fact]
        public void T1()
        {
            throw new NotImplementedException($"{RuntimeHelpers.GetHashCode(dapperWrapper.Object)}");
        }

        [Fact]
        public void T2()
        {
            throw new NotImplementedException($"{RuntimeHelpers.GetHashCode(dapperWrapper.Object)}");
        }

        [Fact]
        public void T3()
        {
            throw new NotImplementedException($"{RuntimeHelpers.GetHashCode(dapperWrapper.Object)}");
        }
    }

    //[Collection("MySerialTests")]
    public class MyTests2
    {
        [Fact]
        public void Test2()
        {
            Console.WriteLine("Test2 START");
            Thread.Sleep(3000);
            Console.WriteLine("Test2 END");
        }
    }
}
