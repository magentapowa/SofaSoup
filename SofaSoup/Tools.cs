using System;
using System.Collections.Generic;
using System.Globalization;

namespace SofaSoupApp
{
    public static class Tools
    {
// Check if charachter is a symbol. (char.IsSymbol() is not appropriate because it misses some symbols
        public static int IsSymbol(char c)
        {
            List<char> charList = new List<char> { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '_', '=', '+', '`', '~', '[', ']', '{', '}', '|', '\\', ':', ';', '"', '\'', ',', '<', '.', '>', '/', '?' };
            return charList.IndexOf(c);
        }

// Build a table view.
// Used to preview a users info.
        public static List<string> BuildSingleRowTableView(string[] headers, string[] values)
        {
            int[] maxes = new int[headers.Length];
            List<string> result = new List<string>{"","",""};

            for (int i = 0; i < maxes.Length; i++)
            {
                maxes[i] = Math.Max(headers[i].Length, values[i].Length);
                result[0] += headers[i] +  " ".Times(maxes[i]-headers[i].Length);
                result[1] += "-".Times(maxes[i]);
                result[2] += values[i] + " ".Times(maxes[i] - values[i].Length);

                if (i != maxes.Length-1)
                {
                    result[0] += " | ";
                    result[1] += "-+-";
                    result[2] += " | ";
                }
            }
            return result;
        }


// Custom Datetime.TryParse()
        public static bool TryParseDate(string dateString, out DateTime date,bool hasHour=true)
        {
            string[] formats;
            if (hasHour)
            {
                formats = new string[]{
                "d/M/yyyy H:m",
                "d/M/yyyy h:mm",
                "dd/MM/yyyy hh:mm",
                "dd/M/yyyy hh:mm"};
            }
            else
            {
                formats = new string[]{
                "d/M/yyyy",
                "dd/MM/yyyy",
                "dd/M/yyyy"};
            }

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.CurrentUICulture, DateTimeStyles.None, out date))
            {
                return true;
            }
            return false;

                    
        }


    }
}
