using in_memory_db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

//todo add corner cases, and have more extensive testing.
namespace in_memory_db_tests
{
    [TestClass]
    public class MergeSortTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            int[] sortedArr = Funcs.MergeSort(new int[] { 1, 4, 3, 2 });
            Assert.IsTrue(sortedArr.SequenceEqual(new int[] { 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void TestMethod2()
        {
            int[] sortedArr = Funcs.MergeSort(new int[] { 1, 4, 3, 2, 7 });
            Assert.IsTrue(sortedArr.SequenceEqual(new int[] { 1, 2, 3, 4, 7 }));
        }

        [TestMethod]
        public void TestMethod3()
        {
            int[] sortedArr = Funcs.MergeSort(new int[] { 1, 4, 78, 3, 2, 34, 22, 1 });
            Assert.IsTrue(sortedArr.SequenceEqual(new int[] { 1, 1, 2, 3, 4, 22, 34, 78 }));
        }
    }
}
