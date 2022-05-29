using System.Linq;
using System;

class Result
{
    public static string pangrams(string s)
    {
        s = s.ToLower();
        var charArray = s.ToCharArray();
        for (int i = 97; i <= 122; i++)
        {
            var res = charArray.Where(x => (int)x == i);
            char tt = charArray.Where(x => (int)x == i).FirstOrDefault();
            Console.WriteLine(tt);
            if (tt.ToString() == "\0")
            {
                return "not pangram";
            }
        }
        return "pangram";
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        #region Pangrams
        /*string s = Console.ReadLine();
        string result = Result.pangrams(s);
        Console.WriteLine(result);*/
        #endregion
        int n = 10;
        for (int i = 1; i <= n; i++)
        {
            if (n % 3 == 0 && n % 5 == 0)
            {
                Console.WriteLine("FizzBuzz");
            }
            else if (n % 3 == 0 && n % 5 != 0)
            {
                Console.WriteLine("Fizz");
            }
            else if (n % 3 != 0 && n % 5 == 0)
            {
                Console.WriteLine("Buzz");
            }
            else
            {
                Console.WriteLine(n); ;
            }

        }
        

        /*PdfSharp.Pdf.PdfDocument pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf("<html><h1>hello</h1></html>"
            , PageSize.Letter);
        pdf.Save("document.pdf");
        Console.ReadLine();*/
    }
}
