using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BiblioCrit.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Tesseract;
using System.IO;

namespace BiblioCrit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public const string trainedDataFolderName = "tessdata";

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new OcrModel();
            return View(model);
        }


        [HttpPost]
        [ActionName("GetImageText")]
        public IActionResult Index(OcrModel ocrModel)
        {
            //string contentRootPath = _webHostEnvironment.ContentRootPath;
            //string tessPath = Path.Combine(contentRootPath, trainedDataFolderName);
            string filePath = ProcessImage(ocrModel.CroppedImage);
            string webRootPath = _webHostEnvironment.WebRootPath;
            var uploads = Path.Combine(webRootPath, "images");

            string tessPath = Path.Combine(trainedDataFolderName, "");
            Console.WriteLine(tessPath);
            string result = "";

            using (var engine = new TesseractEngine(tessPath, ocrModel.DestinationLanguage, EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(filePath))
                {
                    var page = engine.Process(img);
                    result = page.GetText();
                    Console.WriteLine(result);
                }
            }

            string imageText = String.IsNullOrWhiteSpace(result) ? "Unreadable image text" : result;
            return RedirectToAction(nameof(GetReviews), new { imageText });
        }

        public IActionResult GetReviews(string imageText)
        {
            ViewBag.GoodReadsAPIKey = _configuration.GetValue<string>("GoodReadsAPIKey");
            ViewBag.imageText = imageText;
            return View();
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

        private string ProcessImage(string croppedImage)
        {
            string filePath = String.Empty;
            string webRootPath = _webHostEnvironment.WebRootPath;
            try
            {
                string base64 = croppedImage;
                byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                var uploads = Path.Combine(webRootPath, "images");
                var uploadsFolder = Path.Combine(webRootPath, "images");
                filePath = Path.Combine(uploads, "book_cover_" + Guid.NewGuid() + ".png");
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }

            return filePath;
        }
    }
}
