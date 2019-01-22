using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Utils
{
    public class TestOutput
    {
        public static void MyCheckAndLogTestResult(string Message, bool Check, bool warn = false)
        {
            Console.Write(Message);
            var pc = Console.ForegroundColor;
            if (warn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("<<WARN>>");
            }
            else
            {
                if (Check)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(" PASSED.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(" FAILED!");
                }
            }
            Console.ForegroundColor = pc;
            if (!warn && !Check)
            {
                throw new Exception($"at Least One Test Failed({Message})!!!");
            }
        }
    }
}
