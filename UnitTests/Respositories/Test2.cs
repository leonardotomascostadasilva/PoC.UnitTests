//[assembly: CollectionBehavior(Parallelization = true)]
namespace UnitTests.Respositories
{
   // [Collection("MySerialTests")]
    public class MyTests1
    {
        [Fact]
        public void Test1()
        {
            Console.WriteLine("Test1 START");
            Thread.Sleep(3000);
            Console.WriteLine("Test1 END");
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
