using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class CrossingTaskGraph
    {
        private struct Node
        {
            public int number;
            public string objects;
        }

        private int[,] connectionMatrix = new int[0, 0];
        private List<Node> headers = [];
        private List<string> combinations = [];
        private int nodeCount;

        private Dictionary<char, int> elements = new()
        {
            {'P', 1},
            {'G', 2},
            {'C', 4},
            {'W', 8},
            {'D', 16 }
        };

        private string[] invalidCombinations;
        public CrossingTaskGraph(string objects = "PGCW", int numberOfSeats = 2, string[]? invalid = null)
        {

            invalidCombinations = invalid ?? (["GC", "WG", "GDW"]);

            combinations.Add(objects);
            combinations.Add("");
            SetHeaders(objects, 0);

            foreach (var comb in combinations)
            {
                int sum = 0;
                for (int i = 0; i < comb.Length; i++)
                {
                    sum += elements[comb[i]];
                }
                if (sum % 2 == 1 || sum == 0) headers.Add(new Node { objects = comb, number = sum });
            }
            headers.Sort((l, r) => l.number.CompareTo(r.number));
            headers = headers.DistinctBy(el => el.number).ToList();

            for (int i = 0; i < headers.Count; i++)
            {
                Node temp = headers[i];
                temp.number = i;
                headers[i] = temp;
            }

            nodeCount = headers.Count;
            connectionMatrix = new int[nodeCount, nodeCount];
            FillMatrix(numberOfSeats);
        }

        private void SetHeaders(string input, int startIndex)
        {
            if (input.Length == 2) return;
            for (int i = startIndex; i < input.Length; i++)
            {
                string editedInput = input.Remove(i, 1);
                combinations.Add(editedInput);
                SetHeaders(editedInput, i);
            }
        }

        private void FillMatrix(int boatSeats)
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (i == j || !headers[j].objects.Contains('P') || !headers[i].objects.Contains('P')) continue;
                    if (headers[i].objects.Length <= boatSeats)
                    {
                        connectionMatrix[i, 0] = 1;
                        break;
                    }

                    if (IsValid(headers[i].objects, headers[j].objects, boatSeats)
                        && IsTransitionAvailable(headers[i].objects, headers[j].objects))
                    {
                        connectionMatrix[i, j] = 1;
                    }
                }
            }
        }

        private bool IsTransitionAvailable(string firstComb, string secondComb)
        {
            string rightCoastAfterTransition = Difference(headers[^1].objects, secondComb);
            string leftCoastWithoutPassenger = Difference(firstComb, rightCoastAfterTransition);
            if (IsInvalidCombination(rightCoastAfterTransition) ||
                IsInvalidCombination(leftCoastWithoutPassenger.Remove(leftCoastWithoutPassenger.IndexOf('P'), 1)))
                return false;
            return true;
        }

        private string Difference(string left, string right)
        {
            string result = "";
            string leftCopy = left;
            string rightCopy = right;
            for (int i = 0; i < leftCopy.Length; i++)
            {
                if (headers[^1].objects.Contains(leftCopy[i]) &&
                    headers[^1].objects.Count(ch => ch == leftCopy[i]) != leftCopy.Count(ch => ch == leftCopy[i]))
                {
                    leftCopy += leftCopy[i];
                }
            }
            for (int i = 0; i < leftCopy.Length; i++)
            {
                if (!rightCopy.Contains(leftCopy[i]))
                {
                    result += leftCopy[i];

                }
                else
                {
                    rightCopy = rightCopy.Remove(rightCopy.IndexOf(leftCopy[i]), 1);
                }

            }

            return result;
        }

        private bool IsValid(string firstComb, string secondComb, int boatSeats)
        {
            if (firstComb.Length < secondComb.Length) (secondComb, firstComb) = (firstComb, secondComb);
            if (firstComb.Length - secondComb.Length > boatSeats - 1) return false;

            int similarNumber = 0;
            for (int i = 0; i < secondComb.Length; i++)
            {
                if (firstComb.Contains(secondComb[i])) similarNumber++;
            }
            if (firstComb.Length - similarNumber <= boatSeats - 1) return true;
            return false;
        }

        private bool IsInvalidCombination(string inputComb)
        {
            if (inputComb.Contains('P')) return false;
            if (ContainsInvalid(inputComb)) return true;
            return false;
        }

        private bool ContainsInvalid(string inputComb)
        {
            foreach (var comb in invalidCombinations)
            {
                int number = 0;
                for (int i = 0; i < comb.Length; i++)
                {
                    if (inputComb.Contains(comb[i]))
                    {
                        number++;
                    }
                }
                if (number == comb.Length) return true;

            }
            return false;
        }

        public string BreadthSearch(int startNode)
        {
            Queue<int> queue = new();
            queue.Enqueue(startNode);
            List<int> visited = [];
            Tree tree = new(startNode);

            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                if (!visited.Contains(node)) visited.Add(node);
                for (int i = 0; i < nodeCount; i++)
                {
                    if (visited.Contains(i) || connectionMatrix[node, i] == 0)
                        continue;
                    else
                    {
                        queue.Enqueue(i);
                        visited.Add(i);
                        tree.AddNode(node, i);
                    }

                }
            }
            string result = "";
            foreach (var i in visited)
            {
                result += $"{headers[i].number} - {headers[i].objects}\n";
            }
            result += "----------------------------------\n";
            result += tree.Print();
            result += "----------------------------------\n";

            return result;

        }

        public string DefthSearch(int startNode)
        {
            Stack<int> stack = new();
            List<int> visited = [];
            Tree tree = new(startNode);
            List<Tree> forest = [];
            stack.Push(startNode);
            bool isconnect = false;
            while (stack.Count > 0)
            {
                int node = stack.Peek();
                if (!visited.Contains(node)) visited.Add(node);
                for (int i = 0; i < nodeCount; i++)
                {
                    if (connectionMatrix[node, i] == 1 && !visited.Contains(i))
                    {
                        isconnect = true;
                        tree.AddNode(node, i);
                        stack.Push(i);
                        break;
                    }
                }

                if (!isconnect)
                {
                    stack.Pop();
                    if (tree.GetNodeNumber() > 1) forest.Add(tree);
                    if (stack.Count > 0) tree = new(stack.Peek());
                }
                else
                    isconnect = false;

            }

            string result = "";
            foreach (var i in visited)
            {
                result += $"{headers[i].number} - {headers[i].objects}\n";
            }
            result += "----------------------------------\n";
            foreach (var tr in forest)
            {
                result += tr.Print();
                result += "----------------------------------\n";
            }
            return result;
        }

        /// <summary>
        /// Availible methods: "breadth" and "defth"
        /// </summary>
        public string SearchSolution(string way = "breadth")
        {
            switch (way)
            {
                case "breadth":
                    return BreadthSearch(nodeCount - 1);
                case "defth":
                    return DefthSearch(nodeCount - 1);
                default:
                    return "Unknown solution way!";
            }
        }
    }
}
