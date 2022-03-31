using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project1
{
    [CustomValidation(typeof(UserData), nameof(XValid))]
    [Index(nameof(DateDifference))]
    public class UserData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }
        [Required]
        public DateTime RegisterDate { get; set; }
        [Required]
        public DateTime LastSeenDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int DateDifference { get; set; }

        /// <summary>
        /// Валидация
        /// </summary>
        /// <remarks>
        /// Сделано не персональными аттрибутами для показа идентификатора пользователя в сообщении
        /// </remarks>
        public static ValidationResult XValid(UserData src) =>
            InvalidDate(src.RegisterDate) ? new ValidationResult($"User {src.UserID} has incorrect register: {src.RegisterDate}") :
            InvalidDate(src.LastSeenDate) ? new ValidationResult($"User {src.UserID} has incorrect last seen date: {src.LastSeenDate}") :
            src.RegisterDate <= src.LastSeenDate ?
            ValidationResult.Success : new ValidationResult($"User {src.UserID} has incorrect register/seen dates: {src.RegisterDate} / {src.LastSeenDate}");

        private static bool InvalidDate(DateTime value) => value < MinValue || value > MaxValue;
        private static readonly DateTime MinValue = new(2020, 01, 01);
        private static readonly DateTime MaxValue = DateTime.Now;
    }
}
