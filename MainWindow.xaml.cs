using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Settings = LearnWorld.Properties.Settings;

namespace LearnWorld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string unlearnedFile;
        WordsManager wordsManager;
        string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/LearnWorld";
        delegate void FileNotFoundDel();

        public char Divider { get; set; }

        public int DisplayTime { get; set; }

        public int DisplayInterval { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            dividerTextBox.Text = Settings.Default.divider;
            displayIntervalTextBox.Text = Settings.Default.displayInterval.ToString();
            displayTimeTextBox.Text = Settings.Default.displayTime.ToString();

            Directory.CreateDirectory(settingsDirectory);
            if (File.Exists(settingsDirectory + @"/unlearnedWordsFilePath"))
            {
                Activate(File.ReadAllText(settingsDirectory + @"/unlearnedWordsFilePath"));
            }
        }

        private void ChooseUnlearnedButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            bool? file = dlg.ShowDialog();

            if (file == true) {
                Activate(dlg.FileName);
                File.WriteAllText(settingsDirectory + @"/unlearnedWordsFilePath", unlearnedFile);
            }
        }

        private void OpenUnlearned_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(unlearnedFile);
            }
            catch {
                MessageBox.Show("Can't find the file", "Error");
                ErrorHandler();
            }
        }

        void Activate(string unlearnedFile)
        {
            filePathUnlearnedLabel.Content = unlearnedFile;
            this.unlearnedFile = unlearnedFile;
            openUnlearnedButton.IsEnabled = true;
            WorkButton.IsEnabled = true;
        }

        private void WorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayIntervalTextBox.Text == "")
            {
                MessageBox.Show("Set the display interval", "Warning");
                return;
            }

            if (dividerTextBox.Text == "")
            {
                MessageBox.Show("Set the divider", "Warning");
                return;
            }

            if (displayTimeTextBox.Text == "")
            {
                MessageBox.Show("Set the display time", "Warning");
                return;
            }

            if ((string)WorkButton.Content == "Stop")
            {
                WorkButton.Content = "Start";
                WorkButton.Background = new SolidColorBrush(Color.FromRgb(76, 170, 144));
                displayIntervalTextBox.IsEnabled = true;
                chooseFileButton.IsEnabled = true;
                dividerTextBox.IsEnabled = dividerTextBox.IsEnabled = true;
                displayTimeTextBox.IsEnabled = true;

                wordsManager.Stop();
            }
            else
            {
                WorkButton.Content = "Stop";
                WorkButton.Background = new SolidColorBrush(Color.FromRgb(170, 106, 76));
                displayIntervalTextBox.IsEnabled = false;
                chooseFileButton.IsEnabled = false;
                dividerTextBox.IsEnabled = dividerTextBox.IsEnabled = false;
                displayTimeTextBox.IsEnabled = false;

                wordsManager = new WordsManager(Divider, DisplayTime, DisplayInterval);
                wordsManager.ShowWords(unlearnedFile, this);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public void ErrorHandler()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(() =>
            {
                displayTimeTextBox.IsEnabled = true;
                displayIntervalTextBox.IsEnabled = true;
                dividerTextBox.IsEnabled = true;
                openUnlearnedButton.IsEnabled = false;
                chooseFileButton.IsEnabled = true;
                WorkButton.Content = "Start";
                WorkButton.Background = new SolidColorBrush(Color.FromRgb(76, 170, 144));
                WorkButton.IsEnabled = false;
                filePathUnlearnedLabel.Content = "Choose the file";
            }));
        }

        private void dividerTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(dividerTextBox.Text.Length > 1)
            {
                dividerTextBox.Text = dividerTextBox.Text.Substring(0, 1);
                return;
            }
            else if(dividerTextBox.Text == "")
                return;

            Divider = dividerTextBox.Text.ToCharArray()[0];
            Settings.Default.divider = Divider.ToString();
            Settings.Default.Save();
        }

        private void displayTimeTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
             if (displayTimeTextBox.Text == "")
                return;

            try
            {
                DisplayTime = int.Parse(displayTimeTextBox.Text);

                Settings.Default.displayTime = DisplayTime;
                Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Set the correct display time", "Warning");
                displayTimeTextBox.Text = "";
            }
        }

        private void displayIntervalTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (displayIntervalTextBox.Text == "")
                return;

            try
            {
                DisplayInterval = int.Parse(displayIntervalTextBox.Text);

                Settings.Default.displayInterval = DisplayInterval;
                Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Set the correct display interval", "Warning");
                displayIntervalTextBox.Text = "";
            }
        }
    }
}
