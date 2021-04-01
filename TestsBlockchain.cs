using NUnit.Framework;
using N1_ComputerScience;
using System.Diagnostics;

namespace Tests
{
    public class TestsBlockchain
    {
        [TestCase(1)] //4ms
        [TestCase(2)] //0ms
        [TestCase(3)] //5ms
        [TestCase(4)] //16ms
        [TestCase(5)] //96ms
        [TestCase(6)] //750ms
        //[TestCase(7)] //5302ms
        //[TestCase(8)] //72989ms
        public void TestZeros(int zeros)
        {
            var watch = Stopwatch.StartNew();
            var blockchain = new Blockchain(zeros);
            blockchain.AddBlock("One");
            blockchain.AddBlock("Two");
            blockchain.AddBlock("Three");
            blockchain.AddBlock("Four");
            blockchain.AddBlock("Five");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.WriteLine($"{zeros} zeros: " + elapsedMs);
            Assert.AreEqual(6, blockchain.Blocks.Count);
            Assert.IsTrue(blockchain.IsCorrectChain());
        }
    }
}