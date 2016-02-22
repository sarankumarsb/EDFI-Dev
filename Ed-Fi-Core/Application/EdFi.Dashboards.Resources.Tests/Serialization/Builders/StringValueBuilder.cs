using System;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
   public class StringValueBuilder : IValueBuilder
   {
       public static bool GenerateNulls = true;
       public static bool GenerateEmptyStrings = true;

       private int counter = 0;
       private int index = 0;

       public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
       {
           value = null;

           if (targetType != typeof(string))
               return false;

           if (index == 1)
           {
               if (!GenerateEmptyStrings)
                   IncrementIndex();
           }

           if (index == 2)
           {
               if (!GenerateNulls)
                   IncrementIndex();
           }
               
           switch (index)
           {
               case 0:
                   value = "String" + (++counter);
                   break;
               case 1:
                   value = string.Empty;
                   break;
               case 2:
                   value = null;
                   break;
           }

           IncrementIndex();

           return true;
       }

       public void Reset()
       {
           counter = 0;
           index = 0;
       }

       private void IncrementIndex()
       {
           index = (index + 1) % 3;
       }

       public TestObjectFactory Factory { get; set; }
    }
}
