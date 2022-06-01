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

        static List<ProductDetail> BuildModel_Main()
        {
            List<ProductDetail> products = new List<ProductDetail>();
            ProductDetail product = new ProductDetail();
            product.Quantity = 10;
            product.ItemCode = "Sugar50";
            product.Type = "food";
            product.ISBN = 123124455;

            for (int i = 0; i < 50; i++)
            {
                product.Quantity = i + 1;
                products.Add(product);
            }
            //products.Add(product8);

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
            List<ProductDetail> products = BuildModel_Main();
            SetFooter(products);
            GeneratePDF();

        }

        private static void CreateProductsTable()
        {
            StringBuilder productTableHtml = new StringBuilder();
            List<ProductDetail> products = BuildModel_Main();
            int numOfProducts = products.Count;
            for (int i = 0; i < numOfProducts; i++)
            {
                string orderDetailHtmlText;
                if (i == 9 || (i - 9) % 19 == 0)
                {
                    orderDetailHtmlText = "</table><br style=\"margin-top:50px\"><table class=\"width-95 product-table margin-table\"><tr><td class=\"add-border padding-for-cell\"><h4 class=\"m-0 p-0\">{{Quantity}}</h4 ></td><td class= \"add-border padding-for-cell\" ><h4 class= \"m-0 p-0\" >{{ItemCode}}</h4 ></td><td class= \"add-border padding-for-cell\" > <h4 class= \"m-0 p-0\" >{{Type}}</h4 ><p class= \"m-0 p-0\" > ISBN:{{ISBN}}</p ></td></tr > ";
                }
                else
                {
                    orderDetailHtmlText = "<tr><td class=\"add-border padding-for-cell\"><h4 class=\"m-0 p-0\">{{Quantity}}</h4 ></td><td class= \"add-border padding-for-cell\" ><h4 class= \"m-0 p-0\" >{{ItemCode}}</h4 ></td><td class= \"add-border padding-for-cell\" > <h4 class= \"m-0 p-0\" >{{Type}}</h4 ><p class= \"m-0 p-0\" > ISBN:{{ISBN}}</p ></td></tr > ";
                }
                orderDetailHtmlText = orderDetailHtmlText.Replace("{{Quantity}}", products[i].Quantity.ToString());
                orderDetailHtmlText = orderDetailHtmlText.Replace("{{ItemCode}}", products[i].ItemCode);
                orderDetailHtmlText = orderDetailHtmlText.Replace("{{Type}}", products[i].Type);
                orderDetailHtmlText = orderDetailHtmlText.Replace("{{ISBN}}", products[i].ISBN.ToString());

                productTableHtml.Append(orderDetailHtmlText);
            }
            htmlText.Replace("{{productDetailsContent}}", productTableHtml.ToString());
        }

        private static void SetFooter(List<ProductDetail> products)
        {
            int n = products.Count;
            string footerHtml = "<div class=\"margin-table footer add-border w-95\" {{StyleForFooter}} ><p>Comments: <span class=\"font-weight-bold\">Customers has a dock and recieve truck shipments</span></p> </div>";
            if (n == 6 || (n - 9) % 15 == 0)
            {
                footerHtml = footerHtml.Replace("{{StyleForFooter}}", "style=\"margin-top: 90px\"");
            }
            else
            {
                footerHtml = "<div class=\"margin-table footer add-border w-95\"><p>Comments: <span class=\"font-weight-bold\">Customers has a dock and recieve truck shipments</span></p> </div>";
            }
            htmlText = htmlText.Replace("{{footer}}", footerHtml);
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
    }

}
