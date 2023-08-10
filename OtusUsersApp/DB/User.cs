namespace OtusUsersApp.DB
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентифкатор
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// почта
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Полное имся
        /// </summary>
        public string FullName { get; set; }
    }
}
