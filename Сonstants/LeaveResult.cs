namespace GoWeb.Сonstants
{
    public enum LeaveResult
    {
        ///<summary>
        /// Операция выписки прошла успешно
        /// </summary>
        SuccessLeave,

        /// <summary>
        /// Выписка невозможна: пользователь уже выписан.
        /// </summary>
        AlreadyLeave,

        /// <summary>
        /// Выписка невозможна: пользователь с таким ID не найден.
        /// </summary>
        UserNotFound,

        /// <summary>
        /// Выписка невозможна: мероприятие с таким ID не найдено.
        /// </summary>
        EventNotFound,

        /// <summary>
        /// Выписка невозможна: пользователь не был зарегистрирован на событие.
        /// </summary>
        UserIsNotRegistered,

        /// <summary>
        /// Выписка невозможна: событие окончено.
        /// </summary>
        EventIsOver,

        /// <summary>
        /// Выписка невозможна: события скоро начнется.
        /// </summary>
        EvenWillStartSoon
    }
}
