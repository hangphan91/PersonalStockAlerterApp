using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocks.Mgr.Helpers
{
    public class WriteToText
    {
        public WriteToText(LogResult log)
        {
            string path = @"LogResult.txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    var headers = "";
                    var list = new List<string>();
                    foreach (PropertyInfo p in log.GetType().GetProperties())
                    {
                        list.Add(p.Name);
                    }
                    headers = string.Join(", ", list);
                    sw.WriteLine($"{headers}");
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                var list = new List<string>();
                foreach (PropertyInfo p in log.GetType().GetProperties())
                {
                    if(p.GetValue(log) == null)
                    {
                        list.Add("null");
                    }
                    else
                    {
                        list.Add(p.GetValue(log).ToString());
                    }
                }
                sw.WriteLine(string.Join(", ", list));
            }
        }
    }
}
