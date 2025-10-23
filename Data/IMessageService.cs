using System.Collections.Generic;
using OrgnTransplant.Models;
using System.Threading.Tasks;

namespace OrgnTransplant.Data
{
    public interface IMessageService
    {
        Task SendRequestAsync(Message message);
        Task RespondToRequestAsync(int messageId, MessageStatus status, DeliveryOption deliveryOption, string responseText);
        Task<List<Message>> GetReceivedMessagesAsync(string hospitalName);
        Task<List<Message>> GetSentMessagesAsync(string hospitalName);
        Task<int> GetUnreadMessagesCountAsync(string hospitalName);
        Task DeleteMessageAsync(int messageId);
    }
}
