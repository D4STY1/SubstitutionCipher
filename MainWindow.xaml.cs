using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SubstitutionCipher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Поля, свойства
        public Dictionary<string, Dictionary<char, char>> AlphabetDictionaries = new();
        #endregion

        #region Конструктор, методы
        public MainWindow() => InitializeComponent();

        /// <summary>
        /// Расшифровывает или зашифровывает строку методом замены при помощи выбранного словаря в AlphabetDictionariesListBox<br/><br/>
        /// Выбранный словарь в AlphabetDictionariesListBox находится на форме окна MainWindow.xaml<br/><br/>
        /// Определение действия зашифровывать или расшифровывать происходит автоматически
        /// </summary>
        /// <param name="source">Исходная строка для шифрования/расшифрования</param>
        /// <returns>Зашифрованная или расшифрованная строка</returns>
        public string EncryptOrDecryptSubstitution(string source)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(source) || AlphabetDictionariesListBox.SelectedItem is null || !AlphabetDictionaries.ContainsKey((string)AlphabetDictionariesListBox.SelectedItem))
                return source;

            foreach (char symbol in source)
            {
                if (AlphabetDictionaries[(string)AlphabetDictionariesListBox.SelectedItem].ContainsKey(symbol))
                    result += AlphabetDictionaries[(string)AlphabetDictionariesListBox.SelectedItem][symbol];
                else if (AlphabetDictionaries[(string)AlphabetDictionariesListBox.SelectedItem].ContainsValue(symbol))
                    result += AlphabetDictionaries[(string)AlphabetDictionariesListBox.SelectedItem].FirstOrDefault(x => x.Value == symbol).Key;
                else
                    result += symbol;
            }

            UsedDictionaryTextBlock.Text = (string)AlphabetDictionariesListBox.SelectedItem;

            return result;
        }
        #endregion

        #region События
        private void OriginalTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            cipherTextBox.Text = EncryptOrDecryptSubstitution(originalTextBox.Text);
            originalTextBox.CaretIndex = originalTextBox.Text.Length;

            if (string.IsNullOrEmpty(originalTextBox.Text))
            {
                originalHintTextBlock.Visibility = Visibility.Visible;
                return;
            }

            originalHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void CipherTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            originalTextBox.Text = EncryptOrDecryptSubstitution(cipherTextBox.Text);
            cipherTextBox.CaretIndex = cipherTextBox.Text.Length;

            if (string.IsNullOrEmpty(cipherTextBox.Text))
            {
                cipherHintTextBlock.Visibility = Visibility.Visible;
                return;
            }

            cipherHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void CloseApplicationButtonClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeApplicationButtonClick(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        private void MaximizeApplicationButtonClick(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState is WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("Алфавитные словари"))
                return;

            string[] alphabetFilePaths = Directory.GetFiles("Алфавитные словари");

            foreach (string alphabetFilePath in alphabetFilePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(alphabetFilePath);

                string[] fileLines = File.ReadAllLines(alphabetFilePath);

                if (fileLines.Length != 2 || fileLines[0].Length == 0 || fileLines[1].Length == 0 || fileLines[0].Length != fileLines[1].Length)
                {
                    MessageBoxResult result = MessageBox.Show($"Файл алфавитного словаря {fileName} поврежден, желаете его удалить?", "Предупреждение загрузки недопустимого алфавитного словаря", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                        File.Delete(alphabetFilePath);

                    continue;
                }

                AlphabetDictionaries.Add(fileName, new Dictionary<char, char>());

                for (int index = 0; index < fileLines[0].Length; index++)
                {
                    char originalSymbol = fileLines[0][index];
                    char cipherSymbol = fileLines[1][index];

                    AlphabetDictionaries.Last().Value.Add(originalSymbol, cipherSymbol);
                }

                AlphabetDictionariesListBox.Items.Add(fileName);
            }
        }

        private void MainGridMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void AddAlphabetDictionariesButtonClick(object sender, RoutedEventArgs e)
        {
            AlphabetManagementWindow window = new();
            window.ShowDialog();
        }

        private void RemoveAlphabetDictionariesButtonClick(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("Алфавитные словари"))
            {
                if (File.Exists($@"Алфавитные словари\{(string)AlphabetDictionariesListBox.SelectedItem}"))
                    File.Delete($@"Алфавитные словари\{(string)AlphabetDictionariesListBox.SelectedItem}");
            }

            if (AlphabetDictionariesListBox.SelectedItem is null)
                return;

            AlphabetDictionaries.Remove((string)AlphabetDictionariesListBox.SelectedItem);
            AlphabetDictionariesListBox.Items.RemoveAt(AlphabetDictionariesListBox.SelectedIndex);
        }

        private void AlphabetDictionariesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => cipherTextBox.Text = EncryptOrDecryptSubstitution(originalTextBox.Text);
        #endregion
    }
}
