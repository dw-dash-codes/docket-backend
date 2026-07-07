using Microsoft.AspNetCore.Mvc;
using RagSaaS.Backend.Store;
using UglyToad.PdfPig;

namespace RagSaaS.Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No, file uploaded");

            if (file.ContentType != "application/pdf")
                return BadRequest("File of this type is not allowed");

            try
            {
                string extractedText = "";

                using (var stream = file.OpenReadStream())
                using (var pdfDocument = PdfDocument.Open(stream))
                {
                    foreach (var page in pdfDocument.GetPages())
                    {
                        extractedText += page.Text + "";
                    }
                }

                DocumentStore.ExtractedText = extractedText;

                return Ok(new
                {
                    message = "File uploading process done ",
                    totalCharacters = extractedText.Length,
                    previewText = extractedText.Substring(0, Math.Min(extractedText.Length, 150)) + "..."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Issue {ex.Message}");
                
            }
        }
    }
}
