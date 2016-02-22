using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    public class FakeGradeLevelUtilitiesProvider : IGradeLevelUtilitiesProvider
    {
        public virtual int FormatGradeLevelForSorting(string gradeLevel)
        {
            int res;
            switch (gradeLevel)
            {
                case "1st Grade":
                    res = 1;
                    break;
                case "2nd Grade":
                    res = 2;
                    break;
                case "3rd Grade":
                    res = 3;
                    break;
                case "4th Grade":
                    res = 4;
                    break;
                case "5th Grade":
                    res = 5;
                    break;
                case "6th Grade":
                    res = 6;
                    break;
                case "7th Grade":
                    res = 7;
                    break;
                case "8th Grade":
                    res = 8;
                    break;
                case "9th Grade":
                    res = 9;
                    break;
                case "10th Grade":
                    res = 10;
                    break;
                case "11th Grade":
                    res = 11;
                    break;
                case "12th Grade":
                    res = 12;
                    break;
                case "Postsecondary":
                    res = 13;
                    break;
                case "Early Education":
                    res = -3;
                    break;
                case "Infant/toddler":
                    res = -2;
                    break;
                case "Preschool/Prekindergarten":
                    res = -1;
                    break;
                case "Transitional Kindergarten":
                    res = 0;
                    break;
                case "Kindergarten":
                    res = 0;
                    break;
                case "Ungraded":
                    res = 14;
                    break;
                default:
                    res = 15;
                    break;
            }
            return res;
        }

        public virtual string FormatGradeLevelForDisplay(string gradeLevel)
        {
            string res;
            switch (gradeLevel)
            {
                case "1st Grade":
                    res = "1st";
                    break;
                case "2nd Grade":
                    res = "2nd";
                    break;
                case "3rd Grade":
                    res = "3rd";
                    break;
                case "4th Grade":
                    res = "4th";
                    break;
                case "5th Grade":
                    res = "5th";
                    break;
                case "6th Grade":
                    res = "6th";
                    break;
                case "7th Grade":
                    res = "7th";
                    break;
                case "8th Grade":
                    res = "8th";
                    break;
                case "9th Grade":
                    res = "9th";
                    break;
                case "10th Grade":
                    res = "10th";
                    break;
                case "11th Grade":
                    res = "11th";
                    break;
                case "12th Grade":
                    res = "12th";
                    break;
                case "Postsecondary":
                    res = "Post";
                    break;
                case "Infant/toddler":
                    res = "Inf";
                    break;
                case "Early Education":
                    res = "E-E";
                    break;
                case "Preschool/Prekindergarten":
                    res = "Pre";
                    break;
                case "Transitional Kindergarten":
                    res = "T-K";
                    break;
                case "Kindergarten":
                    res = "K";
                    break;
                case "Ungraded":
                    res = "U";
                    break;
                default:
                    res = "NA";
                    break;
            }
            return res;
        }

    }
}
