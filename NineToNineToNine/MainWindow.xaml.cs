using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
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
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly Calculator _calculator;
        private DateTime _start = DateTime.MinValue;
        private DateTime _lastRefresh = DateTime.MinValue;

        public MainWindow()
        {
            InitializeComponent();
            this.tf_startTime.Text = DateTime.Now.ToString();

            tf_W2_TextChanged(this, null);
            _calculator = new Calculator();
            _calculator.Progress += _calculator_Progress;
            _calculator.End += _calculator_End; ;
        }

        private void _calculator_Progress(object? sender, CalcEvent e)
        {
            var totalSeconds = DateTime.Now.Subtract(_lastRefresh).TotalSeconds;
            if (totalSeconds >= 2)
            {
                DispayResult(e);
                _lastRefresh = DateTime.Now;
            }

            this.Dispatcher.Invoke(() => { tf_interation.Text = e.QuotioenTop.ToString(); });
        }
        private void _calculator_End(object? sender, CalcEvent e)
        {
            DispayResult(e);
        }

        private void DispayResult(CalcEvent e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.tf_interation.Text = e.Interation.ToString("#,##0");
                if (chb_viewResult.IsChecked ?? false)
                    this.tf_Result.Text = e.Result.ToString("#,##0");
                tf_Digits.Text = e.Result.ToString().Length.ToString("#,##0");
                td_Duration.Text = (DateTime.Now.Subtract(_start)).ToString();
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            else
            {
                _start = DateTime.Now;

                tbStartStop.Text = "_Cancel";
                tbStart.Visibility = Visibility.Collapsed;
                tbStop.Visibility = Visibility.Visible;

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

                    int startIteration = 1;
                    BigInteger sum = w1;

                    await Task.Run(() =>
                            _calculator.StartCalcualte(
                            w1,
                            w2,
                            w3, startIteration, sum, filename, token),
                        token);
                }

                catch (OperationCanceledException ex)
                {

                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                    tbStartStop.Text = "_Starten";
                    tbStart.Visibility = Visibility.Visible;
                    tbStop.Visibility = Visibility.Collapsed;
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


        private void tf_W1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((tf_W3 != null) && (tf_oberePotenz != null))
                tf_oberePotenz.Text = Math.Pow(int.Parse(tf_W2.Text), int.Parse(tf_W3.Text)).ToString("#,##0");
        }

        private void tf_W2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((tf_W3 != null) && (tf_oberePotenz != null))
                tf_oberePotenz.Text = Math.Pow(int.Parse(tf_W2.Text), int.Parse(tf_W3.Text)).ToString("#,##0");
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
