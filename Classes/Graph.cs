using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class Graph
    {

        private int[,] connectionMatrix = new int[0, 0];
        private int nodeCount;
        private string[] labyrinthConnections = [];

        public Graph(int nodeCount, bool forLabyrinth = false)
        {

            if (nodeCount <= 0)
            {
                Console.WriteLine($"Cannot create graph with {nodeCount} nodes!");
                return;
            }

            this.nodeCount = nodeCount;
            connectionMatrix = new int[nodeCount + 1, nodeCount + 1];

            for (int i = 1; i < nodeCount + 1; i++)
            {
                connectionMatrix[i, 0] = i;
                connectionMatrix[0, i] = i;
            }

            if (forLabyrinth)
            {
                int offset = (int)Math.Sqrt(nodeCount);
                int[] possibleConnections = [];
                for (int i = 1; i < nodeCount + 1; i++)
                {
                    if ((i - 1) % offset == 0)
                    {
                        possibleConnections = [1, offset, -offset];
                    }
                    else if (i % offset == 0)
                    {
                        possibleConnections = [-1, offset, -offset];
                    }
                    else
                    {
                        possibleConnections = [1, -1, offset, -offset];
                    }
                    for (int j = 0; j < possibleConnections.Length; j++)
                    {
                        if (i + possibleConnections[j] > 0 && i + possibleConnections[j] <= nodeCount)
                        {
                            connectionMatrix[i, i + possibleConnections[j]] = 1;
                        }
                    }
                }
            }

        }

        public void AddConnection(int node, string connections)
        {
            string[] connectionsArray = connections.Split(' ');
            for (int i = 0; i < connectionsArray.Length; i++)
            {
                if (connectionsArray[i].Trim() == "") continue;
                connectionMatrix[node, int.Parse(connectionsArray[i])] = 1;
            }
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < nodeCount + 1; i++)
            {
                for (int j = 0; j < nodeCount + 1; j++)
                {
                    Console.Write($"{connectionMatrix[i, j]}\t");
                }
                Console.WriteLine();
            }
        }

        public int BreadthSearch(int startNode)
        {
            Queue<int> queue = new();
            queue.Enqueue(startNode);
            List<int> visited = [];
            Tree tree = new(startNode);

            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                if (!visited.Contains(node)) visited.Add(node);
                for (int i = 1; i < nodeCount + 1; i++)
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
            labyrinthConnections = tree.GetTreeWithConnections();

            return visited.Count;
        }

        public int DefthSearch(int startNode)
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
                for (int i = 1; i < nodeCount + 1; i++)
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

            return visited.Count;
        }

        public string[] GenerateLabyrinth(int startNode)
        {
            List<int> visitedNodes = [startNode];
            List<int> availableNodes = [];
            Tree tree = new(startNode);
            for (int i = 1; i < nodeCount + 1; i++)
            {
                if (connectionMatrix[startNode, i] == 1) availableNodes.Add(i);
            }

            Random random = new();
            while (visitedNodes.Count != nodeCount)
            {

                int next = availableNodes[random.Next(0, availableNodes.Count)];

                for (int i = 0; i < visitedNodes.Count; i++)
                {
                    if (connectionMatrix[visitedNodes[i], next] == 1)
                    {
                        tree.AddNode(visitedNodes[i], next);
                        break;
                    }
                }

                visitedNodes.Add(next);
                for (int i = 1; i < nodeCount + 1; i++)
                {
                    if (connectionMatrix[next, i] == 1 &&
                        !visitedNodes.Contains(i) &&
                        !availableNodes.Contains(i)) availableNodes.Add(i);
                }
                availableNodes.Remove(next);
            }

            return tree.GetTreeWithConnections();

        }

        public string CheckConnectivity()
        {
            if (BreadthSearch(1) == nodeCount)
                return "Graph is connected!";
            else
                return "Graph is not connected!";
        }
    }
}