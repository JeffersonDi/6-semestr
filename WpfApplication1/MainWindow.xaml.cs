using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        public string thePathToTheFile = "";
        public string textR = "", text2 = "", text3 = "";
        public bool CB1Check = false;

        public class test
        {
            public string Words { get; set; }
        }

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
            //textFromFile.t
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
                    textFromFile = Regex.Replace(@textFromFile, "[ ]+", " ");
                    return MyRegex.Replace(@textFromFile.ToLower(), @" ");
                case 2:
                    MyRegex = new Regex(@"[^а-я]", RegexOptions.IgnoreCase);
                    textFromFile = Regex.Replace(textFromFile, "[ ]+", " ");
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
                string[] arr = text.Split(' ');
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
            double DeVZ1 = 0, DeVZ2 = 0, DeVZ3 = 0, DistanceRTo2 = 0, DistanceRTo3;
            var th1 = new Thread(RankingTextR);
            var th2 = new Thread(RankingText2);
            var th3 = new Thread(RankingText3);
            List<Thread> ThreadList1 = new List<Thread>
            {
                th1,
                th2,
                th3
            };
            th1.Start();
            //th1.Join();
            th2.Start();
            //th2.Join();
            th3.Start();
            //th3.Join();

            Wait(ThreadList1);
            //Console.WriteLine("Все ок");

            void RankingTextR()
            {
                if (textR != "" && MD5(textR) != hashOutput)
                {
                    Console.WriteLine("Поток 1 начало");
                    hashOutput = MD5(textR);
                    List<RankingWithCipf> rankingWithCipf = new List<RankingWithCipf>();
                    List<Ranking> rankings = new List<Ranking>();
                    string textCipf1 = RankingCipf(textR, CB1Check);
                    textCipf1 = Regex.Replace(@textCipf1, @"\s+", " ");textCipf1.Trim();

                    string[] arr = textCipf1.Split(' ');
                    List<string> ls = new List<string>(arr);
                    /*ls.Sort();
                    foreach (string val in ls.Distinct())
                    {
                        rankings.Add(new Ranking { Count = ls.Where(x => x == val).Count(), Words = val });
                        Console.WriteLine(val + " - " + ls.Where(x => x == val).Count() + " раз");
                    }*/

                 /*
                while (true)//(var LS in ls)
                {
                    if (ls.FindAll((x) => x == ls.ElementAt(0)).Count != 0)
                    {
                        //int indexFirst = ls.FindIndex((x)=>x==ls.ElementAt(0)); int indexLast = ls.FindLastIndex((x)=>x== ls.ElementAt(0));

                        foreach (var LS in buf)
                        {
                            //Console.WriteLine(LS);
                        }
                        rankings.Add(new Ranking { Count = buf.Count(), Words = ls.ElementAt(0) });
                        j++;
                        ls.RemoveAll((x) => x == ls.ElementAt(0));

                        //Console.WriteLine(LS);
                    }
                    else
                        break;
                }*/
                /*foreach (var LS in rankings)
                {
                    Console.WriteLine(LS);
                }*/

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
                //var SortRankings = rankings.OrderBy(u => u.Words);
                foreach (var n in SortRankings)
                {
                    rankingWithCipf.Add(new RankingWithCipf { RealDistribution = n.Count, Words = n.Words, ZipfDistribution = n.Count / j, DevZ = Math.Abs(n.Count - (n.Count / j)) });
                    DeVZ1 += Math.Abs(n.Count - (n.Count / j));
                    j++;
                }
                //Console.WriteLine("Сумма отклонений: " + DeVZ1.ToString());
                DeVZ1 /= j;
                //Console.WriteLine("Колличество уникальных слов в тексте: " + j);
                //Console.WriteLine("Нормализация: " + DeVZ1.ToString());
                //GridCipf1.ItemsSource = rankingWithCipf;

                Dispatcher.BeginInvoke(new ThreadStart(delegate { GridCipf1.ItemsSource = rankingWithCipf;}));
                Console.WriteLine("Поток 1 выполнился");
            }
                else
                    Console.WriteLine("Пустая строка №1");
            }
            void RankingText2()
            {
                
                if (text2 != "" && MD5(text2) != hashOutput2)
                {
                    Console.WriteLine("Поток 2 Начало");
                    hashOutput2 = MD5(text2);
                    List<RankingWithCipf> rankingWithCipf2 = new List<RankingWithCipf>();
                    List<Ranking> rankings2 = new List<Ranking>();
                    string textCipf2 = RankingCipf(text2, CB1Check);
                    textCipf2 = Regex.Replace(@textCipf2, @"\s+", " "); textCipf2.Trim();
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
                        DeVZ2 += Math.Abs(n.Count - (n.Count / j));
                        j++;
                    }
                    DeVZ2 /= j;
                    //GridCipf2.ItemsSource = rankingWithCipf2;
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { GridCipf2.ItemsSource = rankingWithCipf2; }));
                    Console.WriteLine("Поток 2 выполнился");
                }
                else
                    Console.WriteLine("Пустая строка №2");
            }

            void RankingText3()
            {
                
                if (text3 != "" && MD5(text3) != hashOutput3)
                {
                    Console.WriteLine("Поток 3 начало");
                    hashOutput3 = MD5(text3);
                    List<RankingWithCipf> rankingWithCipf3 = new List<RankingWithCipf>();
                    List<Ranking> rankings3 = new List<Ranking>();
                    string textCipf3 = RankingCipf(text3, CB1Check);
                    textCipf3 = Regex.Replace(@textCipf3, @"\s+", " "); textCipf3.Trim();
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
                        DeVZ3 += Math.Abs(n.Count - (n.Count / j));
                        j++;
                    }
                    DeVZ3 /= j;
                    Console.WriteLine("Поток 3 Конец");
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { GridCipf3.ItemsSource = rankingWithCipf3; }));
                    //GridCipf3.ItemsSource = rankingWithCipf3;
                }
                else
                    Console.WriteLine("Пустая строка №3");
            }

            void Wait(List<Thread> ThreadList)
            {
                while (true)
                {
                    int WorkCount = 0;

                    for (int i = 0; i < ThreadList.Count; i++)
                    {
                        WorkCount += (ThreadList[i].IsAlive) ? 0 : 1;
                    }

                    if (WorkCount == ThreadList.Count)
                        break;
                }
            }
            if (DeVZ1 != 0 && DeVZ2 != 0 && DeVZ3 != 0)
            {
                DistanceRTo2 = DeVZ2 - DeVZ1;
                DistanceRTo3 = DeVZ3 - DeVZ1;
                double input;
                if (DistanceRTo2 < DistanceRTo3)
                {
                    input = (DistanceRTo3 - DistanceRTo2) / (Math.Abs(DistanceRTo2)) * 100;
                    //Console.WriteLine("Дистанция между R и 2: " + DistanceRTo2.ToString() + "\n Дистаниция между R и 3: " + DistanceRTo3.ToString());
                    MessageBox.Show("Расстояние между 3 и R текстами  на " + input.ToString() + "% меньше чем расстояние между 2 и R текстами.\n Вывод: скорее всего 3 текст был написан автором эталонного текста");
                }
                else
                {
                    input = (DistanceRTo2 - DistanceRTo3) / (Math.Abs(DistanceRTo3)) * 100;
                    //Console.WriteLine("Дистанция между R и 2: " + DistanceRTo2.ToString() + "\n Дистаниция между R и 3: " + DistanceRTo3.ToString());
                    MessageBox.Show("Расстояние между 2 и R текстами  на " + input.ToString() + "% меньше чем расстояние между 3 и R текстами.\n Вывод: скорее всего 2" +
                        " текст был написан автором эталонного текста");
                }                 
            }
        }

        private void buttonCipf_Click(object sender, RoutedEventArgs e)
        {
            textR = textBoxCipf1.Text;
            text2 = textBoxCipf2.Text;
            text3 = textBoxCipf3.Text;
            CB1Check = RB1.IsChecked.Value;
            /*var thread = new Thread(RankingWithCipfFunction);
            thread.Start();*/
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
        private string RankingCipf(string tb, bool RB1Checked)
        {
            string textFromFile = tb;
            int index;
            Regex MyRegex;
            //textFromFile = Regex.Replace(textFromFile.ToLower(), @"(?!="")""(?!"")", "");
            //textFromFile.ToCharArray().Where(n => !char.IsDigit(n)).ToArray();
            if (RB1Checked == true)
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