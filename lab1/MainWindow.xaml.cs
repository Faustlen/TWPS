using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace lab1
{
    public partial class MainWindow : Window
    {
        private IdentifierTree tree = new IdentifierTree();
        private int totalComparisons = 0;
        private int searchCount = 0;
        private List<string> allIdentifiers = new List<string>(); // Список загруженных идентификаторов

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            string path = FilePathTextBox.Text;
            if (File.Exists(path))
            {
                allIdentifiers = File.ReadAllLines(path).Select(id => id.Trim()).ToList();
                tree.Clear();
                foreach (var id in allIdentifiers)
                {
                    tree.Insert(id);
                }
                UpdateList();
            }
            else
            {
                MessageBox.Show("Файл не найден!");
            }
        }

        private void SearchIdentifier_Click(object sender, RoutedEventArgs e)
        {
            string identifier = SearchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(identifier))
            {
                searchCount++;
                int comparisons;
                bool found = tree.Search(identifier, out comparisons);

                UpdateSearchStatistics(found, comparisons);
            }
        }

        private void FindAll_Click(object sender, RoutedEventArgs e)
        {
            if (allIdentifiers.Count == 0)
            {
                MessageBox.Show("Список идентификаторов пуст. Сначала загрузите файл.");
                return;
            }

            int totalComparisonsLocal = 0;
            int foundCount = 0;

            foreach (var identifier in allIdentifiers)
            {
                int comparisons;
                bool found = tree.Search(identifier, out comparisons);
                totalComparisonsLocal += comparisons;
                if (found) foundCount++;
            }

            searchCount++;
            totalComparisons += totalComparisonsLocal;
            double avgComparisons = searchCount > 0 ? (double)totalComparisons / searchCount : 0;

            SearchCountTextBlock.Text = $"Всего поиск: {searchCount} раз";
            TreeStatusText.Text = $"Найдено {foundCount} из {allIdentifiers.Count}";

            TreeComparisons.Text = $"Сравнений: {totalComparisonsLocal}";
            TreeTotalComparisons.Text = $"Всего сравнений: {totalComparisons}";
            TreeAvgComparisons.Text = $"В среднем сравнений: {avgComparisons:F2}";
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            searchCount = 0;
            totalComparisons = 0;

            SearchCountTextBlock.Text = "Всего поиск: 0 раз";
            TreeStatusText.Text = "Поиск не проводился";


            TreeComparisons.Text = "Сравнений: 0";
            TreeTotalComparisons.Text = "Всего сравнений: 0";
            TreeAvgComparisons.Text = "В среднем сравнений: 0";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateList()
        {
            IdentifiersList.Items.Clear();
            foreach (var entry in tree.GetAllIdentifiers())
            {
                IdentifiersList.Items.Add(entry);
            }
        }

        private void UpdateSearchStatistics(bool found, int comparisons)
        {
            searchCount++;
            totalComparisons += comparisons;
            double avgComparisons = searchCount > 0 ? (double)totalComparisons / searchCount : 0;

            SearchCountTextBlock.Text = $"Всего поиск: {searchCount} раз";

            if (found)
            {
                TreeStatusText.Text = "Идентификатор найден";
            }
            else
            {
                TreeStatusText.Text = "Идентификатор не найден";
            }

            TreeComparisons.Text = $"Сравнений: {comparisons}";
            TreeTotalComparisons.Text = $"Всего сравнений: {totalComparisons}";
            TreeAvgComparisons.Text = $"В среднем сравнений: {avgComparisons:F2}";
        }
    }

    public class TreeNode
    {
        public char Key { get; set; }
        public List<string> Identifiers { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }

        public TreeNode(char key)
        {
            Key = key;
            Identifiers = new List<string>();
        }
    }

    public class IdentifierTree
    {
        private TreeNode root;

        public void Insert(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) return;
            char key = identifier[0];
            root = InsertRecursive(root, key, identifier);
        }

        private TreeNode InsertRecursive(TreeNode node, char key, string identifier)
        {
            if (node == null)
            {
                var newNode = new TreeNode(key);
                newNode.Identifiers.Add(identifier);
                return newNode;
            }

            if (key < node.Key)
                node.Left = InsertRecursive(node.Left, key, identifier);
            else if (key > node.Key)
                node.Right = InsertRecursive(node.Right, key, identifier);
            else
                node.Identifiers.Add(identifier);

            return node;
        }

        public bool Search(string identifier, out int comparisons)
        {
            comparisons = 0;
            char key = identifier[0];
            TreeNode node = FindNode(root, key, ref comparisons);

            if (node != null)
            {
                comparisons += node.Identifiers.Count;
                return node.Identifiers.Contains(identifier);
            }
            return false;
        }

        private TreeNode FindNode(TreeNode node, char key, ref int comparisons)
        {
            while (node != null)
            {
                comparisons++;
                if (key < node.Key) node = node.Left;
                else if (key > node.Key) node = node.Right;
                else return node;
            }
            return null;
        }

        public void Clear() => root = null;

        public List<string> GetAllIdentifiers()
        {
            List<string> result = new List<string>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(TreeNode node, List<string> result)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, result);
                result.Add($"{node.Key}: {string.Join(", ", node.Identifiers)}");
                InOrderTraversal(node.Right, result);
            }
        }
    }
}
