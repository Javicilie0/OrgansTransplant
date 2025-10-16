namespace OrgnTransplant
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
        /// Необходимо е вземане - клиентът трябва да дойде лично
        /// </summary>
        PickupRequired
    }
}
