using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        public string thePathToTheFile = "";
        public class Ranking
        {
            public string Words { get; set; }
            public int Count { get; set; }
        }

        public class RankingWithCipf
        {
            public string Words { get; set; }
            public int RealDistribution { get; set; }
            public double ZipfDistribution { get; set; }
            public double DevZ { get; set; }
        }     
        string hash1 = " ", hashOutput = " ", hashOutput2 = " ", hashOutput3 = " ";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursors.Margin = new Thickness((150 * index), 35, 0, 5);

            switch (index)
            {
                case 0:
                    //GridMain.Background = Brushes.Aquamarine;
                    SPRanged.Visibility = Visibility.Visible;
                    SPRD.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    //GridMain.Background = Brushes.Beige;
                    SPRanged.Visibility = Visibility.Hidden;
                    SPRD.Visibility = Visibility.Visible;
                    break;
            }
        }

        private string WriteFile()
        {
            string textFromFile = textBox1.Text;
            int index;
            //textFromFile = Regex.Replace(textFromFile.ToLower(), @"(?!="")""(?!"")", "");
            //textFromFile.ToCharArray().Where(n => !char.IsDigit(n)).ToArray();
            Regex MyRegex;
            if (RBR1.IsChecked == true)
                index = 1;
            else
                index = 2;
            switch (index)
            {
                case 1:
                    MyRegex = new Regex(@"[^a-z]", RegexOptions.IgnoreCase);
                    return MyRegex.Replace(@textFromFile.ToLower(), @" ");
                case 2:
                    MyRegex = new Regex(@"[^а-я]", RegexOptions.IgnoreCase);
                    return MyRegex.Replace(@textFromFile.ToLower(), @" ");

            }
            return "";
        }

#pragma warning disable IDE1006 // Стили именования
        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
#pragma warning restore IDE1006 // Стили именования
        {
            if (textBox1.Text != "" && MD5(textBox1.Text) != hash1)
            {
                hash1 = MD5(textBox1.Text);
                List<Ranking> rankings = new List<Ranking>();
                //string text = "cat!, cat cat cat cat cat cat cat! cat cat, dog dog dog dog dog"; text = Regex.Replace(text.ToLower(), "[-.?!)(,:]", "");
                string text = WriteFile();
                //string path = @"C:\\";
                /*using (FileStream fstream = File.OpenRead($"{path}note.txt"))
                {
                    // преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    //Console.WriteLine($"Текст из файла: {textFromFile}");
                    text = textFromFile.ToLower();
                }*/
                string[] arr = text.Split();
                string result = "";
                foreach (var val in arr)
                {
                    if (!result.Contains(val))
                    {
                        int count = 0;
                        for (var i = 0; i < arr.Length; i++)
                        {
                            if (arr[i] == val)
                                count++;
                        }
                        rankings.Add(new Ranking { Count = count, Words = val });
                        result += val + "_" + count + " ";
                    }
                }
                var SortRankings = rankings.OrderByDescending(u => u.Count);
                Grid.ItemsSource = SortRankings;
            }
            else
                MessageBox.Show("Пожалуйста, введите текст");
        }

        private void MI_ChooseFileText1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Document", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "Text documents (.txt)|*.txt" // Filter files by extension
            };

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                using (FileStream fstream = File.OpenRead($"{filename}"))
                {
                    //// преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.UTF8.GetString(array);
                    textBox1.Text = textFromFile;
                }
            }
        }

        private void MI_Clear1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
        }

        private string MD5(string str)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
               return BitConverter.ToString(
                  md5.ComputeHash(Encoding.UTF8.GetBytes(str))
                ).Replace("-", String.Empty);
            }
        }

        private void RankingWithCipfFunction()
        {        
            if (textBoxCipf1.Text != "" && MD5(textBoxCipf1.Text) != hashOutput)
            {
                hashOutput = MD5(textBoxCipf1.Text);
                List<RankingWithCipf> rankingWithCipf = new List<RankingWithCipf>();
                List<Ranking> rankings = new List<Ranking>();
                string textCipf1 = RankingCipf(textBoxCipf1);
                string[] arr = textCipf1.Split();
                string result = "";
                int number = 0;
                foreach (var val in arr)
                {
                    if (!result.Contains(val))
                    {
                        int count = 0;
                        for (var i = number; i < arr.Length; i++)
                        {
                            if (arr[i] == val)
                                count++;
                        }
                        rankings.Add(new Ranking { Count = count, Words = val });
                        result += val + "_" + count + " ";
                        number++;
                    }
                }
                int j = 1;
                var SortRankings = rankings.OrderByDescending(u => u.Count);
                foreach (var n in SortRankings)
                {
                    rankingWithCipf.Add(new RankingWithCipf { RealDistribution = n.Count, Words = n.Words, ZipfDistribution = n.Count / j, DevZ = Math.Abs(n.Count - (n.Count / j)) });
                    j++;
                }
                GridCipf1.ItemsSource = rankingWithCipf;
            }
            if (textBoxCipf2.Text != "" && MD5(textBoxCipf2.Text) != hashOutput2)
            {
                hashOutput2 = MD5(textBoxCipf2.Text);
                List<RankingWithCipf> rankingWithCipf2 = new List<RankingWithCipf>();
                List<Ranking> rankings2 = new List<Ranking>();
                string textCipf2 = RankingCipf(textBoxCipf2);
                string[] arr = textCipf2.Split();
                string result = "";
                foreach (var val in arr)
                {
                    if (!result.Contains(val))
                    {
                        int count = 0;
                        for (var i = 0; i < arr.Length; i++)
                        {
                            if (arr[i] == val)
                                count++;
                        }
                        rankings2.Add(new Ranking { Count = count, Words = val });
                        result += val + "_" + count + " ";
                    }
                }
                int j = 1;
                var SortRankings = rankings2.OrderByDescending(u => u.Count);
                foreach (var n in SortRankings)
                {
                    rankingWithCipf2.Add(new RankingWithCipf { RealDistribution = n.Count, Words = n.Words, ZipfDistribution = n.Count / j, DevZ = Math.Abs(n.Count - (n.Count / j)) });
                    j++;
                }
                GridCipf2.ItemsSource = rankingWithCipf2;
            }
            if (textBoxCipf3.Text != "" && MD5(textBoxCipf3.Text) != hashOutput3)
            {
                hashOutput3 = MD5(textBoxCipf3.Text);
                List<RankingWithCipf> rankingWithCipf3 = new List<RankingWithCipf>();
                List<Ranking> rankings3 = new List<Ranking>();
                string textCipf3 = RankingCipf(textBoxCipf3);
                string[] arr = textCipf3.Split();
                string result = "";

                foreach (var val in arr)
                {
                    if (!result.Contains(val))
                    {
                        int count = 0;
                        for (var i = 0; i < arr.Length; i++)
                        {
                            if (arr[i] == val)
                                count++;
                        }
                        //rankingWithCipf3.Add(new RankingWithCipf { RealDistribution = count, Words = val });//, ZipfDistribution = count / j, DevZ = Math.Abs(count - (count / j))
                        rankings3.Add(new Ranking { Count = count, Words = val });

                        result += val + "_" + count + " ";
                    }
                }
                int j = 1;
                var SortRankings = rankings3.OrderByDescending(u => u.Count);
                foreach (var n in SortRankings)
                {
                    rankingWithCipf3.Add(new RankingWithCipf { RealDistribution = n.Count, Words = n.Words, ZipfDistribution = n.Count / j, DevZ = Math.Abs(n.Count - (n.Count / j)) });
                    j++;
                }
                GridCipf3.ItemsSource = rankingWithCipf3;
            }
        }

        private void buttonCipf_Click(object sender, RoutedEventArgs e)
        {
            RankingWithCipfFunction();
        }

        private string RankingCipf(TextBox tb)
        {
            string textFromFile = tb.Text;
            int index;
            Regex MyRegex;
            //textFromFile = Regex.Replace(textFromFile.ToLower(), @"(?!="")""(?!"")", "");
            //textFromFile.ToCharArray().Where(n => !char.IsDigit(n)).ToArray();
            if (RB1.IsChecked == true)
                index = 1;
            else
                index = 2;
            switch (index)
            {
                case 1:
                    MyRegex = new Regex(@"[^a-z]", RegexOptions.IgnoreCase);
                    return MyRegex.Replace(@textFromFile.ToLower(), @" ");
                case 2:
                    MyRegex = new Regex(@"[^а-я]", RegexOptions.IgnoreCase);
                    return MyRegex.Replace(@textFromFile.ToLower(), @" ");

            }return "";
           //textFromFile.Where(c => !char.IsPunctuation(c)).Aggregate("", (current, c) => current + c);
        }

        private void MI_ChooseFileTextCipf1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Document", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "Text documents (.txt)|*.txt" // Filter files by extension
            };

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                using (FileStream fstream = File.OpenRead($"{filename}"))
                {
                    //// преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.UTF8.GetString(array);
                    textBoxCipf1.Text = textFromFile;
                }
            }
        }

        private void MI_ClearCipf1_Click(object sender, RoutedEventArgs e)
        {
            textBoxCipf1.Text = "";
        }

        private void MI_ChooseFileTextCipf2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                using (FileStream fstream = File.OpenRead($"{filename}"))
                {
                    //// преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.UTF8.GetString(array);
                    textBoxCipf2.Text = textFromFile;
                }
            }
        }

        private void MI_ClearCipf2_Click(object sender, RoutedEventArgs e)
        {
            textBoxCipf2.Text = "";
        }

        private void RB1_Checked(object sender, RoutedEventArgs e)
        {
            hashOutput = " "; hashOutput2 = " "; hashOutput3 = " ";
        }

        private void RBR1_Checked(object sender, RoutedEventArgs e)
        {
            hash1 = " ";
        }

        private void MI_ChooseFileTextCipf3_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                using (FileStream fstream = File.OpenRead($"{filename}"))
                {
                    //// преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.UTF8.GetString(array);
                    textBoxCipf3.Text = textFromFile;
                }
            }
        }

        private void MI_ClearCipf3_Click(object sender, RoutedEventArgs e)
        {
            textBoxCipf3.Text = "";
        }
    }
}