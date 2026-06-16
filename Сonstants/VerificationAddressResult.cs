namespace GoWeb.Сonstants
{
    public enum VerificationAddressResult
    {
        /// <summary>
        /// Ошибка, несуществующий город.
        /// </summary>
        NonExistentCity,

        /// <summary>
        /// Ошибка,локация находится за пределами города.
        /// </summary>
        LocationOutsideCity,
        /// <summary>
        ///  Ошибка,локация уже есть в БД.
        /// </summary>
        NonExistentLocation,
        /// <summary>
        /// Успех, такого адреса нет в БД
        /// </summary>
        Ok

    }

}
