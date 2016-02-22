using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Common
{
    public static class Base36Extensions
    {
        private static readonly char[] charArray = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
                                                           'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 
                                                           'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 
                                                           'u', 'v', 'w', 'x', 'y', 'z' };

        private static HashSet<string> reservedFileNames = new HashSet<string>{"con", "prn", "aux", "nul", 
                                                                         "com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9", 
                                                                         "lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"};

        public static string ConvertToBase36(this long input)
        {
            bool inputIsNegative = input < 0;
            input = Math.Abs(input); // make sure input is positive

            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(charArray[input % 36]);
                input /= 36;
            }

            var base36 = new string(result.ToArray());
            if (reservedFileNames.Contains(base36))
                base36 = "_" + base36;
            if (inputIsNegative)
                base36 = "-" + base36;
            return base36;
        }
    }
}
