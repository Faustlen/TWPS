using lab3.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace lab3
{
    public partial class MainWindow : Window
    {
        private List<Token> _tokens = new List<Token>();
        private Parser _parser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Текстовые файлы|*.txt;*.cs;*.cpp;*.pas|Все файлы|*.*";
            if (ofd.ShowDialog() == true)
            {
                txtSource.Text = System.IO.File.ReadAllText(ofd.FileName);
            }
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            var scanner = new Scanner();
            _tokens = scanner.Scan(txtSource.Text);

            for (int i = 0; i < _tokens.Count; i++)
                _tokens[i].Index = i + 1;
            dgTokens.ItemsSource = _tokens;
            dgTokens.Items.Refresh();

            _parser = new Parser(_tokens);
            try
            {
                var root = _parser.ParseS();

                treeSyntax.Items.Clear();
                var treeItem = CreateTreeItem(root);
                treeSyntax.Items.Add(treeItem);
                treeItem.ExpandSubtree();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Синтаксическая ошибка: " + ex.Message);
            }
        }

        private TreeViewItem CreateTreeItem(Node node)
        {
            var item = new TreeViewItem { Header = node.Name };
            foreach (var child in node.Children)
            {
                item.Items.Add(CreateTreeItem(child));
            }
            return item;
        }
    }
}
