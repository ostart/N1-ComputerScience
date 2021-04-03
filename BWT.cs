using System;
using System.Linq;
using System.Collections.Generic;

namespace N1_ComputerScience
{
    public class BWT
    {
        public BWTNode RootOfDescendingTree {get;set;}
        public List<BWTNode> DescendingTree {get;set;}
        public BWTNode RootOfAscendingTree {get;set;}
        public List<BWTNode> AscendingTree {get;set;}
        private Dictionary<BWTNode, List<BWTNode>> _LeafBindingStructure {get;set;}

        private Random _rnd = new Random();
        
        private int _levels;
        private List<int> _colors;

        public BWT(int levels, int colors, bool isSymmetric, int threshold = 1)
        {
            if (colors < 4) throw new InvalidOperationException("Need more than 3 colors");
            _levels = levels;
            _colors = Enumerable.Range(1, colors).ToList();
            DescendingTree = new List<BWTNode>();
            AscendingTree = new List<BWTNode>();
            for (int i = 0; i < (int)Math.Pow(2, levels + 1) - 1; i++)
            {
                DescendingTree.Add(null);
                AscendingTree.Add(null);
            }

            RootOfDescendingTree = GenerateNode(null, null, false, 0, true);
            Colorize(RootOfDescendingTree);
            var shift = (int)Math.Pow(2, levels + 1) - 1;
            RootOfAscendingTree = GenerateNode(null, null, false, 0, false, shift);
            if (isSymmetric) Colorize(DescendingTree, AscendingTree);
            else Colorize(RootOfAscendingTree);

            var leafsNumber = (int)Math.Pow(2, levels);
            var descLeafs = DescendingTree.GetRange(DescendingTree.Count - leafsNumber, leafsNumber);
            var ascLeafs = AscendingTree.GetRange(AscendingTree.Count - leafsNumber, leafsNumber);
            _LeafBindingStructure = new Dictionary<BWTNode, List<BWTNode>>();
            for (int i = 0; i < descLeafs.Count; i++)
            {
                _LeafBindingStructure.Add(descLeafs[i], new List<BWTNode> {null, null});
                _LeafBindingStructure.Add(ascLeafs[i], new List<BWTNode> {null, null});
            }
            BindLeafs(descLeafs,ascLeafs);
            ColorizeLeafs(descLeafs, ascLeafs);
        }

        public int QuantumModeling(int startIndex, int stopIndex)
        {
            BWTNode currentNode = null;
            currentNode = DescendingTree.Find(x => x.Index == startIndex);
            if (currentNode == null) currentNode = AscendingTree.Find(x => x.Index == startIndex);
            if (currentNode == null) throw new NullReferenceException($"There is no a node with index {startIndex}");

            var currentNodes = new List<BWTNode> { currentNode };
            var counter = 0;
            while(true)
            {
                counter += 1;
                var newCurrentNodes = new List<BWTNode>();
                foreach (var node in currentNodes)
                {
                    var nodeDoubling1 = RandomStep(node);
                    if (nodeDoubling1.Index == stopIndex) return counter;
                    if (!newCurrentNodes.Contains(nodeDoubling1)) newCurrentNodes.Add(nodeDoubling1);
                    var nodeDoubling2 = RandomStep(node);
                    if (nodeDoubling2.Index == stopIndex) return counter;
                    if (!newCurrentNodes.Contains(nodeDoubling2)) newCurrentNodes.Add(nodeDoubling2);
                }
                currentNodes = newCurrentNodes;
            }
        }

        public int NormalModeling(int startIndex, int stopIndex)
        {
            BWTNode currentNode = null;
            currentNode = DescendingTree.Find(x => x.Index == startIndex);
            if (currentNode == null) currentNode = AscendingTree.Find(x => x.Index == startIndex);
            if (currentNode == null) throw new NullReferenceException($"There is no a node with index {startIndex}");

            var counter = 0;
            do
            {
                currentNode = RandomStep(currentNode);
                counter += 1;
            }while(currentNode.Index != stopIndex);
            return counter;
        }

        private BWTNode RandomStep(BWTNode currentNode)
        {
            var direction = _rnd.Next(3);
            switch (direction)
            {
                case 0:
                    var parent = currentNode.Parent;
                    return parent == null ? currentNode : parent;
                case 1:
                    return currentNode.LeftChild;
                case 2:
                    return currentNode.RightChild;
                default:
                    throw new Exception("Incorrect direction");
            }
        }

        public List<BWTNode> FindPath(int color, FindCriterion findCriterion)
        {
            var leafsNumber = (int)Math.Pow(2, _levels);
            var descLeafs = DescendingTree.GetRange(DescendingTree.Count - leafsNumber, leafsNumber);
            var ascLeafs = AscendingTree.GetRange(AscendingTree.Count - leafsNumber, leafsNumber);
            var descLeafColorCountGoToTheTop = LeafColorCountDictionaryGoToTheTop(descLeafs, color);
            var ascLeafColorCountGoToTheTop = LeafColorCountDictionaryGoToTheTop(ascLeafs, color);

            var bridges = new List<List<BWTNode>>();
            foreach (var node in descLeafs)
                BuildBridges(node, ascLeafs, visited: new List<BWTNode>(), currentPath: new List<BWTNode>(), bridges);

            var result = new Dictionary<int, List<BWTNode>>();
            foreach (var bridge in bridges)
            {
                var counter = 0;
                for (int i = 0; i < bridge.Count; i++)
                {
                    if (i != 0)
                    {
                        if (i == bridge.Count - 1) counter += ascLeafColorCountGoToTheTop[bridge[bridge.Count - 1]];
                        else
                        {
                            if (bridge[i].LeftChild.Equals(bridge[i - 1]))
                            {
                                if (bridge[i].LeftChildColor == color) counter += bridge[i].LeftChildColor;
                            }
                            if (bridge[i].RightChild.Equals(bridge[i - 1]))
                            {
                                if (bridge[i].RightChildColor == color) counter += bridge[i].RightChildColor;
                            }
                        }
                    }
                    else counter += descLeafColorCountGoToTheTop[bridge[0]];
                }
                if (!result.Keys.Contains(counter)) result.Add(counter, bridge);
            }
            if (findCriterion == FindCriterion.MaxBranchesOfColor)
                return BuildPath(result[result.Keys.Max()]);
            else
                return BuildPath(result[result.Keys.Min()]);
        }

        private List<BWTNode> BuildPath(List<BWTNode> bridge)
        {
            var resultPath = new List<BWTNode>();
            List<BWTNode> descPart = BuildPathToTop(bridge[0].Parent, new List<BWTNode>());
            descPart.Reverse();
            resultPath.AddRange(descPart);
            resultPath.AddRange(bridge);
            var ascPart = BuildPathToTop(bridge[bridge.Count - 1].Parent, new List<BWTNode>());
            resultPath.AddRange(ascPart);
            return resultPath;
        }

        private List<BWTNode> BuildPathToTop(BWTNode node, List<BWTNode> currentPath)
        {
            currentPath.Add(node);
            if (node.Parent == null) return currentPath;
            return BuildPathToTop(node.Parent, currentPath);
        }

        private void BuildBridges(BWTNode node, List<BWTNode> ascLeafs, List<BWTNode> visited, List<BWTNode> currentPath, List<List<BWTNode>> bridges)
        {
            visited.Add(node);
            currentPath.Add(node);
            if(ascLeafs.Contains(node))
            {
                var currentPathClone = currentPath.Select(x => x.Clone()).ToList();
                bridges.Add(currentPathClone);
            }
            if (!visited.Contains(node.LeftChild)) BuildBridges(node.LeftChild, ascLeafs, visited, currentPath, bridges);
            if (!visited.Contains(node.RightChild)) BuildBridges(node.RightChild, ascLeafs, visited, currentPath, bridges);
        }

        private Dictionary<BWTNode, int> LeafColorCountDictionaryGoToTheTop(List<BWTNode> leafNodes, int color)
        {
            var result = new Dictionary<BWTNode, int>();
            foreach (var node in leafNodes)
            {
                var count = CountColorByGoToTheTop(node, color);
                result.Add(node, count);
            }
            return result;
        }

        private int CountColorByGoToTheTop(BWTNode node, int color, int counter = 0)
        {
            if (node.Parent == null)
            {
                if (node.ParentColor == color) return counter + 1;
                else return counter;
            }
            if (node.ParentColor == color) return CountColorByGoToTheTop(node.Parent, color, counter + 1);
            else return CountColorByGoToTheTop(node.Parent, color, counter);
        }

        public void ConsoleWriteLines()
        {
            var shift = 0;
            for (int i = 0; i <= _levels; i++)
            {
                var nodesNumber = (int)Math.Pow(2, i);
                var nodesInLine = DescendingTree.GetRange(shift, nodesNumber);
                shift += nodesNumber;
                WriteParentColors(nodesInLine);
                WriteIndexes(nodesInLine);
                if (i == _levels) WriteChildColors(nodesInLine);
            }

            for (int i = _levels; i >= 0; i--)
            {
                var nodesNumber = (int)Math.Pow(2, i);
                shift -= nodesNumber;
                var nodesInLine = AscendingTree.GetRange(shift, nodesNumber);
                if (i == _levels) WriteChildColors(nodesInLine, true);
                WriteIndexes(nodesInLine, true);
                WriteParentColors(nodesInLine, true);
            }
        }

        private void WriteChildColors(List<BWTNode> nodesInLine, bool isUpward = false)
        {
            if (isUpward)
            {
                for (var i = nodesInLine.Count - 1; i >= 0; i--)
                {
                    var node = nodesInLine[i];
                    Console.ForegroundColor = (ConsoleColor)node.RightChildColor;
                    Console.Write(" |");
                    Console.ForegroundColor = (ConsoleColor)node.LeftChildColor;
                    Console.Write(" |");
                }
            }
            else
            {
                foreach (var node in nodesInLine)
                {
                    Console.ForegroundColor = (ConsoleColor)node.LeftChildColor;
                    Console.Write(" |");
                    Console.ForegroundColor = (ConsoleColor)node.RightChildColor;
                    Console.Write(" |");
                }
            }

            Console.ResetColor();
            Console.WriteLine(" ");
        }

        private void WriteIndexes(List<BWTNode> nodesInLine, bool isUpward = false)
        {
            var indexes = nodesInLine.Select(x => x.Index).ToList();
            if (isUpward) indexes.Reverse();
            var str = string.Join(" ", indexes);
            Console.WriteLine($" {str} ");
        }

        private void WriteParentColors(List<BWTNode> nodesInLine, bool isUpward = false)
        {
            if (isUpward)
            {
                for (var i = nodesInLine.Count - 1; i >= 0; i--)
                {
                    var node = nodesInLine[i];
                    Console.ForegroundColor = (ConsoleColor)node.ParentColor;
                    Console.Write(" |");
                }
            }
            else
            {
                foreach (var node in nodesInLine)
                {
                    Console.ForegroundColor = (ConsoleColor)node.ParentColor;
                    Console.Write(" |");
                }
            }
            
            Console.ResetColor();
            Console.WriteLine(" ");
        }

        private void ColorizeLeafs(List<BWTNode> descLeafs, List<BWTNode> ascLeafs)
        {
            var index = GetLeftColorIndexForLevel(descLeafs[0].Level);

            foreach (var descLeaf in descLeafs)
            {
                var relatedNodes = _LeafBindingStructure[descLeaf];
                descLeaf.LeftChild = relatedNodes[0];
                descLeaf.LeftChildColor = _colors[index];
                descLeaf.RightChild = relatedNodes[1];
                descLeaf.RightChildColor = _colors[index + 1];
            }
            foreach (var ascLeaf in ascLeafs)
            {
                var relatedNodes = _LeafBindingStructure[ascLeaf];
                ascLeaf.LeftChild = relatedNodes[0];
                ascLeaf.LeftChildColor = _colors[index];
                ascLeaf.RightChild = relatedNodes[1];
                ascLeaf.RightChildColor = _colors[index + 1];
            }
        }

        private void BindLeafs(List<BWTNode> descLeafs, List<BWTNode> ascLeafs)
        {
            foreach (var node in descLeafs)
            {
                if (_LeafBindingStructure[node][0] != null && _LeafBindingStructure[node][1] != null) continue;

                foreach (var possibleLinkedNode in ascLeafs)
                {
                    if (_LeafBindingStructure[node][0] == null && _LeafBindingStructure[possibleLinkedNode][0] == null)
                    {
                        _LeafBindingStructure[node][0] = possibleLinkedNode;
                        _LeafBindingStructure[possibleLinkedNode][0] = node;
                        continue;
                    }
                    if (_LeafBindingStructure[node][1] == null && _LeafBindingStructure[possibleLinkedNode][1] == null)
                    {
                        _LeafBindingStructure[node][1] = possibleLinkedNode;
                        _LeafBindingStructure[possibleLinkedNode][1] = node;
                        continue;
                    }
                }
            }

            foreach (var node in ascLeafs)
            {
                if (_LeafBindingStructure[node][0] != null && _LeafBindingStructure[node][1] != null) continue;

                foreach (var possibleLinkedNode in descLeafs)
                {
                    if (_LeafBindingStructure[node][0] == null && _LeafBindingStructure[possibleLinkedNode][0] == null)
                    {
                        _LeafBindingStructure[node][0] = possibleLinkedNode;
                        _LeafBindingStructure[possibleLinkedNode][0] = node;
                        continue;
                    }
                    if (_LeafBindingStructure[node][1] == null && _LeafBindingStructure[possibleLinkedNode][1] == null)
                    {
                        _LeafBindingStructure[node][1] = possibleLinkedNode;
                        _LeafBindingStructure[possibleLinkedNode][1] = node;
                        continue;
                    }
                }
            }
        }

        private void Colorize(List<BWTNode> descendingTree, List<BWTNode> ascendingTree)
        {
            var colorList = descendingTree.Select(x => x.ParentColor).ToList();
            var ascendingParentColors = new List<int>();
            var shift = 0;
            for (int i = 0; i <= _levels; i++)
            {
                var count = (int)Math.Pow(2, i);
                var elements = colorList.GetRange(shift, count);
                shift += count;
                elements.Reverse();
                foreach (var elem in elements)    
                    ascendingParentColors.Add(elem);
            }
            for (int i = 0; i < ascendingParentColors.Count; i++)
            {
                ascendingTree[i].ParentColor = ascendingParentColors[i];
                if (i == 0) continue;
                if (i % 2 == 0) //right
                {
                    var index = (i - 2) / 2;
                    ascendingTree[index].RightChildColor = ascendingParentColors[i];
                }
                else //left
                {
                    var index = (i - 1) / 2;
                    ascendingTree[index].LeftChildColor = ascendingParentColors[i];
                }
            }
        }

        private void Colorize(BWTNode node)
        {
            if (node.LeftChild == null && node.RightChild == null) return;

            var index = GetLeftColorIndexForLevel(node.Level);

            node.LeftChildColor = _colors[index];
            node.LeftChild.ParentColor = _colors[index];
            node.RightChildColor = _colors[index + 1];
            node.RightChild.ParentColor = _colors[index + 1];
            Colorize(node.LeftChild);
            Colorize(node.RightChild);
        }

        private int GetLeftColorIndexForLevel(int level)
        {
            var baseColors = _colors.Count;
            if (_colors.Count % 2 == 1) baseColors -= 1;
            var index = (level * 2) % baseColors;
            return index;
        }

        private BWTNode GenerateNode(int? parentIndex, BWTNode parentNode, bool isRight, int level, bool isDescending, int shift = 0, int threshold = 1)
        {
            if(parentIndex == null)
            {
                var root = new BWTNode(0, null, 0, shift, threshold);
                if (isDescending) DescendingTree[0] = root;
                else AscendingTree[0] = root;
                root.ParentColor = 0;
                root.LeftChild = GenerateNode(0, root, false, level+1, isDescending, shift);
                root.RightChild = GenerateNode(0, root, true, level+1, isDescending, shift);
                return root;
            }

            if(level > _levels) return null;

            var currentIndex = isRight ? 2*(int)parentIndex + 2 : 2*(int)parentIndex + 1; 
            var node = new BWTNode(currentIndex, parentNode, level, shift, threshold);
            if (isDescending) DescendingTree[currentIndex] = node;
            else AscendingTree[currentIndex] = node;
            node.LeftChild = GenerateNode(currentIndex, node, false, level+1, isDescending, shift);
            node.RightChild = GenerateNode(currentIndex, node, true, level+1, isDescending, shift);
            return node;
        }
    }

    public class BWTNode: IEquatable<BWTNode>
    {
        public int Index {get;set;}
        public int Level {get;set;}

        public BWTNode Parent {get;set;}
        public int ParentColor {get;set;}

        public BWTNode LeftChild {get;set;}
        public int LeftChildColor {get;set;}

        public BWTNode RightChild {get;set;}
        public int RightChildColor {get;set;}

        public BWTNode(int index, BWTNode parent, int level, int shift, int threshold)
        {
            Index = index + shift + threshold;
            Parent = parent;
            Level = level;
        }

        public BWTNode Clone()
        {
            var clone = new BWTNode(Index, Parent, Level, 0, 0);
            clone.ParentColor = ParentColor;
            clone.LeftChild = LeftChild;
            clone.LeftChildColor = LeftChildColor;
            clone.RightChild = RightChild;
            clone.RightChildColor = RightChildColor;
            return clone;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as BWTNode;
            if( other == null ) return false;

            return Equals (other);             
        }

        public override int GetHashCode()
        {
            return Index ^ Level ^ ParentColor;
        }

        public bool Equals(BWTNode other)
        {
            if(other == null) return false;
            if(ReferenceEquals (this, other)) return true;
            return this.Index == other.Index && this.Level == other.Level && this.ParentColor == other.ParentColor;
        }

        // public class EqualityComparer: IEqualityComparer<BWTNode>
        // {
        //     public bool Equals(BWTNode x, BWTNode y) 
        //     {
        //         return x.Index == y.Index && x.Level == y.Level && x.ParentColor == y.ParentColor;
        //     }

        //     public int GetHashCode(BWTNode x) 
        //     {
        //         return x.Index ^ x.Level ^ x.ParentColor;
        //     }

        // }
    }

    public enum FindCriterion
    {
        MinBranchesOfColor,
        MaxBranchesOfColor,
    }
}