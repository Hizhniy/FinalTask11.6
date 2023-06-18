using TestoBot.Models;

namespace TestoBot.Services
{
    public interface IStorage
    {        
        /// Получение сессии пользователя по идентификатору        
        Session GetSession(long chatId);
    }
}