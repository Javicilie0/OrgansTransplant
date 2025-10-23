namespace OrgnTransplant.Models
{
    /// <summary>
    /// Опции за доставка на органа
    /// </summary>
    public enum DeliveryOption
    {
        /// <summary>
        /// Неуточнено
        /// </summary>
        NotSpecified,

        /// <summary>
        /// С шофьор - болницата предоставя транспорт
        /// </summary>
        WithDriver,

        /// <summary>
        /// С хеликоптер - спешна въздушна доставка
        /// </summary>
        WithHelicopter,

        /// <summary>
        /// Необходимо е вземане - клиентът трябва да дойде лично
        /// </summary>
        PickupRequired
    }
}
