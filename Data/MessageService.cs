using System.Collections.Generic;
using OrgnTransplant.Models;
using System.Threading.Tasks;
using OrgnTransplant.Data;

namespace OrgnTransplant.Data
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SendRequestAsync(Message message)
        {
            await _messageRepository.SendRequestAsync(message);
        }

        public async Task RespondToRequestAsync(int messageId, MessageStatus status, DeliveryOption deliveryOption, string responseText)
        {
            await _messageRepository.RespondToRequestAsync(messageId, status, deliveryOption, responseText);
        }

        public async Task<List<Message>> GetReceivedMessagesAsync(string hospitalName)
        {
            return await _messageRepository.GetReceivedMessagesAsync(hospitalName);
        }

        public async Task<List<Message>> GetSentMessagesAsync(string hospitalName)
        {
            return await _messageRepository.GetSentMessagesAsync(hospitalName);
        }

        public async Task<int> GetUnreadMessagesCountAsync(string hospitalName)
        {
            return await _messageRepository.GetUnreadMessagesCountAsync(hospitalName);
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            await _messageRepository.DeleteMessageAsync(messageId);
        }
    }
}
