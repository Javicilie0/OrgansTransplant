using System;

namespace OrgnTransplant.Models
{
    /// <summary>
    /// Модел за съобщение между болници
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Уникален идентификатор на съобщението
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Болница изпращач
        /// </summary>
        public string FromHospital { get; set; }

        /// <summary>
        /// Болница получател
        /// </summary>
        public string ToHospital { get; set; }

        /// <summary>
        /// Име на органа
        /// </summary>
        public string OrganName { get; set; }

        /// <summary>
        /// Име на донора (опционално)
        /// </summary>
        public string DonorName { get; set; }

        /// <summary>
        /// ID на донора (опционално)
        /// </summary>
        public int? DonorId { get; set; }

        /// <summary>
        /// Тип на съобщението (Request/Response)
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Опция за доставка
        /// </summary>
        public DeliveryOption DeliveryOption { get; set; }

        /// <summary>
        /// Статус на заявката (Pending/Accepted/Rejected)
        /// </summary>
        public MessageStatus Status { get; set; }

        /// <summary>
        /// Текст на съобщението/заявката
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Дата и час на създаване
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата и час на отговор (null ако няма отговор)
        /// </summary>
        public DateTime? RespondedAt { get; set; }

        /// <summary>
        /// Текст на отговора (null ако няма отговор)
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Дали съобщението е прочетено от получателя
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Конструктор по подразбиране
        /// </summary>
        public Message()
        {
            MessageType = MessageType.Request;
            DeliveryOption = DeliveryOption.NotSpecified;
            Status = MessageStatus.Pending;
            CreatedAt = DateTime.Now;
            IsRead = false;
        }
    }
}
