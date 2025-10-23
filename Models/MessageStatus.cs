namespace OrgnTransplant.Models
{
    /// <summary>
    /// Статус на съобщението/заявката
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// Чака отговор
        /// </summary>
        Pending,

        /// <summary>
        /// Приета заявка
        /// </summary>
        Accepted,

        /// <summary>
        /// Отхвърлена заявка
        /// </summary>
        Rejected
    }
}
