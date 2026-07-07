using Microsoft.AspNetCore.Mvc;
using RagSaaS.Backend.Models;
using RagSaaS.Backend.Services;
using RagSaaS.Backend.Store;
using RagSaaS.Backend.Data;

namespace RagSaaS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly AppDbContext _context;

        public ChatController(GeminiService geminiService, AppDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(DocumentStore.ExtractedText))
                return BadRequest("No document indexed. Please upload a document first.");

            try
            {
                int currentSessionId;

                if (request.SessionId == null || request.SessionId == 0)
                {
                    string title = request.Question.Length > 30
                        ? request.Question.Substring(0, 30) + "..."
                        : request.Question;

                    var newSession = new ChatSession
                    {
                        UserId = request.UserId,
                        Title = $"Query: {title}",
                        ActiveFile = request.ActiveFile
                    };
                    _context.ChatSessions.Add(newSession);
                    await _context.SaveChangesAsync();
                    currentSessionId = newSession.Id;
                }
                else
                {
                    currentSessionId = request.SessionId.Value;
                }

                var userMessage = new ChatMessage
                {
                    SessionId = currentSessionId,
                    Sender = "user",
                    Text = request.Question
                };
                _context.ChatMessages.Add(userMessage);
                await _context.SaveChangesAsync();

                var answer = await _geminiService.GenerateAnswerAsync(request.Question, DocumentStore.ExtractedText);

                var aiMessage = new ChatMessage
                {
                    SessionId = currentSessionId,
                    Sender = "assistant",
                    Text = answer
                };
                _context.ChatMessages.Add(aiMessage);
                await _context.SaveChangesAsync();

                return Ok(new { answer, sessionId = currentSessionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}