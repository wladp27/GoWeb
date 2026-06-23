namespace GoWeb.Сonstants
{
    public enum JoinResult
    {
        /// <summary>
        /// Операция прошла успешно, пользователь зарегистрирован впервые.
        /// </summary>
        SuccessNewRegistration,

        /// <summary>
        /// Операция прошла успешно, статус пользователя был обновлен (повторная регистрация).
        /// </summary>
        SuccessStatusUpdated,
        /// <summary>
        /// Операция прошла успешно, статус пользователя в резерве.
        /// </summary>
        SuccessInReserve,
        /// <summary>
        /// Регистрация невозможна: пользователь уже имеет активный статус.
        /// </summary>
        AlreadyRegistered,

        /// <summary>
        /// Регистрация невозможна: пользователь с таким ID не найден.
        /// </summary>
        UserNotFound,

        /// <summary>
        /// Регистрация невозможна: мероприятие с таким ID не найдено.
        /// </summary>
        EventNotFound,

        /// <summary>
        /// Регистрация невозможна: мероприятие не доступно.
        /// </summary>
        NoAccessToEvent,
        /// <summary>
        /// Регистрация невозможна: вы записаны на другое мероприятие.
        /// </summary>
        TimeCoincidences,

        /// <summary>
        /// Регистрация невозможна: недостаточно высокий рейтинг.
        /// </summary>
        IsufficientlyRequiredRating



    }

}
