using System;
using System.Collections.Generic;
using System.Threading;

namespace Portfolio_WPF_App.Core.DataModel
{
    public static class DBEntryGenerator
    {
        public static List<List<object>> GetDbRows(int numberOfRows)
        {
            List<List<object>> rows = new List<List<object>>();

            for (int iter = 0; iter < numberOfRows; iter++)
            {
                List<object> values1 = new List<object>()
                {
                    0,
                    DateTime.Now.ToString(),
                    GetRandomName(),
                    GetRandomSureName(),
                    "Das ist eine laaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaa aaaaaaaaaaaaaa aaaaaaaaaaa aaaaaaaange Test Message"
                };
                rows.Add(values1);
            }
            return rows;
        }

        private static string GetRandomName(bool withTimeOut = true)
        {
            if (withTimeOut)
                Thread.Sleep(10);
            string name = "";

            List<string> maleNames = new List<string>() { "Liam", "Noah", "William", "James", "Logan", "Benjamin", "Mason", "Elijah", "Oliver", "Jacob" };
            List<string> femaleNames = new List<string>() { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Mia", "Charlotte", "Amelia", "Evelyn" };

            if (new Random().Next(0, 10) % 2 == 0)
            {
                name = maleNames[new Random().Next(0, maleNames.Count - 1)] + " ";
            }
            else
            {
                name = femaleNames[new Random().Next(0, femaleNames.Count - 1)] + " ";
            }
            return name;
        }

        private static string GetRandomSureName(bool withTimeOut = true)
        {
            if (withTimeOut)
                Thread.Sleep(10);
            string name = "";

            List<string> lastNames = new List<string>() { "Tappler", "Stacher", "Kolleger", "Floss", "Schoiswohl", "Christandl", "Zwanzger", "Hrab", "Pressl",
                                                          "Zach", "Pensold", "Schriebl"};
            name = lastNames[new Random().Next(0, lastNames.Count - 1)];
            return name;
        }
    }
}
