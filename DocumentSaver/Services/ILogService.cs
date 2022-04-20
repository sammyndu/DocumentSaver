using DocumentSaver.Data.Entities;

namespace DocumentSaver.Services
{
    public interface ILogService
    {
        Task AddLog(String username, String logAction);
        Task<List<Log>> GetLogs();
    }
}