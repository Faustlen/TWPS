using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace lab2
{
    public partial class MainWindow : Window
    {
        public class Lexeme
        {
            public int Number { get; set; }
            public string LexemeType { get; set; }
            public string LexemeValue { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы|*.txt|Все файлы|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileNameTextBox.Text = openFileDialog.FileName;
            }
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var filePath = FileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                InputTextBox.Text = text;

                var tokens = Tokenize(text);

                TokensDataGrid.ItemsSource = tokens;
            }
            else
            {
                MessageBox.Show("Файл не найден или путь к файлу не указан.");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private List<Lexeme> Tokenize(string input)
        {
            var result = new List<Lexeme>();
            int index = 0;
            int tokenCounter = 1;
            int errorCount = 0;

            while (index < input.Length)
            {
                char c = input[index];

                // Пропуск пробелов, табуляций, переводов строк
                if (char.IsWhiteSpace(c))
                {
                    index++;
                    continue;
                }

                // Однострочные комментарии //
                if (c == '/' && index + 1 < input.Length && input[index + 1] == '/')
                {
                    index += 2;
                    while (index < input.Length && input[index] != '\n')
                    {
                        index++;
                    }
                    continue;
                }

                // Символьная константа: 'x'
                if (c == '\'')
                {
                    if (index + 2 < input.Length && input[index + 2] == '\'')
                    {
                        string charConst = input.Substring(index, 3);
                        result.Add(new Lexeme
                        {
                            Number = tokenCounter++,
                            LexemeType = "Символьная константа",
                            LexemeValue = charConst
                        });
                        index += 3;
                    }
                    else
                    {
                        errorCount++;
                        result.Add(new Lexeme
                        {
                            Number = tokenCounter++,
                            LexemeType = "Ошибка",
                            LexemeValue = "Некорректная символьная константа"
                        });
                        index++;
                    }
                    continue;
                }

                // Присваивание: :=
                if (c == ':' && index + 1 < input.Length && input[index + 1] == '=')
                {
                    result.Add(new Lexeme
                    {
                        Number = tokenCounter++,
                        LexemeType = "Присваивание",
                        LexemeValue = ":="
                    });
                    index += 2;
                    continue;
                }

                // Операторы +, -, *, /
                if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    result.Add(new Lexeme
                    {
                        Number = tokenCounter++,
                        LexemeType = "Оператор",
                        LexemeValue = c.ToString()
                    });
                    index++;
                    continue;
                }

                // Круглые скобки ( )
                if (c == '(' || c == ')')
                {
                    result.Add(new Lexeme
                    {
                        Number = tokenCounter++,
                        LexemeType = "Скобка",
                        LexemeValue = c.ToString()
                    });
                    index++;
                    continue;
                }

                // Точка с запятой ;
                if (c == ';')
                {
                    result.Add(new Lexeme
                    {
                        Number = tokenCounter++,
                        LexemeType = "Разделитель",
                        LexemeValue = ";"
                    });
                    index++;
                    continue;
                }

                // Идентификатор: [a-zA-Z_][a-zA-Z0-9_]* (не более 32 символов)
                if (IsIdentifierStart(c))
                {
                    int startPos = index;
                    index++;
                    while (index < input.Length && IsIdentifierPart(input[index]))
                    {
                        index++;
                    }
                    string ident = input.Substring(startPos, index - startPos);

                    if (ident.Length <= 32)
                    {
                        result.Add(new Lexeme
                        {
                            Number = tokenCounter++,
                            LexemeType = "Идентификатор",
                            LexemeValue = ident
                        });
                    }
                    else
                    {
                        errorCount++;
                        result.Add(new Lexeme
                        {
                            Number = tokenCounter++,
                            LexemeType = "Ошибка",
                            LexemeValue = $"Идентификатор '{ident}' превышает 32 символа"
                        });
                    }
                    continue;
                }

                // Всё остальное -> "Ошибка"
                errorCount++;
                result.Add(new Lexeme
                {
                    Number = tokenCounter++,
                    LexemeType = "Ошибка",
                    LexemeValue = c.ToString()
                });
                index++;
            }

            MessageBox.Show(
                $"Обнаружено ошибок: {errorCount}\nВсего лексем: {result.Count}",
                "Лексический анализ"
            );

            return result;
        }

        private bool IsIdentifierStart(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private bool IsIdentifierPart(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }
    }
}
