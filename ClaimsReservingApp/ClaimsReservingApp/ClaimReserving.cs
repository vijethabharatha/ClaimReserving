using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ClaimsReservingApp
{
    public class ClaimReserving
    {
        private static string p_filePath;
        private static char[] possibleDelimters = new char[] { ',', ';', '-', ':', '/', '@', '!', '#', '$', '%', '&', '*', '+' };
        private static char delimiter = ' ';
        public static string ReadOnlyFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File path does not exist.");
            }

            return p_filePath = filePath;
        }
        public static void Main(string[] args)
        {            
            string path =  @"..\..\Store\ClaimsReserving.txt";
            ReadOnlyFile(path);
           
            List<string> result = GetValuesFromFile(path);
            if (result != null && result.Count() > 0)
            {
                WriteOutputToFile(result);
            }
         }

        public static List<string> GetValuesFromFile(string p_filePath)
        {
            // The columns can be in any order and with any different names 
            // but only consideration is, it should at least be close enough to eg below
            // 1. prod (product, products etc)
            // 2. ori ( origin year, ori. yr., ori. year etc...)
            // 3. dev (dev year, dev. yr, etc...)
            // 4. inc (incremental value, inc. val, etc...)
            string colProduct = "", colOriginYear = "", colDevelopmentYear = "", colIncrementalvalue = "";

            List<Dictionary<string, object>> claims = new List<Dictionary<string, object>>();
            string[] modelObject = null;

            StreamReader sr = null;
            List<string> claimResult = new List<string>();
            string[] data = null;
            
            string line = "";
            try
            {
                int cnt = 0;
                sr = new StreamReader(p_filePath);

                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();

                    if (line != null && line.Trim() != String.Empty)
                    {
                        // find delimiter
                        for (var x = 0; x < possibleDelimters.Length; x++)
                        {
                            var d = line.Split(possibleDelimters[x]);
                            if (d.Length > 1)
                            {
                                delimiter = Convert.ToChar(possibleDelimters[x]);
                                break;
                            }

                        }
                        data = line.Split(delimiter);
                        if (data != null)
                        {
                            if (++cnt == 1)
                            {
                                modelObject = data;
                                for (var i = 0; i < data.Length; i++)
                                {
                                    if (data[i] != null)
                                    {
                                        var col = data[i].Trim();
                                        if (col.ToLower().Contains("prod"))
                                            colProduct = col;
                                        else if (col.ToLower().Contains("ori"))
                                            colOriginYear = col;
                                        else if (col.ToLower().Contains("dev"))
                                            colDevelopmentYear = col;
                                        else if (col.ToLower().Contains("inc"))
                                            colIncrementalvalue = col;
                                    }
                                }
                                continue;
                            }

                            if (data.Length > 0)
                            {
                                Dictionary<string, object> claim = new Dictionary<string, object>();
                                for (var i = 0; i < data.Length; i++)
                                {
                                    if (modelObject[i] != null && data[i] != null)
                                        claim.Add(modelObject[i].Trim(), data[i].Trim());
                                }
                                claims.Add(claim);
                            }
                        }
                    }
                }

                if (claims != null)
                {
                    var sel = claims.Select(s => s[colProduct]);
                    var products = sel != null ? sel.Distinct().ToList() : null;

                    sel = claims.Select(s => s[colOriginYear]);
                    var originYears = sel != null ? sel.Distinct().OrderBy(o => o).ToList() : null;

                    if (products != null && originYears != null)
                    {
                        claimResult.Add(originYears.FirstOrDefault().ToString() + ", " + originYears.Count().ToString());
                        products.ForEach(p =>
                        {
                            var result = p != null ? p.ToString() : "";
                            originYears.ForEach(o =>
                            {
                                var product = claims.Where(s => s[colProduct].Equals(p));
                                var devYears = product.Where(c => c[colOriginYear].Equals(o));

                                if (devYears != null && devYears.Count() > 0)
                                {
                                    var count = 0.0;
                                    originYears.ForEach(i =>
                                    {
                                        var res = devYears.Where(c => c[colDevelopmentYear].Equals(i));
                                        if (res.Count() > 0)
                                        {
                                            count += double.Parse(res.FirstOrDefault()[colIncrementalvalue].ToString());
                                            result += ", " + count.ToString();
                                        }
                                        else if (count > 0)
                                        {
                                            result += ", " + count.ToString();
                                        }
                                    });
                                }
                                else
                                {
                                    var maxOriginYear = originYears.Max();
                                    var balance = Int32.Parse(maxOriginYear.ToString()) - Int32.Parse(o.ToString());
                                    for (var x = 0; x <= balance; x++)
                                    {
                                        result += ", 0";
                                    }
                                }
                            });
                            claimResult.Add(result);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap any exception that occurs in reading the file and return null.
                claimResult = null;
                Console.WriteLine(ex);
            }
            finally
            {
                if (sr != null) { sr.Close(); }
            }
            return claimResult;
        }

        public static void WriteOutputToFile(List<string> resultData)
        {
            string path = @"..\..\Store\Outputfile.txt";
           
            using (TextWriter tw = new StreamWriter(path))
            {
                foreach (String result in resultData)
                    tw.WriteLine(result);
            }
        }
    }
}
