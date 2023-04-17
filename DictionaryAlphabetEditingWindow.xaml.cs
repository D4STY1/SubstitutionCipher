using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SubstitutionCipher
{
    /// <summary>
    /// Логика взаимодействия для AlphabetManagementWindow.xaml
    /// </summary>
    public partial class AlphabetManagementWindow : Window
    {
        #region Конструктор, методы
        public AlphabetManagementWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region События
        private void MouseDownTriggerred(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        private void AlphabetNameTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(alphabetNameTextBox.Text))
            {
                alphabetNameHintTextBlock.Visibility = Visibility.Visible;
                return;
            }

            alphabetNameHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void OriginalAlphabetTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(originalAlphabetTextBox.Text))
            {
                originalAlphabetHintTextBlock.Visibility = Visibility.Visible;
                return;
            }

            originalAlphabetTextBox.Text = new string(originalAlphabetTextBox.Text.Distinct().ToArray());
            originalAlphabetTextBox.CaretIndex = originalAlphabetTextBox.Text.Length;

            originalAlphabetHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void CipherAlphabetTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cipherAlphabetTextBox.Text))
            {
                cipherAlphabetHintTextBlock.Visibility = Visibility.Visible;
                return;
            }

            cipherAlphabetTextBox.Text = new string(cipherAlphabetTextBox.Text.Distinct().ToArray());
            cipherAlphabetTextBox.CaretIndex = cipherAlphabetTextBox.Text.Length;

            cipherAlphabetHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) => Close();

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(alphabetNameTextBox.Text))
            {
                MessageBox.Show("Недопустимое название словаря");
                return;
            }

            if (string.IsNullOrEmpty(originalAlphabetTextBox.Text))
            {
                MessageBox.Show("Недопустимое значение в поле 'Оригинальные символы'");
                return;
            }

            if (string.IsNullOrEmpty(cipherAlphabetTextBox.Text))
            {
                MessageBox.Show("Недопустимое значение в поле 'Заменяемые символы'");
                return;
            }

            if (originalAlphabetTextBox.Text.Length != cipherAlphabetTextBox.Text.Length)
            {
                MessageBox.Show("Недопустимая длина полей 'Оригинальные символы', 'Заменяемые символы'\nДлина полей должна быть эквивалентна друг другу");
                return;
            }

            if (Application.Current.MainWindow is null || ((MainWindow)Application.Current.MainWindow) is null)
            {
                MessageBox.Show("Недопустимый handle окна MainWindow");
                return;
            }

            MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);

            if (mainWindow.AlphabetDictionaries.ContainsKey(alphabetNameTextBox.Text))
            {
                MessageBox.Show($"Название словаря '{alphabetNameTextBox.Text}' уже присутствует в списке словарей, укажите другое название");
                return;
            }

            mainWindow.AlphabetDictionaries.Add(alphabetNameTextBox.Text, new Dictionary<char, char>());

            for (int i = 0; i < cipherAlphabetTextBox.Text.Length; i++)
            {
                char originalSymbol = originalAlphabetTextBox.Text[i];
                char replaceableSymbol = cipherAlphabetTextBox.Text[i];

                mainWindow.AlphabetDictionaries.Last().Value.Add(originalSymbol, replaceableSymbol);
            }

            if (!Directory.Exists("Алфавитные словари"))
                Directory.CreateDirectory("Алфавитные словари");

            File.WriteAllText($@"Алфавитные словари\{alphabetNameTextBox.Text}", $"{originalAlphabetTextBox.Text}\n{cipherAlphabetTextBox.Text}");

            mainWindow.AlphabetDictionariesListBox.Items.Add(alphabetNameTextBox.Text);
            Close();
        }
        #endregion
    }
}
