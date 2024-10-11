using System;
using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Helpers
{
    public static class StringHelper
    {
        public static string[] SplitAndReturnList(string input, char seperator)
        {
            var list = new List<string>();

            if (!string.IsNullOrWhiteSpace(input))
            {
                list.AddRange(input.Split(seperator));
            }

            return list.ToArray();
        }

        public static string GetCssClassName()
        {
            return $"back-color-{new Random().Next().ToString()}";
        }
    }
}