using System;
using System.CodeDom;
using System.Diagnostics.Tracing;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace NineToNineToNine;

public partial class MainWindow
{

    public event EventHandler<CalcEvent> Progress = (sender, args) => { };

    internal async Task HeavyMethodAsync(int w1, int w2, int w3, int n, BigInteger sum, string filename,
    CancellationToken cancelToken)
    {
        DateTime start = DateTime.Now;
        BigInteger quotientTop = BigInteger.Pow(w2, w3);

        for (n = 1; n <= quotientTop - 1; n++)
        {
            sum = BigInteger.Multiply(sum, w1);

            if (n % 1000 == 0) //nur jedes 1000 mal aktualisieren
            {
                this.Dispatcher.Invoke(() =>
                {
                    Progress(this, new CalcEvent(n, sum));
                    this.tf_interation.Text = n.ToString("0,000");
                    if (chb_viewResult.IsChecked ?? false)
                        this.tf_Result.Text = sum.ToString("0,000");
                    tf_Digits.Text = sum.ToString().Length.ToString("0,000");
                    td_Duration.Text = (DateTime.Now.Subtract(start)).ToString();
                });
            }


            await WriteCurrentStatus(filename, n, quotientTop, sum);


            if (cancelToken.IsCancellationRequested)
            {
                await WriteCurrentStatus(filename, n, quotientTop, sum);
                cancelToken.ThrowIfCancellationRequested();
            }
        }

        await WriteCurrentStatus(filename, n, quotientTop, sum);

        this.Dispatcher.Invoke(() => { tf_interation.Text = quotientTop.ToString(); });
    }


    private async Task WriteCurrentStatus(string filename, int n, BigInteger quotientTop, BigInteger sum)
    {
        await File.WriteAllTextAsync(filename, n + "/" + quotientTop + "####" + sum.ToString());
    }
}

public class CalcEvent : EventArgs
{
    public CalcEvent()
    {
            
    }

    public CalcEvent(int interation, BigInteger result )
    {
        Interation = interation;
        Result = result;
    }

    public int Interation { get; set; }
    public BigInteger Result { get; set; }

}