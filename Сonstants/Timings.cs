namespace GoWeb.Сonstants
{
    public enum Timings
    {
        /// <summary>
        /// Время после которого пересоздается событие в часах после его окончания
        /// </summary>
        RecreateTime = 1,

        /// <summary>
        /// За сколько времени до начала события, событие будет отменено при недостаточном количестве участников
        /// </summary>
        CanceledTime = 2,

        /// <summary>
        /// За сколько времени до начала события, можно выписаться из него 
        PossibleDischargeTime = 3
    }
}
