using Microsoft.AspNetCore.Mvc;
using MIRS.Web.Models;
using System.Diagnostics;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.XObjects;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting.Server;

namespace MIRS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            PdfFileContentText pdfFileContentText = new PdfFileContentText();
            string filePathTaxSearch = Directory.GetCurrentDirectory() + "\\Templates\\Tax Search.pdf";

            using (var document = PdfDocument.Open(filePathTaxSearch))
            {
                foreach (var page in document.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page, true);

                    pdfFileContentText.pdfFileContentTaxSearch = string.Format("{0}\n{1}", pdfFileContentText.pdfFileContentTaxSearch, text);
                }
            }

            string filePathTitleSearch = Directory.GetCurrentDirectory() + "\\Templates\\Title Search.pdf";
            using (var document = PdfDocument.Open(filePathTitleSearch))
            {
                foreach (var page in document.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page, true);

                    pdfFileContentText.pdfFileContentTitleSearch = string.Format("{0}\n{1}", pdfFileContentText.pdfFileContentTitleSearch, text);
                }
            }

            using (var document = PdfDocument.Open(filePathTaxSearch))
            {
                foreach (var page in document.GetPages())
                {
                    foreach (var image in page.GetImages())
                    {
                        if (!image.TryGetBytes(out var b))
                        {
                            b = image.RawBytes;
                        }

                        var type = string.Empty;
                        switch (image)
                        {
                            case XObjectImage ximg:
                                type = "XObject";
                                break;
                            case InlineImage inline:
                                type = "Inline";
                                break;
                        }

                        pdfFileContentText.pdfFileImageTaxSearch = string.Format("{0}\n{1}", pdfFileContentText.pdfFileImageTaxSearch, $"Image with {b.Count} bytes of type '{type}' on page {page.Number}. Location: {image.Bounds}.");
                    }
                }
            }

            using (var document = PdfDocument.Open(filePathTitleSearch))
            {
                foreach (var page in document.GetPages())
                {
                    foreach (var image in page.GetImages())
                    {
                        if (!image.TryGetBytes(out var b))
                        {
                            b = image.RawBytes;
                        }

                        var type = string.Empty;
                        switch (image)
                        {
                            case XObjectImage ximg:
                                type = "XObject";
                                break;
                            case InlineImage inline:
                                type = "Inline";
                                break;
                        }

                        pdfFileContentText.pdfFileImageTitleSearch = string.Format("{0}\n{1}", pdfFileContentText.pdfFileImageTitleSearch, $"Image with {b.Count} bytes of type '{type}' on page {page.Number}. Location: {image.Bounds}.");
                    }
                }
            }

            return View(pdfFileContentText);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}