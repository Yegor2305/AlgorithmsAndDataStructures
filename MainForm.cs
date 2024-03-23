using AlgorithmsAndDataStructures.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AlgorithmsAndDataStructures
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    exceptions[i, j] = 1;
                }
            }

            instructionsLabel.Text = "1 - Resedential, 2 - Urban, 3 - Indastrial";
        }

        #region Linear data structures
        private LinkedList list = new();
        private Heap heap = new(40);
        private bool fromLastToFirst = true;

        private void linkedListElementToAddRemoveBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void addToLinkedListButton_Click(object sender, EventArgs e)
        {
            if (linkedListAfterWhichInsertBox.Text != "" && linkedListElementToAddRemoveBox.Text != "")
            {
                list.Add(int.Parse(linkedListElementToAddRemoveBox.Text),
                    int.Parse(linkedListAfterWhichInsertBox.Text));
                linkedListLabel.Text = $"List content: {list.GetList(fromLastToFirst)}";
            }
            else if (linkedListElementToAddRemoveBox.Text != "")
            {
                list.Add(int.Parse(linkedListElementToAddRemoveBox.Text));
                linkedListLabel.Text = $"List content: {list.GetList(fromLastToFirst)}";
            }
            linkedListElementToAddRemoveBox.Text = "";
            linkedListAfterWhichInsertBox.Text = "";
        }

        private void reverseLinkedListButton_Click(object sender, EventArgs e)
        {
            fromLastToFirst = !fromLastToFirst;
            linkedListLabel.Text = $"List content: {list.GetList(fromLastToFirst)}";
        }

        private void removeFromLinkedListButton_Click(object sender, EventArgs e)
        {
            if (linkedListElementToAddRemoveBox.Text != "")
            {
                list.Remove(int.Parse(linkedListElementToAddRemoveBox.Text));
                linkedListLabel.Text = $"List content: {list.GetList(fromLastToFirst)}";
            }
        }

        private void findElementInLinkedList_Click(object sender, EventArgs e)
        {
            if (linkedListElementToAddRemoveBox.Text != "")
            {
                int res = list.FindNodeByData(int.Parse(linkedListElementToAddRemoveBox.Text));
                if (res == -1)
                    linkedListElementToAddRemoveBox.Text = "";
                else
                    linkedListElementToAddRemoveBox.Text = $"{res}";

            }
        }

        private void uploadHeapFileButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openHeapFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                heap = new(40);
                using (StreamReader file = new(openHeapFile.FileName))
                {
                    string line;
                    ThingsData info = new();
                    while ((line = file.ReadLine()!) != null)
                    {
                        info.name = line;
                        line = file.ReadLine()!;
                        info.person = line;
                        line = file.ReadLine()!;
                        info.number = int.Parse(line);
                        line = file.ReadLine()!;
                        info.price = float.Parse(line, CultureInfo.InvariantCulture);
                        line = file.ReadLine()!;
                        info.date = DateTime.ParseExact(line, "dd.MM.yyyy", null);
                        heap.Insert(elem: info);
                    }
                }
                heapResultsBox.Text = heap.PrintHeap(fromLastToFirst);
            }
        }

        private void reverseHeapButton_Click(object sender, EventArgs e)
        {
            fromLastToFirst = !fromLastToFirst;
            heapResultsBox.Text = heap.PrintHeap(fromLastToFirst);
        }

        private void sortHeapButton_Click(object sender, EventArgs e)
        {
            heap.HeapSort();
            heapResultsBox.Text = heap.PrintHeap(fromLastToFirst);
        }

        #endregion

        #region Hash table
        private HashTable table = new(300);

        private void setTableSizeButton_Click(object sender, EventArgs e)
        {
            if (hashTableSize.Text != "")
            {
                table.Rehash(int.Parse(hashTableSize.Text));
                hashTableSize.Text = "";
            }

        }

        private void hashTableSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            openHashTableFile = new OpenFileDialog();

            if (openHashTableFile.ShowDialog() == DialogResult.OK)
            {
                string path = openHashTableFile.FileName;
                string text = File.ReadAllText(path);
                string signs = ".,!?-()";
                foreach (char c in signs)
                {
                    text = text.Replace(c.ToString(), "");
                }

                text = text.Replace('\n', ' ');

                string[] words = text.Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    table.Add(words[i]);
                }

                tableOutputBox.Text = table.GetHashTableContent();
                mostFrequentWord.Text = table.GetMostFrequentWord();

            }
        }

        private void findWordButton_Click(object sender, EventArgs e)
        {
            findedWord.Text = table.GetNumberOfRepetitions(wordToFind.Text);
        }

        private void removeWordButton_Click(object sender, EventArgs e)
        {
            table.Remove(wordToRemove.Text);
            tableOutputBox.Text = table.GetHashTableContent();
            mostFrequentWord.Text = table.GetMostFrequentWord();
        }
        #endregion

        #region BTree

        private int elementHeigth = 29;
        private int startYPosition = 409;
        private int startXPosition = 410;
        private BTree bTree = new(3);

        private void DisplayTree(BTree tree)
        {
            startXPosition = 410;
            if (tree.IsEmpty()) return;
            Tuple<List<string>, List<int>> tuple = tree.GetTreeElementsOnHeigth(tree.GetHeigth());
            tree.SetCurrentHeigth(1);
            List<string> keys = tuple.Item1;
            List<int> childrenNumber = tuple.Item2;
            List<int> sizes = Enumerable.Repeat(102, keys.Count).ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                Label label = new()
                {
                    Size = new Size(100, elementHeigth),
                    Margin = new Padding(0),
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(startXPosition + i * 102, startYPosition),
                    Text = keys[i]
                };
                btreeTab.Controls.Add(label);
            }

            for (int i = tree.GetHeigth(); i > 0; i--)
            {
                tuple = tree.GetTreeElementsOnHeigth(i - 1);
                tree.SetCurrentHeigth(1);
                keys = tuple.Item1;
                childrenNumber = tuple.Item2;
                int startPosition = startXPosition;
                int firstPadding = 0;
                for (int j = 0; j < keys.Count; j++)
                {
                    int sum = sizes.GetRange(j, childrenNumber[j]).Sum();
                    int xLocation = (sum - 102) / 2;
                    if (j == 0) firstPadding += xLocation;
                    sizes.RemoveRange(j, childrenNumber[j]);
                    if (j == 0 || j == keys.Count - 1) sizes.Insert(j, sum - xLocation);
                    else sizes.Insert(j, sum);

                    Label label = new()
                    {
                        Size = new Size(100, elementHeigth),
                        Margin = new Padding(0),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Location = new Point(startPosition + xLocation, startYPosition - (tree.GetHeigth() - (i - 1)) * elementHeigth * 2),
                        Text = keys[j]
                    };
                    btreeTab.Controls.Add(label);
                    startPosition += sum;
                }
                startXPosition += firstPadding;
            }
        }

        private void addItemButton_Click(object sender, EventArgs e)
        {
            if (inventoryNumber.Text.Length == 0 || type.Text.Length == 0 ||
                name.Text.Length == 0 || workTime.Text.Length == 0 ||
                downtime.Text.Length == 0) { return; }

            ElementInformation info = new();
            info.inventoryNumber = int.Parse(inventoryNumber.Text);
            info.type = type.Text;
            info.name = name.Text;
            info.worktime = int.Parse(workTime.Text);
            info.downtime = int.Parse(downtime.Text);
            bTree.Insert(info);
            //            if (tree.GetHeigth() < 4) {
            foreach (var label in btreeTab.Controls.OfType<Label>().ToList())
            {
                btreeTab.Controls.Remove(label);
            }
            DisplayTree(bTree);
            //            } 
            machineLabel.Text = bTree.GetMachineByInventoryNumber(int.Parse(inventoryNumber.Text));
            mostUsedMachineLabel.Text = bTree.GetMostUsedMachine();
        }

        private void removeItemButton_Click(object sender, EventArgs e)
        {
            if (inventoryNumber.Text.Length == 0) return;
            foreach (var label in btreeTab.Controls.OfType<Label>().ToList())
            {
                btreeTab.Controls.Remove(label);
            }
            bTree.Delete(int.Parse(inventoryNumber.Text));
            DisplayTree(bTree);
            machineLabel.Text = bTree.GetMachineByInventoryNumber(int.Parse(inventoryNumber.Text));
            mostUsedMachineLabel.Text = bTree.GetMostUsedMachine();
        }

        private void HandleLetters(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void inventoryNumber_TextChanged(object sender, EventArgs e)
        {
            if (inventoryNumber.Text != "")
                machineLabel.Text = bTree.GetMachineByInventoryNumber(int.Parse(inventoryNumber.Text));
        }

        private void setDegreeButton_Click(object sender, EventArgs e)
        {
            if (degreeBox.Text.Length == 0 || (int.Parse(degreeBox.Text) < 3 || int.Parse(degreeBox.Text) > 8)) return;
            if (bTree.IsEmpty())
            {
                bTree = new BTree(int.Parse(degreeBox.Text));
            }
            else
            {
                DialogResult result = MessageBox.Show("Existing tree will be deleted!", "Attention!",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    bTree = new BTree(int.Parse(degreeBox.Text));
                    foreach (var label in btreeTab.Controls.OfType<Label>().ToList())
                    {
                        btreeTab.Controls.Remove(label);
                    }
                }
            }
            degreeBox.Text = "";
        }

        #endregion

        #region Greedy algorithms
        private HuffmanTree huffmanTree = new();
        private InvestorProblem investorProblem = new();
        private string text = "";

        private void decodeDataButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "BIN files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string binFileName = openFileDialog.FileName;
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    huffmanTree.UploadDataForDecoding(openFileDialog.FileName);
                    outputTextBox.Text = huffmanTree.Decode(binFileName);
                }
            }
        }

        private void uploadFileOrEncodeTextButton_Click(object sender, EventArgs e)
        {

            if (inputTextBox.Text.Trim() == "")
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    text = File.ReadAllText(openFileDialog.FileName);
                else
                    return;
            }
            else
                text = inputTextBox.Text;

            List<Node> symbols = [];

            foreach (char ch in text)
            {
                if (!symbols.Any(item => item.ch == ch.ToString()))
                {
                    symbols.Add(new Node(ch.ToString(), text.Count(item => item == ch)));
                }
            }
            huffmanTree.CreateTree(symbols);
            saveEncodedTextButton.Visible = true;
            saveDecodingDataButton.Visible = true;
        }

        private void saveEncodedTextButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "BIN files (*.bin)|*.bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                huffmanTree.EncodeTextAndWriteToFile(saveFileDialog.FileName, text);
            }
        }

        private void saveDecodingDataButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                huffmanTree.SaveDataForDecoding(saveFileDialog.FileName);
            }
        }

        private void addProjectButton_Click(object sender, EventArgs e)
        {
            if (dateStart.Text == "" || dateEnd.Text == "" ||
                profit.Text.Remove(profit.Text.Length - 1).Trim() == "") return;
            if (DateTime.TryParse(dateStart.Text, out DateTime startDate) &&
                DateTime.TryParse(dateEnd.Text, out DateTime endDate))
            {
                investorProblem.AddProject(new Project(startDate, endDate,
                    float.Parse(profit.Text.Remove(profit.Text.Length - 1).Trim())));
                inputProjectsBox.Text = investorProblem.GetInputProjects();
            }
        }

        private void resetProjectsButton_Click(object sender, EventArgs e)
        {
            investorProblem.Clear();
            inputProjectsBox.Text = investorProblem.GetInputProjects();
            selectedProjectsBox.Text = investorProblem.GetSelectedProjects();
        }

        private void selectProjectsButton_Click(object sender, EventArgs e)
        {
            investorProblem.SelectProjects();
            selectedProjectsBox.Text = investorProblem.GetSelectedProjects();
        }
        #endregion

        #region Dynamic programming

        private List<int> cuts = [];
        private int[,] exceptions = new int[3, 3];

        private string MinCostStringCut(string inputString, int[] cuts)
        {
            int[,] resultsMatrix = new int[inputString.Length + 1, inputString.Length + 1];
            int[,] cutsMatrix = new int[inputString.Length + 1, inputString.Length + 1];
            string finalCuts = "";

            for (int i = 0; i < inputString.Length + 1; i++)
            {
                for (int j = 0; j < inputString.Length + 1; j++)
                {
                    if (i == j)
                        resultsMatrix[i, j] = 0;
                    else
                        resultsMatrix[i, j] = int.MaxValue;
                }
            }

            int cutString(int first, int last)
            {
                if (last - first == 1) return 0;
                if (resultsMatrix[first, last] != int.MaxValue) return resultsMatrix[first, last];

                int result = int.MaxValue;

                foreach (int cut in cuts)
                {
                    if (first < cut && cut < last)
                    {
                        int tempResult = cutString(first, cut) + cutString(cut, last) + (last - first);
                        if (tempResult < result)
                        {
                            result = tempResult;
                            cutsMatrix[first, last] = cut;
                        }

                        resultsMatrix[first, last] = result;
                    }
                }

                if (result == int.MaxValue) result = 0;


                return result;

            }

            void getCuts(int first, int last)
            {
                if (cutsMatrix[first, last] == 0) return;

                int cut = cutsMatrix[first, last];
                finalCuts += $" {cut}";
                getCuts(first, cut);
                getCuts(cut, last);
            }

            int result = cutString(0, inputString.Length);
            getCuts(0, inputString.Length);

            return $"{result} -> {finalCuts.Trim()}";
        }

        private string CalculateCombinations(int[,] exceptions, int houseNumber)
        {
            if (houseNumber > 39) return double.PositiveInfinity.ToString();
            long[,] elementsMatrix = new long[houseNumber, 3];
            for (int i = 0; i < 3; i++)
            {
                elementsMatrix[0, i] = 1;
            }

            for (int i = 1; i < houseNumber; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (exceptions[0, j] == 1) elementsMatrix[i, 0] += elementsMatrix[i - 1, j];
                    if (exceptions[1, j] == 1) elementsMatrix[i, 1] += elementsMatrix[i - 1, j];
                    if (exceptions[2, j] == 1) elementsMatrix[i, 2] += elementsMatrix[i - 1, j];
                }
            }

            return (elementsMatrix[houseNumber - 1, 0] +
                elementsMatrix[houseNumber - 1, 1] +
                elementsMatrix[houseNumber - 1, 2]).ToString("N0");
        }

        private void addSeparationButton_Click(object sender, EventArgs e)
        {
            if (separation.Text.Trim() == "") return;

            cuts.Add(int.Parse(separation.Text));
            separationsLabel.Text = $"[{string.Join(", ", cuts)}]";
            separation.Text = "";
            separation.Focus();
        }

        private void resetSeparationsButton_Click(object sender, EventArgs e)
        {
            cuts.Clear();
            separationsLabel.Text = "No separations";
        }

        private void calculateCuttingStringButton_Click(object sender, EventArgs e)
        {
            if (cuts.Count == 0 || inputString.Text.Trim() == "" ||
                cuts.Any(e => e > inputString.Text.Length))
            {
                resultLabel.Text = "Error";
                return;
            }

            resultLabel.Text = $"Result: {MinCostStringCut(inputString.Text, [.. cuts])}";
        }

        private void separation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void housesNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void exception_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if ((exception.Text.Length == 0 || exception.Text[^1] == ' ') && e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void addExceptionButton_Click(object sender, EventArgs e)
        {
            if (exception.Text == "") return;
            int[] enteredException = exception.Text.Split(' ').Select(int.Parse).ToArray();
            if (enteredException.Any(e => e > 3 || e < 1) || enteredException.Length == 1 ||
                enteredException.Length > 2) return;
            exceptions[enteredException[0] - 1, enteredException[1] - 1] = 0;
            exceptions[enteredException[1] - 1, enteredException[0] - 1] = 0;
            if (exceptionsLabel.Text == "No exceptions")
                exceptionsLabel.Text = $"|{string.Join(", ", enteredException)}|";
            else
                exceptionsLabel.Text += $", |{string.Join(", ", enteredException)}|";

            exception.Text = "";
            exception.Focus();
        }

        private void resetExceptionsButton_Click(object sender, EventArgs e)
        {
            exceptionsLabel.Text = "No exceptions";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    exceptions[i, j] = 1;
                }
            }
        }

        private void calculateCombinationsButton_Click(object sender, EventArgs e)
        {
            if (housesNumber.Text == "") return;
            combinationsResultLabel.Text = $"Result: {CalculateCombinations(exceptions, int.Parse(housesNumber.Text))}";
        }

        #endregion

        #region Graph traversal algorithms
        private string[] connectionsTraversal = [];
        private Label[] labyrinthNodes = [];
        private int lastNumber = 0;
        int nodeNumberTraversal = 0;

        private void calculateCrossingTaskButton_Click(object sender, EventArgs e)
        {
            if (objectsTextBox.Text.Trim() == "" || seatsNumberTextBox.Text.Trim() == "") return;

            CrossingTaskGraph graph = new(objectsTextBox.Text, int.Parse(seatsNumberTextBox.Text));
            resultTextBox.Text = graph.SearchSolution();

        }

        private void setNodesNumberTraversalButton_Click(object sender, EventArgs e)
        {
            if (nodesTraversalCount.Text == "" || int.Parse(nodesTraversalCount.Text) == 0)
            {
                return;
            }
            connectionsTraversal = Enumerable.Repeat("", int.Parse(nodesTraversalCount.Text)).ToArray();
            nodeNumberTraversal = int.Parse(nodesTraversalCount.Text);
            nodeTraversalChoose.Items.Clear();
            nodeToConnectTraversal.Items.Clear();
            for (int i = 1; i <= connectionsTraversal.Length; i++)
            {
                nodeTraversalChoose.Items.Add(i);
                nodeToConnectTraversal.Items.Add(i);
            }
            UpdateTraversalGraphListBox();
            nodeTraversalChoose.SelectedIndex = 0;
            nodeToConnectTraversal.SelectedIndex = 0;
        }

        private void UpdateTraversalGraphListBox()
        {
            nodesListBoxTraversal.Items.Clear();
            for (int i = 0; i < connectionsTraversal.Length; i++)
            {
                if (connectionsTraversal[i] != null)
                {
                    nodesListBoxTraversal.Items.Add($"{i + 1} -> {connectionsTraversal[i].Trim()}");
                }
                else
                {
                    nodesListBoxTraversal.Items.Add($"{i + 1} ->");
                }
            }
        }

        private void addConnectionTraversalButton_Click(object sender, EventArgs e)
        {
            try
            {
                connectionsTraversal[(int)nodeTraversalChoose.SelectedItem! - 1] += $" {nodeToConnectTraversal.SelectedItem}";
                UpdateTraversalGraphListBox();
            }
            catch { }
        }

        private void removeConnectionTraversalButton_Click(object sender, EventArgs e)
        {
            string connectionToFind = nodeToConnectTraversal.Text;
            int node = (int)nodeTraversalChoose.SelectedItem!;
            if (connectionsTraversal[node - 1].Contains(connectionToFind))
            {
                connectionsTraversal[node - 1] = connectionsTraversal[node - 1].Remove(connectionsTraversal[node - 1].IndexOf(connectionToFind),
                    connectionToFind.Length);
            }
            UpdateTraversalGraphListBox();
        }

        private void graphConnCalculateButton_Click(object sender, EventArgs e)
        {
            if (nodeNumberTraversal == 0) return;
            Graph graph = new(nodeNumberTraversal);
            for (int i = 0; i < connectionsTraversal.Length; i++)
            {
                graph.AddConnection(i + 1, connectionsTraversal[i].Trim());
            }
            graphConnResultLabel.Text = graph.CheckConnectivity();
            graphConnResultLabel.Location = new Point(Width / 2 - resultLabel.Width / 2, graphConnResultLabel.Location.Y);
        }

        private void generateLabyrinthButton_Click(object sender, EventArgs e)
        {
            GC.Collect();

            int number = (int)labyrinthSize.Value * (int)labyrinthSize.Value;
            int offset = (int)labyrinthSize.Value;

            for (int i = 0; i < labyrinthPage.Controls.Count; i++)
            {
                if (lastNumber == number)
                {
                    if (labyrinthPage.Controls[i] is Panel)
                    {
                        labyrinthPage.Controls.Remove(labyrinthPage.Controls[i]);
                        i--;
                    }
                }
                else
                {
                    if (labyrinthPage.Controls[i] is Label lab &&
                    lab.Text != "Choose size:" ||
                    labyrinthPage.Controls[i] is Panel)
                    {
                        labyrinthPage.Controls.Remove(labyrinthPage.Controls[i]);
                        i--;
                    }
                }
            }

            lastNumber = number;

            labyrinthNodes = new Label[number];

            int posX = 10;
            int posY = 10;

            for (int i = 0; i < labyrinthNodes.Length; i++)
            {
                labyrinthNodes[i] = new()
                {
                    Text = $"{i + 1}",
                    Size = new Size(40, 40),
                    Location = new Point(posX, posY),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = i == 0 || i == labyrinthNodes.Length - 1 ? Color.Gray : Color.Transparent
                };

                posX += 80;

                if ((i + 1) % offset == 0)
                {
                    posX = 10;
                    posY += 80;
                }

                labyrinthPage.Controls.Add(labyrinthNodes[i]);
            }

            Graph graph = new(number, true);
            Random rand = new();
            string[] labyrinth = graph.GenerateLabyrinth(rand.Next(1, number));

            for (int i = 0; i < labyrinth.Length; i++)
            {
                if (labyrinth[i] == null) continue;
                int[] nodeConnections = labyrinth[i].Split(' ').Select(int.Parse).ToArray();
                foreach (int conn in nodeConnections)
                {
                    Panel panel = new()
                    {
                        BackColor = Color.Black
                    };
                    int direction = i + 1 - conn;
                    if (direction == 1)
                    {
                        panel.Size = new Size(40, 3);
                        panel.Location = new Point(labyrinthNodes[conn - 1].Location.X + 40,
                            labyrinthNodes[conn - 1].Location.Y + labyrinthNodes[conn - 1].Height / 2);
                    }
                    else if (direction == -1)
                    {
                        panel.Size = new Size(40, 3);
                        panel.Location = new Point(labyrinthNodes[i].Location.X + 40,
                            labyrinthNodes[i].Location.Y + labyrinthNodes[i].Height / 2);
                    }
                    else if (direction == offset)
                    {
                        panel.Size = new Size(3, 40);
                        panel.Location = new Point(labyrinthNodes[conn - 1].Location.X + labyrinthNodes[conn - 1].Width / 2,
                            labyrinthNodes[conn - 1].Location.Y + 40);
                    }
                    else if (direction == -offset)
                    {
                        panel.Size = new Size(3, 40);
                        panel.Location = new Point(labyrinthNodes[i].Location.X + labyrinthNodes[i].Width / 2,
                            labyrinthNodes[i].Location.Y + 40);
                    }
                    labyrinthPage.Controls.Add(panel);
                }
            }
        }
        #endregion

        #region Graph path algorithms
        private string[] connectionsPath = [];
        private int nodeNumberPath = 0;

        private void setNodesNumberPathButton_Click(object sender, EventArgs e)
        {
            if (nodesPathCount.Text == "" || int.Parse(nodesPathCount.Text) == 0)
            {
                return;
            }
            connectionsPath = Enumerable.Repeat("", int.Parse(nodesPathCount.Text)).ToArray();
            nodeNumberPath = int.Parse(nodesPathCount.Text);
            nodePathChoose.Items.Clear();
            nodeToConnectPath.Items.Clear();
            startNodeComboBox.Items.Clear();
            finalNodeComboBox.Items.Clear();
            for (int i = 1; i <= connectionsPath.Length; i++)
            {
                nodePathChoose.Items.Add(i);
                nodeToConnectPath.Items.Add(i);
                startNodeComboBox.Items.Add(i);
                finalNodeComboBox.Items.Add(i);
            }
            UpdatePathGraphListBox();
            nodePathChoose.SelectedIndex = 0;
            nodeToConnectPath.SelectedIndex = 0;
            startNodeComboBox.SelectedIndex = 0;
            finalNodeComboBox.SelectedIndex = finalNodeComboBox.Items.Count - 1;
        }


        private void UpdatePathGraphListBox()
        {
            nodesPathListBox.Items.Clear();
            for (int i = 0; i < connectionsPath.Length; i++)
            {
                if (connectionsPath[i] != null)
                {
                    string connection = Regex.Replace(connectionsPath[i].Trim(), @"\.(\-?\d+)", "($1)");
                    nodesPathListBox.Items.Add($"{i + 1} -> {connection}");
                }
                else
                {
                    nodesPathListBox.Items.Add($"{i + 1} ->");
                }
            }
        }

        private void addConnectionPathButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(weightPath.Text, out int number)) return;
            try
            {
                connectionsPath[(int)nodePathChoose.SelectedItem! - 1] += $" {nodeToConnectPath.SelectedItem}.{number}";
                UpdatePathGraphListBox();
            }
            catch { }
        }

        private void floydWarshallButton_Click(object sender, EventArgs e)
        {
            if (nodeNumberPath == 0) return;
            WeightedGraph graph = new(nodeNumberPath);
            for (int i = 0; i < connectionsPath.Length; i++)
            {
                graph.AddConnection(i + 1, connectionsPath[i].Trim());
            }
            result.Text = graph.FloydWarshall();
        }

        private void bellmanFordButton_Click(object sender, EventArgs e)
        {
            if (nodeNumberPath == 0) return;
            WeightedGraph graph = new(nodeNumberPath);
            for (int i = 0; i < connectionsPath.Length; i++)
            {
                graph.AddConnection(i + 1, connectionsPath[i].Trim());
            }
            result.Text = graph.BellmanFord(startNodeComboBox.SelectedIndex + 1);
        }

        private void dijkstraButton_Click(object sender, EventArgs e)
        {
            if (nodeNumberPath == 0) return;
            WeightedGraph graph = new(nodeNumberPath);
            for (int i = 0; i < connectionsPath.Length; i++)
            {
                graph.AddConnection(i + 1, connectionsPath[i].Trim());
            }
            result.Text = graph.Dijkstra(startNodeComboBox.SelectedIndex + 1, finalNodeComboBox.SelectedIndex + 1);
        }

        private void removeConnectionPathButton_Click(object sender, EventArgs e)
        {
            string connectionToFind = $"{int.Parse(nodeToConnectPath.Text)}.{int.Parse(weightPath.Text)}";
            int node = (int)nodePathChoose.SelectedItem!;
            if (connectionsPath[node - 1].Contains(connectionToFind))
            {
                connectionsPath[node - 1] = connectionsPath[node - 1].Remove(connectionsPath[node - 1].IndexOf(connectionToFind),
                    connectionToFind.Length);
            }
            UpdatePathGraphListBox();
        }

        private void weight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '-')
            {
                return;
            }
            else
            {
                e.Handled = true;
            }

        }
        #endregion
    }
}
