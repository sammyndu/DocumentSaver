using DocumentSaver.Data;
using DocumentSaver.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentSaver.Services
{
    public class LogService : ILogService
    {
        private AppDbContext _context;

        public LogService(
            AppDbContext context
            )
        {
            _context = context;
        }

        public async Task<List<Log>> GetLogs()
        {
            var logs = await _context.Logs.ToListAsync();

            return logs;
        }

        public async Task AddLog(String username, String logAction)
        {
            var log = new Log
            {
                Username = username,
                Action = logAction,
                DateCreated = DateTime.Now
            };

            _context.Logs.Add(log);

            await _context.SaveChangesAsync();
        }
    }
}
