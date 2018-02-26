using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClaimsReservingApp;
using System.Collections.Generic;
using System.IO;

namespace ClaimReservingTest
{
    // Ideally unit test cases should 100% code coverage. 
    // Due to time constraints I am adding some methods.
    // TODO: Test cases to cover
    // 1. Test for multiple splitters eg: { ',', ';', '-', ':', '/', '@', '!', '#', '$', '%', '&', '*', '+' }
    // 2. Test for re-ordering the columns in the input file
    // 3. Test for small and upper case of the column names
    // 4. Test for more number of origin years
    // 5. Test for multiple products with more number of origin years and re-ordering of columns
    [TestClass]
    public class ClaimReservingTestClass
    {
        [TestMethod]
        public void Test_ReadOnlyFile()
        {
            string path = @"..\..\Store\ClaimsReserving.txt";

            string f_path = ClaimReserving.ReadOnlyFile(path);
            Assert.AreEqual(path, f_path);
        }

        [TestMethod]
        public void Test_GetValuesFromFile()
        {
            string path = @"..\..\Store\ClaimsReserving.txt";
           
            List<string> claimResult = new List<string>();
            claimResult.Add("1990, 4");
            claimResult.Add("Comp, 0, 0, 0, 0, 0, 0, 0, 110, 280, 200");
            claimResult.Add("Non-Comp, 45.2, 110, 110, 147, 50, 125, 150, 55, 140, 100");

            List<string> result = ClaimReserving.GetValuesFromFile(path);
            result.ForEach(o => Assert.IsTrue(result.Contains(o)));
        }

        [TestMethod]
        public void Test_WriteOutputToFile()
        {
            List<string> claimResult = new List<string>();
            claimResult.Add("1990, 4");
            claimResult.Add("Comp, 0, 0, 0, 0, 0, 0, 0, 110, 280, 200");
            claimResult.Add("Non-Comp, 45.2, 110, 110, 147, 50, 125, 150, 55, 140, 100");

            ClaimReserving.WriteOutputToFile(claimResult);

            string path = @"..\..\Store\Outputfile.txt";
            Assert.IsTrue(File.Exists(path));
        }


    }
}
