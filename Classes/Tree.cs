using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class Tree(int name)
    {
        private class Node
        {
            public int name;
            public List<Node> children = [];

            public Node(int name)
            {
                this.name = name;
            }
        }

        private Node head = new Node(name);
        private int nodeCount = 1;

        public void AddNode(int parentName, int nodeName)
        {

            Node node = SearchNode(parentName, head);
            node.children.Add(new Node(nodeName));
            nodeCount++;
        }

        private Node SearchNode(int nodeName, Node current)
        {
            if (current.name == nodeName)
            {
                return current;
            }

            foreach (var child in current.children)
            {
                Node searchResult = SearchNode(nodeName, child);
                if (searchResult != null)
                {
                    return searchResult;
                }
            }

            return null!;
        }

        public int GetNodeNumber()
        {
            return nodeCount;
        }

        string[] treeWithConnections = [];
        public string[] GetTreeWithConnections()
        {
            treeWithConnections = new string[nodeCount];
            GetTreeWithConnectionsRecursive(head);
            return treeWithConnections;
        }

        private void GetTreeWithConnectionsRecursive(Node root)
        {
            if (root.children.Count > 0)
                treeWithConnections[root.name - 1] = string.Join(' ', root.children.Select(child => child.name));
            foreach (var child in root.children)
            {
                GetTreeWithConnectionsRecursive(child);
            }
        }

        string result = "";
        public string Print()
        {
            PrintTree(head);
            return result;
        }

        private void PrintTree(Node root)
        {
            if (root.children.Count > 0) result += $"{root.name} -> {string.Join(',', root.children.Select(child => child.name))}\n";
            foreach (var child in root.children)
            {
                PrintTree(child);
            }
        }
    }
}
