using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using N1_ComputerScience;

namespace Tests
{
    [TestFixture]
    public class TestsBWT
    {
        [Test]
        public void TestBWT_NegativeCase_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new BWT(3, 3, false));
        }

        [Test]
        public void TestBWT_PositiveCase_CorrectTreeStructure()
        {
            var bwt = new BWT(3, 4, true);
            foreach (var node in bwt.DescendingTree)
            {
                Assert.IsTrue(node.ParentColor != node.LeftChildColor && node.ParentColor != node.RightChildColor && node.LeftChildColor != node.RightChildColor);
            }
            foreach (var node in bwt.AscendingTree)
            {
                Assert.IsTrue(node.ParentColor != node.LeftChildColor && node.ParentColor != node.RightChildColor && node.LeftChildColor != node.RightChildColor);
            }
        }

        [Test]
        public void TestBWT_PositiveCase_SymmetricTree()
        {
            var bwt = new BWT(3, 4, true);
            var rootDesc = bwt.RootOfDescendingTree;
            var rootAsc = bwt.RootOfAscendingTree;
            Assert.IsTrue(rootDesc.ParentColor == rootAsc.ParentColor);
            Assert.IsTrue(rootDesc.LeftChildColor == rootAsc.RightChildColor);
            Assert.IsTrue(rootDesc.RightChildColor == rootAsc.LeftChildColor);

            Assert.IsTrue(rootDesc.LeftChild.ParentColor == rootAsc.RightChild.ParentColor);
            Assert.IsTrue(rootDesc.LeftChild.LeftChildColor == rootAsc.RightChild.RightChildColor);
            Assert.IsTrue(rootDesc.LeftChild.RightChildColor == rootAsc.RightChild.LeftChildColor);

            Assert.IsTrue(rootDesc.RightChild.ParentColor == rootAsc.LeftChild.ParentColor);
            Assert.IsTrue(rootDesc.RightChild.LeftChildColor == rootAsc.LeftChild.RightChildColor);
            Assert.IsTrue(rootDesc.RightChild.RightChildColor == rootAsc.LeftChild.LeftChildColor);

            Assert.IsTrue(rootDesc.LeftChild.LeftChild.ParentColor == rootAsc.RightChild.RightChild.ParentColor);
            Assert.IsTrue(rootDesc.LeftChild.LeftChild.LeftChildColor == rootAsc.RightChild.RightChild.RightChildColor);
            Assert.IsTrue(rootDesc.LeftChild.LeftChild.RightChildColor == rootAsc.RightChild.RightChild.LeftChildColor);

            Assert.IsTrue(rootDesc.LeftChild.RightChild.ParentColor == rootAsc.RightChild.LeftChild.ParentColor);
            Assert.IsTrue(rootDesc.LeftChild.RightChild.LeftChildColor == rootAsc.RightChild.LeftChild.RightChildColor);
            Assert.IsTrue(rootDesc.LeftChild.RightChild.RightChildColor == rootAsc.RightChild.LeftChild.LeftChildColor);
        }

        [Test]
        public void TestBWT_PositiveCase_FindMinColor1()
        {
            var bwt = new BWT(3, 4, true);
            var path = bwt.FindPath(color: 1, FindCriterion.MinBranchesOfColor);
            var expected = new List<int> {1,2,4,9,24,8,23,19,17,16};
            Assert.AreEqual(expected.Count, path.Count);
            for (int i = 0; i < path.Count; i++)
            {
                Assert.AreEqual(expected[i], path[i].Index);
            }
        }

        [Test]
        public void TestBWT_PositiveCase_FindMaxColor4()
        {
            var bwt = new BWT(3, 4, true);
            var path = bwt.FindPath(color: 4, FindCriterion.MaxBranchesOfColor);
            var expected = new List<int> {1,2,4,8,23,9,24,19,17,16};
            Assert.AreEqual(expected.Count, path.Count);
            for (int i = 0; i < path.Count; i++)
            {
                Assert.AreEqual(expected[i], path[i].Index);
            }
        }

        [Test]
        public void TestBWT_PositiveCase_AverageNumberOfStepNormalModeling()
        {
            var bwt = new BWT(3, 4, true);
            var results = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                var normalNumberOfSteps = bwt.NormalModeling(1,16);
                results.Add(normalNumberOfSteps);
            }
            var avg = results.Average();
            Assert.AreEqual(avg, 82, 1);
        }

        [Test]
        public void TestBWT_PositiveCase_AverageNumberOfStepQuantumModeling()
        {
            var bwt = new BWT(3, 4, true);
            var results = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                var quantumNumberOfSteps = bwt.QuantumModeling(1,16);
                results.Add(quantumNumberOfSteps);
            }
            var avg = results.Average();
            Assert.AreEqual(avg, 11, 1);
        }

        [Test]
        public void TestBWT_PositiveCase_CompareNormalAndQuantumCalc()
        {
            var bwt = new BWT(3, 4, true);
            var normalNumberOfSteps = bwt.NormalModeling(1,16);
            var quantumNumberOfSteps = bwt.QuantumModeling(1,16);
            Assert.IsTrue(normalNumberOfSteps > quantumNumberOfSteps);
        }
    }
}