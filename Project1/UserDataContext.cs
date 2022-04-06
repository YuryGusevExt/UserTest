using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Project1
{
    public class UserDataContext :  DbContext
    {
        virtual public DbSet<UserData> Users { get; set; }

        public UserDataContext() : base() { }

        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>().Property(p => p.DateDifference).HasComputedColumnSql("DATEDIFF(day,RegisterDate,LastSeenDate)");
            modelBuilder.Entity<UserData>().ToTable("Users");
        }

        /// <summary>
        /// Сохранение списка пользователей
        /// </summary>
        /// <param name="data">Новый список</param>
        /// <returns>Текст ошибки или null</returns>
        internal string SaveUsers(IEnumerable<UserData> data)
        {
            var toDel = new HashSet<int>();
            foreach (var u in Users.AsNoTracking())
                toDel.Add(u.UserID);
            foreach (var d in data)
            {
                if (toDel.Remove(d.UserID))
                    Users.Update(d);
                else
                    Users.Add(d);
            }
            var dels = Users.Where(x => toDel.Contains(x.UserID));
            Users.RemoveRange(dels);
            SaveChanges();
            return null;
        }
    }
}