using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;


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

                tf_Ergebnis.Text = "";
                tf_Startzeit.Text = DateTime.Now.ToString();
                tf_Endzeit.Text = "";
                int w1 = int.Parse(tf_W1.Text);
                int w2 = int.Parse(tf_W2.Text);
                int w3 = int.Parse(tf_W3.Text);

                tf_interation.Text = "starting"; // UI Thread
                try
                {
                    await Task.Run(() =>
                        HeavyMethodAsync(
                            w1,
                            w2,
                            w3,
                            tf_interation,
                            tf_Ergebnis,
                            tf_Stellen, token), token);
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
                tf_Endzeit.Text = DateTime.Now.ToString();
            }
        }

        internal async Task HeavyMethodAsync(int w1, int w2, int w3, TextBox tfInteration, TextBox tfSumme, TextBox tf_Stellen, CancellationToken cancelToken)
        {
            DateTime start = DateTime.Now;

            BigInteger sum = w1;
            BigInteger quotientTop = BigInteger.Pow(w2, w3);

            var folder = CreateAndGetFolder();

            var filename = Path.Combine(folder, $"{w1} pow {w2} pow {w3}.txt");

            int n;
            for (n = 1; n <= quotientTop - 1; n++)
            {
                sum = BigInteger.Multiply(sum, w1);

                if (n % 1000 == 0) //nur jedes 1000 mal aktualisieren
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        tfInteration.Text = n.ToString("0,000");
                        if (chb_viewResult.IsChecked ?? false)
                            tfSumme.Text = sum.ToString("0,000");
                        tf_Stellen.Text = sum.ToString().Length.ToString("0,000");
                        tf_Dauer.Text = (DateTime.Now.Subtract(start)).Minutes.ToString("0,000");
                    });


                    await WriteCurrentStatus(filename, n, quotientTop, sum);
                }

                if (cancelToken.IsCancellationRequested)
                {
                    await WriteCurrentStatus(filename, n, quotientTop, sum);
                    cancelToken.ThrowIfCancellationRequested();
                }
            }

            await WriteCurrentStatus(filename, n, quotientTop, sum);

            this.Dispatcher.Invoke(() =>
            {
                tfInteration.Text = quotientTop.ToString();
            });
        }

        private string CreateAndGetFolder()
        {
            var folder = Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NineToNineToNine");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        private async Task WriteCurrentStatus(string filename, int n, BigInteger quotientTop, BigInteger sum)
        {
            await File.WriteAllTextAsync(filename, n + "/" + quotientTop + "####" + sum.ToString());
        }

        private void tf_W2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((tf_W3 != null) && (tf_oberePotenz != null))
                tf_oberePotenz.Text = Math.Pow(int.Parse(tf_W2.Text), int.Parse(tf_W3.Text)).ToString("0,000");
        }

        private void chb_viewResult_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!chb_viewResult.IsChecked ?? false)
                tf_Ergebnis.Text = "";
        }

        private void sf_ShowResultFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", CreateAndGetFolder());
        }
    }
}
