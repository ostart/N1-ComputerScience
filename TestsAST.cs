using NUnit.Framework;
using N1_ComputerScience;
using System.Collections.Generic;

namespace Tests
{
    public class TestsASTParser
    {
        [Test]
        public void Test1ParseFunction()
        {
            var input = "7+3*5-2";
            var result = AST.Parse(input);
            var ethalon = new List<ANode> 
            {
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "7" },
                new ANode { token_type = TokenType.Operation, token_value = "+" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "3" },
                new ANode { token_type = TokenType.Operation, token_value = "*" },
                new ANode { token_type = TokenType.Integer, token_value = "5" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Operation, token_value = "-" },
                new ANode { token_type = TokenType.Integer, token_value = "2" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" }
            };
            Assert.AreEqual(ethalon.Count, result.Count);
            for(var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(ethalon[i].token_type, result[i].token_type);
                Assert.AreEqual(ethalon[i].token_value, result[i].token_value);
            }
        }

        [Test]
        public void Test2ParseFunction()
        {
            var input = "7+3/25*(5-2)";
            var result = AST.Parse(input);
            var ethalon = new List<ANode> 
            {
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "7" },
                new ANode { token_type = TokenType.Operation, token_value = "+" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "3" },
                new ANode { token_type = TokenType.Operation, token_value = "/" },
                new ANode { token_type = TokenType.Integer, token_value = "25" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Operation, token_value = "*" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "5" },
                new ANode { token_type = TokenType.Operation, token_value = "-" },
                new ANode { token_type = TokenType.Integer, token_value = "2" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" }
            };
            Assert.AreEqual(ethalon.Count, result.Count);
            for(var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(ethalon[i].token_type, result[i].token_type);
                Assert.AreEqual(ethalon[i].token_value, result[i].token_value);
            }
        }

        [Test]
        public void TestBuildFunction()
        {
            var input = "7+3*5-2";
            var result = AST.Parse(input);
            var ethalon = new List<ANode> 
            {
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "7" },
                new ANode { token_type = TokenType.Operation, token_value = "+" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "3" },
                new ANode { token_type = TokenType.Operation, token_value = "*" },
                new ANode { token_type = TokenType.Integer, token_value = "5" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Operation, token_value = "-" },
                new ANode { token_type = TokenType.Integer, token_value = "2" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" }
            };
            Assert.AreEqual(ethalon.Count, result.Count);
            for(var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(ethalon[i].token_type, result[i].token_type);
                Assert.AreEqual(ethalon[i].token_value, result[i].token_value);
            }

            var actualTree = AST.Build(result);

            var root = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "+" }, null);
            var ethalonTree = new SimpleTree<ANode>(root);
            var leftRootChild = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "7" }, root);
            ethalonTree.AddChild(root, leftRootChild);
            var rightRootChild = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "-" }, root);
            ethalonTree.AddChild(root, rightRootChild);
            var leftMultiplyOperation = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "*" }, rightRootChild);
            ethalonTree.AddChild(rightRootChild, leftMultiplyOperation);
            var right2Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "2" }, rightRootChild);
            ethalonTree.AddChild(rightRootChild, right2Value);
            var left3Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "3" }, leftMultiplyOperation);
            ethalonTree.AddChild(leftMultiplyOperation, left3Value);
            var right5Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "5" }, leftMultiplyOperation);
            ethalonTree.AddChild(leftMultiplyOperation, right5Value);

            var actualTreeList = actualTree.GetAllNodes();
            var ethalonTreeList = ethalonTree.GetAllNodes();

            Assert.AreEqual(ethalonTreeList.Count, actualTreeList.Count);
            for(var i = 0; i < actualTreeList.Count; i++)
            {
                Assert.AreEqual(ethalonTreeList[i].NodeValue.token_type, actualTreeList[i].NodeValue.token_type);
                Assert.AreEqual(ethalonTreeList[i].NodeValue.token_value, actualTreeList[i].NodeValue.token_value);
                if (ethalonTreeList[i].Children == null) Assert.IsNull(actualTreeList[i].Children);
                else
                {
                    Assert.AreEqual(ethalonTreeList[i].Children.Count, actualTreeList[i].Children.Count);
                    Assert.AreEqual(ethalonTreeList[i].Children.Count, 2);
                    Assert.AreEqual(actualTreeList[i].Children.Count, 2);
                }
            }
        }

        [Test]
        public void TestInterpretAndTranslateFunction()
        {
            var input = "7+3*5-2";
            var result = AST.Parse(input);
            var ethalon = new List<ANode> 
            {
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "7" },
                new ANode { token_type = TokenType.Operation, token_value = "+" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Bracket, token_value = "(" },
                new ANode { token_type = TokenType.Integer, token_value = "3" },
                new ANode { token_type = TokenType.Operation, token_value = "*" },
                new ANode { token_type = TokenType.Integer, token_value = "5" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Operation, token_value = "-" },
                new ANode { token_type = TokenType.Integer, token_value = "2" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" },
                new ANode { token_type = TokenType.Bracket, token_value = ")" }
            };
            Assert.AreEqual(ethalon.Count, result.Count);
            for(var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(ethalon[i].token_type, result[i].token_type);
                Assert.AreEqual(ethalon[i].token_value, result[i].token_value);
            }

            var actualTree = AST.Build(result);

            var root = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "+" }, null);
            var ethalonTree = new SimpleTree<ANode>(root);
            var leftRootChild = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "7" }, root);
            ethalonTree.AddChild(root, leftRootChild);
            var rightRootChild = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "-" }, root);
            ethalonTree.AddChild(root, rightRootChild);
            var leftMultiplyOperation = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Operation, token_value = "*" }, rightRootChild);
            ethalonTree.AddChild(rightRootChild, leftMultiplyOperation);
            var right2Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "2" }, rightRootChild);
            ethalonTree.AddChild(rightRootChild, right2Value);
            var left3Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "3" }, leftMultiplyOperation);
            ethalonTree.AddChild(leftMultiplyOperation, left3Value);
            var right5Value = new SimpleTreeNode<ANode>(new ANode { token_type = TokenType.Integer, token_value = "5" }, leftMultiplyOperation);
            ethalonTree.AddChild(leftMultiplyOperation, right5Value);

            var actualTreeList = actualTree.GetAllNodes();
            var ethalonTreeList = ethalonTree.GetAllNodes();

            Assert.AreEqual(ethalonTreeList.Count, actualTreeList.Count);
            for(var i = 0; i < actualTreeList.Count; i++)
            {
                Assert.AreEqual(ethalonTreeList[i].NodeValue.token_type, actualTreeList[i].NodeValue.token_type);
                Assert.AreEqual(ethalonTreeList[i].NodeValue.token_value, actualTreeList[i].NodeValue.token_value);
                if (ethalonTreeList[i].Children == null) Assert.IsNull(actualTreeList[i].Children);
                else
                {
                    Assert.AreEqual(ethalonTreeList[i].Children.Count, actualTreeList[i].Children.Count);
                    Assert.AreEqual(ethalonTreeList[i].Children.Count, 2);
                    Assert.AreEqual(actualTreeList[i].Children.Count, 2);
                }
            }

            var resultNode = AST.InterpretAndTranslate(actualTree);
            Assert.AreEqual(TokenType.Integer, resultNode.token_type);
            Assert.AreEqual("20", resultNode.token_value);
            Assert.AreEqual("(7+((3*5)-2))", resultNode.translated_result);
        }
    }
}