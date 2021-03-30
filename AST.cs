using System;
using System.Collections.Generic;

namespace N1_ComputerScience
{
    public class AST
    {
        public static List<ANode> Parse(string input)
        {
            var preparedInput = AddBrackets(input);
            return GenerateTokens(preparedInput);
        }

        public static SimpleTree<ANode> Build(List<ANode> nodes)
        {
            var currentNode = new SimpleTreeNode<ANode>(null, null);
            var result = new SimpleTree<ANode>(currentNode);
            foreach (var node in nodes)
            {
                if (node.token_type == TokenType.Bracket && node.token_value == "(")
                {
                    var leftEmpty = new SimpleTreeNode<ANode>(null, currentNode);
                    result.AddChild(currentNode, leftEmpty);
                    currentNode = leftEmpty;
                }
                if (node.token_type == TokenType.Bracket && node.token_value == ")")
                {
                    currentNode = currentNode.Parent;
                }
                if (node.token_type == TokenType.Integer)
                {
                    currentNode.NodeValue = node;
                    currentNode = currentNode.Parent;
                }
                if (node.token_type == TokenType.Operation)
                {
                    currentNode.NodeValue = node;
                    var rightEmpty = new SimpleTreeNode<ANode>(null, currentNode);
                    result.AddChild(currentNode, rightEmpty);
                    currentNode = rightEmpty;
                }
            }
            return result;
        }

        public static ANode InterpretAndTranslate(SimpleTree<ANode> ast)
        {
            if (ast.Root.NodeValue.token_type == TokenType.Integer) return ast.Root.NodeValue;

            var currentNode = GetCurrentNodeForInterpretAndTranslate(ast.Root);
            string newValue = CalculateValueForInterpretAndTranslate(currentNode);
            string newTranslatedResult = TranslateResult(currentNode);
            currentNode.NodeValue.token_type = TokenType.Integer;
            currentNode.NodeValue.token_value = newValue;
            currentNode.NodeValue.translated_result = newTranslatedResult;

            return InterpretAndTranslate(ast);
        }

        private static SimpleTreeNode<ANode> GetCurrentNodeForInterpretAndTranslate(SimpleTreeNode<ANode> node)
        {
            if (node.Children[0].NodeValue.token_type == TokenType.Integer && node.Children[1].NodeValue.token_type == TokenType.Integer) return node;
            if (node.Children[0].NodeValue.token_type == TokenType.Operation) return GetCurrentNodeForInterpretAndTranslate(node.Children[0]);
            if (node.Children[1].NodeValue.token_type == TokenType.Operation) return GetCurrentNodeForInterpretAndTranslate(node.Children[1]);
            return null;
        }

        private static string CalculateValueForInterpretAndTranslate(SimpleTreeNode<ANode> currentNode)
        {
            var arg1 = int.Parse(currentNode.Children[0].NodeValue.token_value);
            var arg2 = int.Parse(currentNode.Children[1].NodeValue.token_value);
            var result = 0;
            switch (currentNode.NodeValue.token_value)
            {
                case "/":
                    result = arg1 / arg2;
                    break;
                case "*":
                    result = arg1 * arg2;
                    break;
                case "-":
                    result = arg1 - arg2;
                    break;
                case "+":
                    result = arg1 + arg2;
                    break;
                default:
                    throw new Exception();
            }
            return result.ToString();
        }

        private static string TranslateResult(SimpleTreeNode<ANode> currentNode)
        {
            var arg1 = string.IsNullOrEmpty(currentNode.Children[0].NodeValue.translated_result) ? currentNode.Children[0].NodeValue.token_value : currentNode.Children[0].NodeValue.translated_result;
            var arg2 = string.IsNullOrEmpty(currentNode.Children[1].NodeValue.translated_result) ? currentNode.Children[1].NodeValue.token_value : currentNode.Children[1].NodeValue.translated_result;
            return $"({arg1}{currentNode.NodeValue.token_value}{arg2})";
        }

        private static List<ANode> GenerateTokens(string preparedInput)
        {
            var result = new List<ANode>();
            var strValue = string.Empty;
            var isDigit = false;
            foreach (var letter in preparedInput)
            {
                if (Char.IsDigit(letter))
                {
                    if (!isDigit) isDigit = true;
                    strValue += letter.ToString();
                }
                else
                {
                    if (isDigit && !string.IsNullOrEmpty(strValue))
                    {
                        result.Add(new ANode { token_type = TokenType.Integer, token_value = strValue });
                        isDigit = false;
                        strValue = string.Empty;
                    }
                    if (letter == '(' || letter == ')')
                    {
                        result.Add(new ANode { token_type = TokenType.Bracket, token_value = letter.ToString() });
                    }
                    else
                    {
                        result.Add(new ANode { token_type = TokenType.Operation, token_value = letter.ToString() });
                    }
                }
            }
            return result;
        }

        private static string AddBrackets(string input)
        {
            if (IsBalancedBrackets('/', input) && IsBalancedBrackets('*', input) && IsBalancedBrackets('-', input) && IsBalancedBrackets('+', input)) return input;
            var str = input;
            if (!IsBalancedBrackets('/', str)) str = BalanceBrackets('/', str);
            if (!IsBalancedBrackets('*', str)) str = BalanceBrackets('*', str);
            if (!IsBalancedBrackets('-', str)) str = BalanceBrackets('-', str);
            if (!IsBalancedBrackets('+', str)) str = BalanceBrackets('+', str);
            return AddBrackets(str);
        }

        private static string BalanceBrackets(char sign, string str)
        {
            var result = str;
            var counter = 0;
            while(result.IndexOf(sign, counter) != -1)
            {
                var index = result.IndexOf(sign, counter);
                var beginOfLeftArg = GetBeginIndexOfLeftArg(index, result);
                result = result.Insert(beginOfLeftArg, "(");
                var endOfRightArg = GetEndIndexOfRightArg(index + 1, result);
                result = result.Insert(endOfRightArg + 1, ")");
                counter = result.IndexOf(sign, counter) + 2;
            }
            return result;
        }

        private static bool IsBalancedBrackets(char sign, string str)
        {
            var indexes = FindIndexes(sign, str);
            foreach (var index in indexes)
            {
                var endOfRightArg = GetEndIndexOfRightArg(index, str);
                var beginOfLeftArg = GetBeginIndexOfLeftArg(index, str);
                try
                {
                    if (str[beginOfLeftArg - 1] != '(' || str[endOfRightArg + 1] != ')') return false;
                }
                catch (System.Exception)
                {
                    return false;
                }   
            }
            return true;
        }

        private static int GetBeginIndexOfLeftArg(int index, string str)
        {
            var nearestIsBracket = str[index - 1] == ')';
            var bracketClosed = 1;
            var nearestIsDigit = Char.IsDigit(str[index - 1]);
            var counter = 2;
            while(index - counter >= 0)
            {
                if (nearestIsBracket && str[index - counter] == '(') return index - counter + 1;
                if (nearestIsBracket)
                {
                    if (str[index - counter] == ')') bracketClosed += 1;
                    if (str[index - counter] == '(')
                    {
                        bracketClosed -= 1;
                        if (bracketClosed == 0) return index - counter;
                    }
                }
                if (nearestIsDigit && !Char.IsDigit(str[index - counter])) return index - counter + 1;
                counter += 1;
            }
            return 0;
        }

        private static int GetEndIndexOfRightArg(int index, string str)
        {
            var nearestIsBracket = str[index + 1] == '(';
            var bracketOpened = 1;
            var nearestIsDigit = Char.IsDigit(str[index + 1]);
            var counter = 2;
            while(index + counter < str.Length)
            {
                if (nearestIsBracket)
                {
                    if (str[index + counter] == '(') bracketOpened += 1;
                    if (str[index + counter] == ')')
                    {
                        bracketOpened -= 1;
                        if (bracketOpened == 0) return index + counter;
                    }
                } 
                if (nearestIsDigit && !Char.IsDigit(str[index + counter])) return index + counter - 1;
                counter += 1;
            }
            return str.Length - 1;
        }

        private static List<int> FindIndexes(char sign, string str)
        {
            var counter = 0;
            var result = new List<int>();
            while(str.IndexOf(sign,counter) != -1)
            {
                result.Add(str.IndexOf(sign,counter));
                counter = str.IndexOf(sign,counter) + 1;
            }
            return result;
        }
    }

    public class ANode
    {
        public TokenType token_type {get;set;}
        public string token_value {get;set;}
        public string translated_result {get;set;}
    }

    public enum TokenType
    {
        Bracket,
        Operation,
        Integer
    }

    public class SimpleTreeNode<T>
    {
        public T NodeValue; // значение в узле
        public SimpleTreeNode<T> Parent; // родитель или null для корня
        public List<SimpleTreeNode<T>> Children; // список дочерних узлов или null

        public SimpleTreeNode(T val, SimpleTreeNode<T> parent)
        {
            NodeValue = val;
            Parent = parent;
            Children = null;
        }
    }

    public class SimpleTree<T>
    {
        public SimpleTreeNode<T> Root; // корень, может быть null

        public SimpleTree(SimpleTreeNode<T> root)
        {
            Root = root;
        }

        public void AddChild(SimpleTreeNode<T> ParentNode, SimpleTreeNode<T> NewChild)
        {
            // ваш код добавления нового дочернего узла существующему ParentNode
            NewChild.Parent = ParentNode;
            if (ParentNode.Children == null)
                ParentNode.Children = new List<SimpleTreeNode<T>>();
            ParentNode.Children.Add(NewChild);
        }

        public List<SimpleTreeNode<T>> GetAllNodes()
        {
            // ваш код выдачи всех узлов дерева в определённом порядке
            var result = new List<SimpleTreeNode<T>>();
            FillResult(Root, result);
            return result;
        }

        private static void FillResult(SimpleTreeNode<T> node, List<SimpleTreeNode<T>> result)
        {
            result.Add(node);
            if (node.Children == null) return;
            foreach (var nodeChild in node.Children)
            {
                FillResult(nodeChild, result);
            }
        }
    }
}