using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NineToNineToNine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource? _cancellationTokenSource;
        public MainWindow()
        {
            InitializeComponent();
            tf_W2_TextChanged(this, null);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            else
            {
                sf_start.Content = "_Cancel";
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                tf_Result.Text = "";
                tf_startTime.Text = DateTime.Now.ToString();
                tf_EndTime.Text = "";
                int w1 = int.Parse(tf_W1.Text);
                int w2 = int.Parse(tf_W2.Text);
                int w3 = int.Parse(tf_W3.Text);

                tf_interation.Text = "starting"; // UI Thread
                try
                {
                    var folder = CreateAndGetFolder();
                    string filename = Path.Combine(folder, $"{w1} pow {w2} pow {w3}.txt");

                    int n = 1;
                    BigInteger sum = w1;

                    await Task.Run(() =>
                        HeavyMethodAsync(
                            w1,
                            w2,
                            w3, n, sum, filename, token), token);
                }

                catch (OperationCanceledException ex)
                {

                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                    sf_start.Content = "_Starten";
                }
                tf_EndTime.Text = DateTime.Now.ToString();
            }
        }

        private string CreateAndGetFolder()
        {
            var folder = Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NineToNineToNine");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }



        private void tf_W2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((tf_W3 != null) && (tf_oberePotenz != null))
                tf_oberePotenz.Text = Math.Pow(int.Parse(tf_W2.Text), int.Parse(tf_W3.Text)).ToString("0,000");
        }

        private void chb_viewResult_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!chb_viewResult.IsChecked ?? false)
                tf_Result.Text = "";
        }

        private void sf_ShowResultFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", CreateAndGetFolder());
        }
    }
}
