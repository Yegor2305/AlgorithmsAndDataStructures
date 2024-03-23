using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class WeightedGraph
    {

        struct Node
        {
            public bool isAvailable;
            public double weight;
            public int name;
        }

        private int[,] connectionMatrix = new int[0, 0];
        private double[,] floydWarshallMatrix = new double[0, 0];
        private Node[] nodes = [];
        private int nodeCount;

        public WeightedGraph(int nodeCount)
        {

            if (nodeCount <= 0)
            {
                Console.WriteLine($"Cannot create graph with {nodeCount} nodes!");
                return;
            }

            this.nodeCount = nodeCount;
            nodes = new Node[nodeCount];
            connectionMatrix = new int[nodeCount + 1, nodeCount + 1];
            floydWarshallMatrix = new double[nodeCount + 1, nodeCount + 1];
            for (int i = 1; i < nodeCount + 1; i++)
            {
                nodes[i - 1] = new Node { isAvailable = true, name = i, weight = double.PositiveInfinity };
                connectionMatrix[i, 0] = i;
                connectionMatrix[0, i] = i;
                floydWarshallMatrix[0, i] = i;
                floydWarshallMatrix[i, 0] = i;
            }

            for (int i = 1; i < nodeCount + 1; i++)
            {
                for (int j = 1; j < nodeCount + 1; j++)
                {
                    if (i == j && floydWarshallMatrix[i, j] == 0)
                    {
                        floydWarshallMatrix[i, j] = 0;
                        continue;
                    }
                    floydWarshallMatrix[i, j] = double.PositiveInfinity;
                }
            }
        }

        public void AddConnection(int node, string connections)
        {
            string[] connectionsArray = connections.Split(' ');
            for (int i = 0; i < connectionsArray.Length; i++)
            {
                if (connectionsArray[i].Trim() == "") continue;
                connectionMatrix[node, int.Parse(connectionsArray[i].Split('.')[0])] = int.Parse(connectionsArray[i].Split('.')[1]);
                floydWarshallMatrix[node, int.Parse(connectionsArray[i].Split('.')[0])] = int.Parse(connectionsArray[i].Split('.')[1]);
            }
        }

        public string Dijkstra(int startNode, int finalNode = 0)
        {
            nodes[startNode - 1].weight = 0;

            int numberOfPassedNodes = 0;
            int currentNode = startNode;
            int[] ways = new int[nodeCount];

            while (numberOfPassedNodes < nodes.Length)
            {
                int minWeightNode = int.MaxValue;
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].isAvailable && nodes[i].weight <= minWeightNode)
                    {
                        currentNode = nodes[i].name;
                        minWeightNode = (int)nodes[i].weight;
                    }
                }

                nodes[currentNode - 1].isAvailable = false;

                if (!HasConnections(currentNode))
                {
                    numberOfPassedNodes++;
                    continue;
                }

                for (int i = 1; i < nodeCount + 1; i++)
                {
                    if (connectionMatrix[currentNode, i] < 0)
                    {
                        return "Dijkstra's algorithm may not work correctly, due to " +
                            "negative weight of connection, use another algorithm!";
                    }
                    if (connectionMatrix[currentNode, i] != 0 && nodes[i - 1].isAvailable)
                    {
                        if (nodes[i - 1].weight > nodes[currentNode - 1].weight + connectionMatrix[currentNode, i])
                        {
                            nodes[i - 1].weight = nodes[currentNode - 1].weight + connectionMatrix[currentNode, i];
                            ways[i - 1] = currentNode;
                        }


                    }
                }

                numberOfPassedNodes++;
                nodes[currentNode - 1].isAvailable = false;
            }

            string result = "";
            foreach (var elem in nodes)
            {
                result += $"{elem.name} - {elem.weight}\n";
            }

            if (finalNode == 0) return result;

            List<int> waysToFinalNode = [];
            int pointerToNode = finalNode;

            while (true)
            {
                waysToFinalNode.Add(pointerToNode);
                if (ways[pointerToNode - 1] == 0)
                    break;
                else
                    pointerToNode = ways[pointerToNode - 1];
            }
            waysToFinalNode.Reverse();
            return $"{result}{string.Join(" -> ", waysToFinalNode)}";

        }

        public string FloydWarshall()
        {

            for (int k = 1; k < nodeCount + 1; k++)
            {
                for (int i = 1; i < nodeCount + 1; i++)
                {
                    for (int j = 1; j < nodeCount + 1; j++)
                    {
                        if (i == j) continue;
                        if (floydWarshallMatrix[i, k] + floydWarshallMatrix[k, j] < floydWarshallMatrix[i, j])
                        {
                            floydWarshallMatrix[i, j] = floydWarshallMatrix[i, k] + floydWarshallMatrix[k, j];
                        }
                    }
                }
            }

            string result = "";

            for (int i = 0; i < nodeCount + 1; i++)
            {
                for (int j = 0; j < nodeCount + 1; j++)
                {
                    result += $"{floydWarshallMatrix[i, j]}\t";
                }
                result += "\n";
            }

            return result;

        }

        public string BellmanFord(int startNode)
        {
            Queue<int> queue = new();
            nodes[startNode - 1].weight = 0;
            queue.Enqueue(startNode);
            int iterationsCounter = 0;

            while (queue.Count > 0)
            {
                if (iterationsCounter == nodes.Length + 1)
                    return "Graph contains negative cycles!";

                int currentNode = queue.Dequeue();
                for (int i = 1; i < nodeCount + 1; i++)
                {
                    if (connectionMatrix[currentNode, i] == 0) continue;
                    if (double.IsPositiveInfinity(nodes[i - 1].weight))
                    {
                        nodes[i - 1].weight = nodes[currentNode - 1].weight + connectionMatrix[currentNode, i];
                        if (!queue.Contains(i)) queue.Enqueue(i);
                    }
                    else if (nodes[i - 1].weight > nodes[currentNode - 1].weight + connectionMatrix[currentNode, i])
                    {
                        nodes[i - 1].weight = nodes[currentNode - 1].weight + connectionMatrix[currentNode, i];
                        if (!queue.Contains(i)) queue.Enqueue(i);
                    }
                }
                iterationsCounter++;
            }

            string result = "";
            foreach (var elem in nodes)
            {
                result += $"{elem.name} - {elem.weight}\n";
            }
            return result;
        }

        private bool HasConnections(int node)
        {
            for (int i = 1; i < nodeCount + 1; i++)
            {
                if (connectionMatrix[node, i] != 0 && nodes[i - 1].isAvailable) return true;
            }
            return false;
        }
    }
}
