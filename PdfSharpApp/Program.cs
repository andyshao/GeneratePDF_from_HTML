using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharpApp.Models;
using TheArtOfDev.HtmlRenderer.PdfSharp;


namespace PdfSharpApp
{
    class Program
    {
        private static StringBuilder htmlText = new StringBuilder();
        private static string ReplaceHtmlValues(string tag, bool isHtml)
        {
            string returnValue = string.Empty;
            tag = tag.Trim();

            switch (tag)
            {
                case "$Employee$":
                    returnValue = "Employee Name";
                    break;
            }
            return returnValue;
        }

        public static string ConvertHtmlToString(TextReader streamToRead, bool isHtml)
        {
            StringBuilder body = new StringBuilder();
            StringBuilder nextTag = new StringBuilder();
            bool inTag = false;
            char nextCharacter = char.MinValue;
            char tagStart = '$';

            while (streamToRead.Peek() >= 0)
            {
                nextCharacter = Convert.ToChar(streamToRead.Peek());
                if (nextCharacter.Equals(tagStart)) inTag = !inTag;

                if (inTag)
                {
                    nextTag.Append(Convert.ToChar(streamToRead.Read()));
                    if (nextTag.Length >= 50)
                    {
                        body.Append(nextTag.ToString());
                        nextTag.Length = 0;
                        inTag = false;
                    }
                }
                else if (nextTag.Length > 0)
                {
                    if (nextCharacter.Equals(tagStart)) nextTag.Append(Convert.ToChar(streamToRead.Read()));
                    body.Append(ReplaceHtmlValues(nextTag.ToString(), isHtml));
                    nextTag.Length = 0;
                }
                else
                {
                    body.Append(Convert.ToChar(streamToRead.Read()));
                }
            }

            return body.ToString();
        }

        static void BuildModel()
        {
            CustomerDetails customerDetails = new CustomerDetails();
            customerDetails.Quantity1 = 10;
            customerDetails.ItemCode1 = "Sugar50";
            customerDetails.Type1 = "food";
            customerDetails.ISBN1 = 123124455;

            customerDetails.Quantity2 = 20;
            customerDetails.ItemCode2 = "Coffee30";
            customerDetails.Type2 = "powder";
            customerDetails.ISBN2 = 23215444;

            customerDetails.Quantity3 = 30;
            customerDetails.ItemCode3 = "Tea100";
            customerDetails.Type3 = "powder";
            customerDetails.ISBN3 = 32232332;

            htmlText.Replace("{{Quantity1}}", (customerDetails.Quantity1).ToString());
            htmlText.Replace("{{ItemCode1}}", customerDetails.ItemCode1);
            htmlText.Replace("{{Type1}}", customerDetails.Type1);
            htmlText.Replace("{{ISBN1}}", (customerDetails.ISBN1).ToString());

            htmlText.Replace("{{Quantity2}}", (customerDetails.Quantity2).ToString());
            htmlText.Replace("{{ItemCode2}}", customerDetails.ItemCode2);
            htmlText.Replace("{{Type2}}", customerDetails.Type2);
            htmlText.Replace("{{ISBN2}}", (customerDetails.ISBN2).ToString());

            htmlText.Replace("{{Quantity3}}", (customerDetails.Quantity3).ToString());
            htmlText.Replace("{{ItemCode3}}", customerDetails.ItemCode3);
            htmlText.Replace("{{Type3}}", customerDetails.Type3);
            htmlText.Replace("{{ISBN3}}", (customerDetails.ISBN3).ToString());
        }

        static List<ProductDetail> BuildModel_Main()
        {
            List<ProductDetail> products = new List<ProductDetail>();
            ProductDetail product = new ProductDetail();
            product.Quantity = 10;
            product.ItemCode = "Sugar50";
            product.Type = "food";
            product.ISBN = 123124455;
            products.Add(product);

            ProductDetail product2 = new ProductDetail();
            product2.Quantity = 9;
            product2.ItemCode = "Sugdddfar50";
            product2.Type = "CPT Professional (Spiral)";
            product2.ISBN = 123124555;
            products.Add(product2);

            ProductDetail product3 = new ProductDetail();
            product3.Quantity = 100;
            product3.ItemCode = "dsfsadf";
            product3.Type = "CPT Professional (Spiral)";
            product3.ISBN = 1124455;
            products.Add(product3);

            ProductDetail product4 = new ProductDetail();
            product4.Quantity = 100;
            product4.ItemCode = "dsfsadf";
            product4.Type = "CPT Professional (Spiral)";
            product4.ISBN = 1124455;
            products.Add(product4);

            ProductDetail product5 = new ProductDetail();
            product5.Quantity = 100;
            product5.ItemCode = "dsfsadf";
            product5.Type = "CPT Professional (Spiral)";
            product5.ISBN = 1124455;
            products.Add(product5);

            return products;
        }

        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //StringBuilder htmlText = new StringBuilder();C:\Users\sgampa\Desktop
            using (StreamReader template = new StreamReader("../../../Index.html"))
            {
                htmlText.Append(ConvertHtmlToString(template, false));
                template.Close();
            }

            //BuildModel();
            CreateProductsTable();
            GeneratePDF();

        }

        private static void CreateProductsTable()
        {
            StringBuilder productTableHtml = new StringBuilder();

            List<ProductDetail> products = BuildModel_Main();
            foreach (ProductDetail product in products)
            {
                string txt = "<tr><td class=\"add-border padding-for-cell\"><h4 class=\"m-0 p-0\">{{Quantity}}</h4 ></td><td class= \"add-border padding-for-cell\" ><h4 class= \"m-0 p-0\" >{{ItemCode}}</h4 ></td><td class= \"add-border padding-for-cell\" > <h4 class= \"m-0 p-0\" >{{Type}}</h4 ><p class= \"m-0 p-0\" > ISBN:{{ISBN}}</p ></td></tr > ";

                txt = txt.Replace("{{Quantity}}", product.Quantity.ToString());
                txt = txt.Replace("{{ItemCode}}", product.ItemCode);
                txt = txt.Replace("{{Type}}", product.Type);
                txt = txt.Replace("{{ISBN}}", product.ISBN.ToString());

                productTableHtml.Append(txt);
            }
            htmlText.Replace("{{productDetailsContent}}", productTableHtml.ToString());
        }

        private static void GeneratePDF()
        {
            //Configure page settings
            var configurationOptions = new PdfGenerateConfig();

            //Page is in Landscape mode, other option is Portrait
            configurationOptions.PageOrientation = PdfSharp.PageOrientation.Portrait;

            //Set page type as Letter. Other options are A4 …
            configurationOptions.PageSize = PdfSharp.PageSize.A4;

            //This is to fit Chrome Auto Margins when printing.Yours may be different
            configurationOptions.MarginBottom = 19;
            configurationOptions.MarginLeft = 2;
            //configurationOptions.MarginRight = 5;

            //The actual PDF generation
            var OurPdfPage = PdfGenerator.GeneratePdf(htmlText.ToString(), configurationOptions);
            //SetFooter(OurPdfPage);

            OurPdfPage.Save("Output.pdf");
        }

        private static void SetFooter(PdfDocument OurPdfPage)
        {
            //Setting Font for our footer
            XFont font = new XFont("Segoe UI,Open Sans, sans-serif, serif", 7);
            XBrush brush = XBrushes.Black;

            //Loop through our generated PDF pages, one by one
            for (int i = 0; i < OurPdfPage.PageCount; i++)
            {
                //Get each page
                PdfSharp.Pdf.PdfPage page = OurPdfPage.Pages[i];

                //Create rectangular area that will hold our footer – play with dimensions according to your page'scontent height and width
                XRect layoutRectangle = new XRect(0, (page.Height - (font.Height + 9)), page.Width, (font.Height - 7));

                //Draw the footer on each page
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    gfx.DrawString(
                        "Page " + i + " of " + OurPdfPage.PageCount,
                        font,
                        brush,
                        layoutRectangle,
                        XStringFormats.Center);
                }
            }
        }
    }

}
