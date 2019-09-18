using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ImageProcessList = new List<ImageProcess>();
            ImageProcessList.Add(new ImageProcess());
            ImageProcessList.Add(new ImageProcessAsync());
            ImageProcessList.Add(new ImageProcessAsync2());
            ImageProcessList.Add(new ImageProcessAsync3());
            ImageProcessList.Add(new ImageProcessAsync4());
            ImageProcessList.Add(new ImageProcessAsync5());

            long firstTime = -1;
            ImageProcessList.ForEach(imageProcess =>
            {
                imageProcess.Run();
                if(firstTime == -1)
                {
                    firstTime = imageProcess.RunningTime;
                }
                else
                {
                    long secondTime = imageProcess.RunningTime;
                    Console.WriteLine($"節省約：{((firstTime - secondTime) / Convert.ToDouble(firstTime)).ToString("#0.00%")}");
                }
            });

            Console.Write("--程式已結束--");
            Console.ReadLine();
        }
    }
}
