using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    public struct ElementInformation
    {
        public int inventoryNumber;
        public string name;
        public string type;
        public int downtime;
        public int worktime;

        public ElementInformation()
        {
            inventoryNumber = 0;
            name = string.Empty;
            type = string.Empty;
            downtime = 0;
            worktime = 0;
        }
    }
    public class BTree
    {
        class Node
        {
            public List<ElementInformation> keys;
            public List<Node> children;
            private readonly int maxKeysSize;
            private readonly int maxChildrenSize;

            public Node(int degree)
            {
                maxKeysSize = degree - 1;
                maxChildrenSize = degree;
                keys = new();
                children = new();
            }

            public int GetIndexToGo(int value)
            {
                if (IsLeaf()) return -1;
                int index = 0;
                for (int i = 0; i < keys.Count; i++)
                {
                    if (value <= keys[i].inventoryNumber)
                        return index;
                    index++;
                }
                return index;
            }

            public bool AddKey(ElementInformation value)
            {
                if (keys.Count < maxKeysSize)
                {
                    keys.Add(value);
                    keys.Sort((x, y) => x.inventoryNumber.CompareTo(y.inventoryNumber));
                    return true;
                }
                return false;
            }

            public void AddChild(Node node = null!, ElementInformation value = default)
            {

                if (node != null) children.Add(node);
                else
                {
                    children.Add(new Node(maxChildrenSize));
                    if (!value.Equals(new ElementInformation())) children.Last().keys.Add(value);
                }
            }

            public bool Full()
            {
                if (keys.Count == maxKeysSize) return true;
                return false;
            }

            public bool IsLeaf()
            {
                if (children.Count == 0) return true;
                return false;
            }
        }

        private struct WorkAndGeneralTime
        {
            public int worktime;
            public int generaltime;
            public float workpercent;
            public WorkAndGeneralTime(int worktime, int generaltime)
            {
                this.worktime = worktime;
                this.generaltime = generaltime;
                workpercent = (float)Math.Round((float)worktime / generaltime * 100, 2);
            }
        }
        private Node head;
        private int treeHeight;
        private int currentHeigth = 1;
        private Dictionary<string, WorkAndGeneralTime> machinesWorkAndGeneralTime;
        private readonly int degree;

        public BTree(int degree)
        {
            this.degree = degree;
            head = null!;
            machinesWorkAndGeneralTime = new Dictionary<string, WorkAndGeneralTime>();
        }

        // All methods connected with insertion

        public void Insert(ElementInformation value)
        {
            if (machinesWorkAndGeneralTime.ContainsKey(value.type))
            {
                machinesWorkAndGeneralTime[value.type] = new WorkAndGeneralTime
                (
                    machinesWorkAndGeneralTime[value.type].worktime + value.worktime,
                    machinesWorkAndGeneralTime[value.type].generaltime + value.worktime + value.downtime
                );
            }
            else
            {
                machinesWorkAndGeneralTime[value.type] = new WorkAndGeneralTime
                (
                    value.worktime,
                    value.worktime + value.downtime
                );
            }

            machinesWorkAndGeneralTime = machinesWorkAndGeneralTime.OrderBy(x => x.Value.workpercent).
                ToDictionary(x => x.Key, x => x.Value);

            if (IsEmpty())
            {
                head = new Node(degree);
                head.AddKey(value);
                treeHeight++;
            }
            else
            {
                if (Search(value.inventoryNumber) != null) return;
                Node nodeToAdd = GetNodeToAdd(value.inventoryNumber);
                if (!nodeToAdd.AddKey(value))
                    SplitNodeAndPaste(nodeToAdd, value);
            }
        }

        private Node GetNodeToAdd(int value)
        {
            Node current = head;
            while (current.GetIndexToGo(value) != -1)
            {
                int next = current.GetIndexToGo(value);
                current = current.children[next];
            }
            return current;
        }

        private static ElementInformation GetMedium(List<ElementInformation> nums)
        {
            if (nums.Count % 2 == 0)
                return nums[nums.Count / 2 - 1];
            else
                return nums[nums.Count / 2];
        }

        private void SplitNodeAndPaste(Node nodeToSplit, ElementInformation value)
        {
            nodeToSplit.keys.Add(value);
            nodeToSplit.keys.Sort((x, y) => x.inventoryNumber.CompareTo(y.inventoryNumber));
            ElementInformation medium = GetMedium(nodeToSplit.keys);
            Node firstBranch = new(degree);
            Node secondBranch = new(degree);

            firstBranch.keys.AddRange(nodeToSplit.keys.GetRange(0, nodeToSplit.keys.Count / 2));
            secondBranch.keys.AddRange(nodeToSplit.keys.GetRange(nodeToSplit.keys.Count / 2,
                nodeToSplit.keys.Count - nodeToSplit.keys.Count / 2));
            if (firstBranch.keys.Contains(medium)) firstBranch.keys.Remove(medium);
            if (secondBranch.keys.Contains(medium)) secondBranch.keys.Remove(medium);

            for (int i = 0; i < nodeToSplit.children.Count; i++)
            {
                if (i < nodeToSplit.children.Count / 2)
                    firstBranch.children.Add(nodeToSplit.children[i]);
                else
                    secondBranch.children.Add(nodeToSplit.children[i]);
            }
            if (nodeToSplit == head)
            {
                Node newNode = new(degree);
                newNode.AddKey(medium);
                newNode.AddChild(firstBranch);
                newNode.AddChild(secondBranch);
                newNode.children.Sort((x, y) => x.keys[0].inventoryNumber.CompareTo(y.keys[0].inventoryNumber));
                head = newNode;
                treeHeight++;
            }
            else
            {
                Node parent = head;
                while (true)
                {
                    if (parent.children.Contains(nodeToSplit)) break;
                    parent = parent.children[parent.GetIndexToGo(nodeToSplit.keys[0].inventoryNumber)];
                }
                parent.children.Remove(nodeToSplit);
                parent.children.Add(firstBranch);
                parent.children.Add(secondBranch);
                parent.children.Sort((x, y) => x.keys[0].inventoryNumber.CompareTo(y.keys[0].inventoryNumber));
                if (!parent.Full())
                    parent.AddKey(medium);
                else
                    SplitNodeAndPaste(parent, medium);
            }
        }

        // All methods connected with deleting

        public void Delete(int inventoryNumber)
        {
            if (IsEmpty()) return;
            Node nodeToDelete = Search(inventoryNumber);

            if (nodeToDelete != null)
            {
                int deleteKeyIndex = nodeToDelete.keys.FindIndex(item => item.inventoryNumber == inventoryNumber);
                machinesWorkAndGeneralTime[nodeToDelete.keys[deleteKeyIndex].type] = new WorkAndGeneralTime(
                        machinesWorkAndGeneralTime[nodeToDelete.keys[deleteKeyIndex].type].worktime - nodeToDelete.keys[deleteKeyIndex].worktime,
                        machinesWorkAndGeneralTime[nodeToDelete.keys[deleteKeyIndex].type].generaltime - nodeToDelete.keys[deleteKeyIndex].worktime - nodeToDelete.keys[deleteKeyIndex].downtime
                    );
                if (machinesWorkAndGeneralTime[nodeToDelete.keys[deleteKeyIndex].type].generaltime == 0)
                    machinesWorkAndGeneralTime.Remove(nodeToDelete.keys[deleteKeyIndex].type);
                machinesWorkAndGeneralTime = machinesWorkAndGeneralTime.OrderBy(x => x.Value.workpercent).
                ToDictionary(x => x.Key, x => x.Value);
                DeletePrivate(nodeToDelete, nodeToDelete!.keys[deleteKeyIndex]);
            }
            else
                Console.WriteLine("Element not found");
        }

        private void DeletePrivate(Node node, ElementInformation value)
        {
            if (!node.IsLeaf())
            {
                Node potentialDonorLeft = GetNodeToAdd(value.inventoryNumber);
                Node potentialDonorRight = GetNodeToAdd(value.inventoryNumber + 1);
                if (potentialDonorRight.keys.Count > 1)
                {
                    node.keys[node.keys.IndexOf(value)] = potentialDonorLeft.keys[0];
                    DeletePrivate(potentialDonorLeft, potentialDonorLeft.keys[0]);
                }
                else
                {
                    node.keys[node.keys.IndexOf(value)] = potentialDonorLeft.keys[^1];
                    DeletePrivate(potentialDonorLeft, potentialDonorLeft.keys[^1]);
                }
            }
            else
            {
                node.keys.Remove(value);
                if (node.keys.Count == 0 && node != head)
                    Rebalance(node, value);
                else
                    return;
            }
        }

        private Node Rebalance(Node nodeToRebalance, ElementInformation originalValue)
        {
            if (nodeToRebalance == head)
            {
                head = nodeToRebalance.children[0];
                treeHeight--;
                return null!;
            }

            Node parent = head;
            while (true)
            {
                if (parent.children.Contains(nodeToRebalance)) break;
                parent = parent.children[parent.GetIndexToGo(originalValue.inventoryNumber)];
            }
            int parentKeyIndex;
            int leftNeigbourIndex = GetPrev(parent.children.IndexOf(nodeToRebalance));
            int rightNeighbourIndex = GetNext(parent, parent.children.IndexOf(nodeToRebalance));
            Node leftNeighbour = parent.children[leftNeigbourIndex];
            Node rightNeighbour = parent.children[rightNeighbourIndex];

            if (leftNeighbour.keys.Count > 1)
            {
                parentKeyIndex = parent.children.IndexOf(nodeToRebalance) - 1;
                nodeToRebalance.keys.Add(parent.keys[parentKeyIndex]);
                parent.keys[parentKeyIndex] = leftNeighbour.keys[^1];
                leftNeighbour.keys.Remove(leftNeighbour.keys.Last());
                if (!leftNeighbour.IsLeaf())
                {
                    Node oddChild = leftNeighbour.children[^1];
                    leftNeighbour.children.Remove(oddChild);
                    if (nodeToRebalance.children.Count == nodeToRebalance.keys.Count)
                        nodeToRebalance.children.Add(oddChild);
                    else
                        return oddChild;
                }
            }
            else if (rightNeighbour.keys.Count > 1)
            {
                parentKeyIndex = parent.children.IndexOf(nodeToRebalance);
                nodeToRebalance.keys.Add(parent.keys[parentKeyIndex]);
                parent.keys[parentKeyIndex] = rightNeighbour.keys[0];
                rightNeighbour.keys.RemoveAt(0);
                if (!rightNeighbour.IsLeaf())
                {
                    Node oddChild = rightNeighbour.children[0];
                    rightNeighbour.children.Remove(oddChild);
                    if (nodeToRebalance.children.Count == 1)
                        nodeToRebalance.children.Add(oddChild);
                    else
                        return oddChild;
                }
            }
            else
            {
                parentKeyIndex = GetPrev(parent.children.IndexOf(nodeToRebalance));
                nodeToRebalance.keys.Add(parent.keys[parentKeyIndex]);
                parent.keys.RemoveAt(parentKeyIndex);
                ElementInformation parentOriginalValue = nodeToRebalance.keys[0];
                Node neigbourToMerge = leftNeighbour == nodeToRebalance ? rightNeighbour : leftNeighbour;

                neigbourToMerge.AddKey(nodeToRebalance.keys[0]);
                foreach (Node child in nodeToRebalance.children)
                {
                    neigbourToMerge.AddChild(child);
                }
                neigbourToMerge.children.Sort((x, y) => x.keys[0].inventoryNumber.CompareTo(y.keys[0].inventoryNumber));
                parent.children.Remove(nodeToRebalance);

                if (parent.keys.Count == 0)
                {
                    if (parent == head)
                    {
                        head = neigbourToMerge;
                        treeHeight--;
                        return null!;
                    }
                    else
                    {
                        Node additionalChild = Rebalance(parent, parentOriginalValue);
                        if (additionalChild != null)
                            parent.AddChild(additionalChild);
                        else
                            return null!;
                    }
                }
            }
            return null!;
        }

        private static int GetNext(Node node, int num)
        {
            return num + 1 > node.children.Count - 1 ? node.children.Count - 1 : num + 1;
        }

        private static int GetPrev(int num)
        {
            return num - 1 < 0 ? 0 : num - 1;
        }

        public string GetMostUsedMachine()
        {
            if (machinesWorkAndGeneralTime.Count == 0)
                return "There are no machines in tree!";
            else
                return $"Type " +
                    $"{machinesWorkAndGeneralTime.ElementAt(machinesWorkAndGeneralTime.Count - 1).Key} " +
                    $"worked {machinesWorkAndGeneralTime.ElementAt(machinesWorkAndGeneralTime.Count - 1).Value.workpercent}% of the time";
        }

        public string GetMachineByInventoryNumber(int inventoryNumber)
        {
            if (IsEmpty()) return "Tree is empty";
            Node searchingNode = Search(inventoryNumber);
            if (searchingNode != null)
            {
                int keyIndex = searchingNode.keys.FindIndex(item => item.inventoryNumber == inventoryNumber);
                return $"Machine with inventory number '{inventoryNumber}' has " +
                    $"{Math.Round((float)searchingNode.keys[keyIndex].downtime / (searchingNode.keys[keyIndex].downtime + searchingNode.keys[keyIndex].worktime) * 100, 2)}% downtime.";
            }
            else
            {
                return $"Machine with inventory number {inventoryNumber} not found!";
            }
        }

        // All methods connected with printing

        public Tuple<List<string>, List<int>> GetTreeElementsOnHeigth(int heigth)
        {
            Tuple<List<string>, List<int>> result = Tuple.Create(new List<string>(), new List<int>());
            GetAllKeysOnHeigth(heigth, result, head);
            return result;
        }
        private Tuple<List<string>, List<int>> GetAllKeysOnHeigth(int heigth, Tuple<List<string>, List<int>> temporary, Node node)
        {

            if (currentHeigth == heigth)
            {
                temporary.Item1.Add(string.Join(", ", node.keys.Select(x => x.inventoryNumber)));
                temporary.Item2.Add(node.children.Count);
                currentHeigth--;
                return temporary;
            }
            else
            {
                foreach (Node child in node.children)
                {
                    currentHeigth++;
                    temporary = GetAllKeysOnHeigth(heigth, temporary, child);
                }

                currentHeigth--;
            }
            return temporary;
        }

        public int GetHeigth()
        {
            return treeHeight;
        }

        public void SetCurrentHeigth(int value)
        {
            currentHeigth = value;
        }

        public bool IsEmpty()
        {
            if (head == null || head.keys.Count == 0) return true;
            else return false;
        }

        private Node Search(int value)
        {
            Node current = head;
            while (true)
            {
                if (current.keys.Any(item => item.inventoryNumber == value)) return current;
                if (current.GetIndexToGo(value) == -1) break;
                current = current.children[current.GetIndexToGo(value)];
            }
            return null!;
        }
    }
}
