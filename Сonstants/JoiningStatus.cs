namespace GoWeb.Сonstants
{
    public enum JoiningStatus
    {
        /// <summary>
        /// Нет статуса
        /// </summary>
        None = 1,
        /// <summary>
        /// Зарегестрирован на событие
        /// </summary>
        Registered = 2,
        /// <summary>
        /// Отменил событие
        /// </summary>
        Cancelled = 3,
        /// <summary>
        /// Посетил событие
        /// </summary>
        Attended = 4,
        /// <summary>
        /// В резерве
        /// </summary>
        InReserve = 5
    }
}
