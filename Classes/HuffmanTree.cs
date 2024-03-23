using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    public class Node
    {
        public string ch { get; set; }
        public int frequency { get; set; }
        public Node? leftChild { get; set; }
        public Node? rightChild { get; set; }

        public Node(string ch = "", int frequency = 0, Node? leftchild = null, Node? rightchild = null)
        {
            this.ch = ch;
            this.frequency = frequency;
            leftChild = leftchild;
            rightChild = rightchild;
        }

        public static bool operator <(Node left, Node right)
        {
            return left.frequency < right.frequency;
        }
        public static bool operator >(Node left, Node right)
        {
            return left.frequency > right.frequency;
        }
    }

    internal class HuffmanTree
    {
        private List<Node> tree;
        private Dictionary<char, string> codes;

        public HuffmanTree()
        {
            tree = [];
            codes = new();
        }

        public List<Node> GetTree() { return tree; }
        public Dictionary<char, string> GetCodes() { return codes; }

        public class SerializationData()
        {
            public List<Node>? tree { get; set; }
            public Dictionary<char, string>? codes { get; set; }

        }

        public void SaveDataForDecoding(string fileName)
        {
            SerializationData data = new SerializationData { tree = tree, codes = codes };
            var opions = new JsonSerializerOptions { WriteIndented = true };
            string jsonSerializationString = JsonSerializer.Serialize(data, opions);
            File.WriteAllText(fileName, jsonSerializationString);
        }

        public void UploadDataForDecoding(string fileName)
        {
            string jsonDeserilizationString = File.ReadAllText(fileName);
            SerializationData data = JsonSerializer.Deserialize<SerializationData>(jsonDeserilizationString)!;
            tree = data.tree!;
            codes = data.codes!;
        }

        public void CreateTree(List<Node> inputList)
        {
            inputList.Sort((a, b) => a.frequency.CompareTo(b.frequency));
            tree.Clear();
            codes.Clear();
            tree.AddRange(inputList);
            if (tree.Count == 1)
            {
                tree.Add(new Node());
            }

            while (tree.Count > 1)
            {

                tree.Sort((a, b) => a.frequency.CompareTo(b.frequency));

                if (tree.Count >= 2)
                {
                    Node parent = new("", tree[0].frequency + tree[1].frequency, tree[0], tree[1]);

                    tree.RemoveRange(0, 2);
                    tree.Add(parent);
                }
            }
            CreateCodes(tree[0], "");

        }

        private void CreateCodes(Node current, string code)
        {
            if (current != null)
            {
                if (current.ch != string.Empty)
                {
                    codes.Add(current.ch[0], code);
                }
                else
                {
                    CreateCodes(current.leftChild!, code + "0");
                    CreateCodes(current.rightChild!, code + "1");
                }
            }
        }

        public void EncodeTextAndWriteToFile(string filename, string text)
        {
            using (BinaryWriter writer = new(File.Open(filename, FileMode.Create)))
            {
                foreach (char ch in text)
                {
                    foreach (char ch2 in codes[ch])
                    {
                        bool bit = ch2 == '1' ? true : false;
                        writer.Write(bit);
                    }
                }
            }
        }

        public string Decode(string fileToRead)
        {
            string fileText = "";// = File.ReadAllText(fileToRead);

            using (BinaryReader reader = new(File.OpenRead(fileToRead)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    bool bit = reader.ReadBoolean();
                    fileText += bit ? '1' : '0';
                }
            }

            string decodedText = string.Empty;

            Node current = tree.FirstOrDefault()!;

            foreach (var bit in fileText)
            {
                if (bit == '0')
                {
                    current = current.leftChild!;
                }
                else
                {
                    current = current.rightChild!;
                }

                if (current.leftChild == null && current.rightChild == null)
                {
                    decodedText += current.ch;
                    current = tree[0];
                }
            }

            return decodedText;
        }
    }
}
