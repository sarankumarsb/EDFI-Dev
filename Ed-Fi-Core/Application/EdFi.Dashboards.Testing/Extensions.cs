// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace EdFi.Dashboards.Testing
{
    public static class Extensions
    {
        public static Guid ToGuid(this char charToRepeat)
        {
            if ((charToRepeat >= '0' && charToRepeat <= '9')
                || (charToRepeat >= 'a' && charToRepeat <= 'f')
                || (charToRepeat >= 'A' && charToRepeat <= 'F'))
                return new Guid(new string(charToRepeat, 32));

            throw new ArgumentException("charToRepeat must be a valid hexidecimal representation character (i.e. a number or a letter between 'a' and 'f').", "charToRepeat");
        }

        public static bool Within(this DateTime basis, double milliseconds, DateTime otherDate)
        {
            return Within(basis, TimeSpan.FromMilliseconds(milliseconds), otherDate);
        }

        public static bool Within(this DateTime basis, TimeSpan timeframe, DateTime otherDate)
        {
            TimeSpan ts = (basis - otherDate);
            if (Math.Abs(ts.TotalMilliseconds) > timeframe.TotalMilliseconds)
                throw new ArgumentOutOfRangeException(string.Format("Basis DateTime of '{0:s}' was not within '{1:c}' of '{2:s}'.", basis, timeframe, otherDate));

            return true;
        }

        public static string ToTabularText<T>(this IEnumerable<T> list)
        {
            return list.ToTabularText(null);
        }

        public static string ToTabularText<T>(this IEnumerable<T> list, string title)
        {
            StringBuilder sb = new StringBuilder();

// Only dump tabular text into unit test log output in Debug builds (to avoid generating copious amounts of text in the build server logs)
#if DEBUG
            sb.AppendLine(title);

            var columns = new List<List<string>>();
            var columnWidths = new List<int>();

            var properties = typeof(T).GetProperties();

            // Prepare headers
            foreach (var property in properties)
            {
                columns.Add(new List<string> { property.Name });
                columnWidths.Add(property.Name.Length);
            }

            // Process rows
            foreach (var item in list)
            {
                int col = 0;

                foreach (var property in properties)
                {
                    object value = property.GetValue(item, null);
                    string valueAsString = string.Empty;

                    if (value != null)
                        valueAsString = value.ToString();

                    columns[col].Add(valueAsString);

                    //if (valueAsString.Length > columnWidths[col])
                    columnWidths[col] = Math.Max(valueAsString.Length, columnWidths[col]);

                    col++;
                }
            }

            string dividerLine = " " + new string('-', columnWidths.Sum() + (columnWidths.Count() * 3));

            for (int r = 0; r < columns[0].Count; r++)
            {
                int col = 0;

                if (r == 0)
                    sb.AppendLine(dividerLine);

                sb.Append("| ");

                foreach (var column in columns)
                {
                    sb.Append(column[r].PadRight(columnWidths[col] + 1) + "| ");
                    col++;
                }

                sb.AppendLine();

                if (r == 0)
                    sb.AppendLine(dividerLine);
            }

            sb.AppendLine(dividerLine);
#endif

            return sb.ToString();
        }


        public static CapturingConstraint CaptureArgumentsFor<MOCK>(this MOCK mock, Expression<Action<MOCK>> methodExpression)
            where MOCK : class
        {
            var method = ReflectionHelper.GetMethod(methodExpression);
            var constraint = new CapturingConstraint();
            var constraints = new List<AbstractConstraint>();

            foreach (var arg in method.GetParameters())
            {
                constraints.Add(constraint);
            }

            mock.Expect(methodExpression.Compile()).Constraints(constraints.ToArray());

            return constraint;
        }
    }

    public class CapturingConstraint : AbstractConstraint
    {
        private ArrayList argList;

        private ArrayList ArgList
        {
            get { return argList ?? (argList = new ArrayList()); }
        }

        public override bool Eval(object obj)
        {
            ArgList.Add(obj);
            return true;
        }

        public T First<T>()
        {
            return ArgumentAt<T>(0);
        }

        public T ArgumentAt<T>(int position)
        {
            return (T)argList[position];
        }

        public override string Message
        {
            get { return ""; }
        }

        public T Second<T>()
        {
            return ArgumentAt<T>(1);
        }

        public bool MethodInvoked
        {
            get
            {
                return (argList != null);
            }
        }
    }
}