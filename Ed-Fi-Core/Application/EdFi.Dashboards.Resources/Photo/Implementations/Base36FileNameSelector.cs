// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations
{
    /// <summary>
    /// Creates a Base36 name for a file. This is useful when the images will be stored on the file system
    /// without the dashboard security layer filtering requests.
    /// </summary>
    public class Base36FileNameSelector : IFileNameSelector
    {
        public string Get(Identifier identifier, PhotoCategory photoCategory)
        {
            var bucket = Math.Abs(identifier.Id.GetHashCode()) % 256;
            var imageIdStr = Encode(identifier.Id);
            var newFileName = String.Format("{0}.{1}", imageIdStr, "jpg");
            return String.Format("Images\\People\\{0}\\{1}\\{2}", photoCategory, bucket, newFileName);
        }

        private static char[] charArray = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static List<string> reservedFileNames = new List<string>{"con", "prn", "aux", "nul", 
                                                                         "com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9", 
                                                                         "lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"};
        /// <summary>
        /// Encode the given number into a Base36 string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static String Encode(long input)
        {
            var inputIsNegative = input < 0;
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
