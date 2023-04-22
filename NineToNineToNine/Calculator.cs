using System;
using System.CodeDom;
using System.Diagnostics.Tracing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Threading.Tasks;

namespace NineToNineToNine;

public class Calculator
{
    public event EventHandler<CalcEvent> Progress = (sender, args) => { };
    public event EventHandler<CalcEvent> End = (sender, args) => { };

    internal async Task StartCalcualte(int w1, int w2, int w3, int startIteration, BigInteger sum, string filename, CancellationToken cancelToken)
    {
        DateTime start = DateTime.Now;
        BigInteger quotientTop = BigInteger.Pow(w2, w3);
        int n;

        for (n = startIteration; n <= quotientTop - 1; n++)
        {
            sum = BigInteger.Multiply(sum, w1);
            Progress(this, new CalcEvent(n, quotientTop, sum));

            await WriteCurrentStatus(filename, n, quotientTop, sum);

            if (cancelToken.IsCancellationRequested)
                cancelToken.ThrowIfCancellationRequested();
        }

        await WriteCurrentStatus(filename, n, quotientTop, sum);
        End(this, new CalcEvent(n, quotientTop, sum));
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

    public CalcEvent(int interation, BigInteger quotioenTop, BigInteger result)
    {
        Interation = interation;
        QuotioenTop = quotioenTop;
        Result = result;
    }

    public int Interation { get; set; }
    public BigInteger Result { get; set; }
    public BigInteger QuotioenTop { get; set; }
}