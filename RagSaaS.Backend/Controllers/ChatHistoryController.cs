using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RagSaaS.Backend.Data;

namespace RagSaaS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatHistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("sessions/{userId}")]
        public async Task<IActionResult> GetSessions(int userId)
        {
            var sessions = await _context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new {
                    id = s.Id,
                    title = s.Title,
                    activeFile = s.ActiveFile,
                    date = s.CreatedAt.ToString("MMM dd, yyyy HH:mm")
                })
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("messages/{sessionId}")]
        public async Task<IActionResult> GetMessages(int sessionId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    id = m.Id,
                    sender = m.Sender,
                    text = m.Text,
                    timestamp = m.Timestamp.ToString("hh:mm tt")
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound("Chat session not found.");

        
            var messages = _context.ChatMessages.Where(m => m.SessionId == sessionId);
            _context.ChatMessages.RemoveRange(messages);

            _context.ChatSessions.Remove(session);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Chat deleted successfully." });
        }
    }
}