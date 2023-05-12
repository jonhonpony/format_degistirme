using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Format_degistirme.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult pdf_to_word()
        {
            return View();
        }

        [HttpPost]
        public ActionResult pdf_to_word(HttpPostedFileBase file)
        {
            string pdfFilePath = Server.MapPath("~/App_Data/" + file.FileName);
            string docxFilePath = Server.MapPath("~/App_Data/" + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".docx");

            // Save the uploaded file to the server
            file.SaveAs(pdfFilePath);

            // Extract the text from the PDF file
            string text = string.Empty;
            using (var pdfReader = new PdfReader(pdfFilePath))
            {
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    text += currentText + Environment.NewLine;
                }
            }

            // Convert the text to Word document
            using (var document = DocX.Create(docxFilePath))
            {
                document.InsertParagraph(text);
                document.Save();
            }

            // Read the Word document as byte array
            byte[] byteArray = System.IO.File.ReadAllBytes(docxFilePath);

            // Delete the uploaded files from the server
            System.IO.File.Delete(pdfFilePath);
            System.IO.File.Delete(docxFilePath);

            // Return the converted Word file to the client
            return File(byteArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", System.IO.Path.GetFileName(docxFilePath));
        }

        public ActionResult word_to_pdf()
        {
            return View();
        }

        [HttpPost]
        public ActionResult word_to_pdf(HttpPostedFileBase file)
        {
            string docxFilePath = Server.MapPath("~/App_Data/" + file.FileName);
            string pdfFilePath = Server.MapPath("~/App_Data/" + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".pdf");
            // Save the uploaded file to the server
            file.SaveAs(docxFilePath);
            // Convert the Word document to PDF
            using (var document = DocX.Load(docxFilePath))
            {
                document.SaveAs(pdfFilePath);
            }
            // Read the PDF document as byte array
            byte[] byteArray = System.IO.File.ReadAllBytes(pdfFilePath);
            // Delete the uploaded files from the server
            System.IO.File.Delete(docxFilePath);
            System.IO.File.Delete(pdfFilePath);
            // Return the converted PDF file to the client
            return File(byteArray, "application/pdf", System.IO.Path.GetFileName(pdfFilePath));
            
        }
        public ActionResult pdf_to_png()
        {
            return View();
        }
        [HttpPost]
        public ActionResult pdf_to_png(HttpPostedFileBase file)
        {
            string pdfFilePath = Server.MapPath("~/App_Data/" + file.FileName);
            string pngFilePath = Server.MapPath("~/App_Data/" + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".png");
            // Save the uploaded file to the server
            file.SaveAs(pdfFilePath);
            // Convert PDF to PNG
            using (var document = PdfiumViewer.PdfDocument.Load(pdfFilePath))
            {
                using (var stream = new MemoryStream())
                {
                    var image = document.Render(0, 300, 300, true);
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    // Save the PNG file to the server
                    System.IO.File.WriteAllBytes(pngFilePath, stream.ToArray());
                    // Delete the uploaded PDF file from the server
                    System.IO.File.Delete(pngFilePath);
                
                    
                    // Return the converted PNG file to the client
                    return File(stream.ToArray(), "image/png", System.IO.Path.GetFileName(pngFilePath));

                }
            }
        }


    }
}
