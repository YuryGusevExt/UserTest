using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Project1
{
    public class UserDataContext : DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

        public DbSet<UserData> Users { get; set; }

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

        internal CalcResult GetCalcResult()
        {
            var dict = Histogramm();
            int count = dict.Count;
            var (med, p10, p90) = CalcStat();
            var res = new CalcResult
            {
                Retention = Retention(7),
                HistY = new int[count],
                HistX = new string[count],
                Median = med,
                Percentile10 = p10,
                Percentile90 = p90
            };
            var i = 0;
            foreach (var v in dict)
            {
                res.HistX[i] = v.Key.ToString();
                res.HistY[i] = v.Value;
                ++i;
            }
            return res;
        }

        private (double, double, double) CalcStat()
        {
            var count = Users.Count();
            if (count == 0)
                return (0.0, 0.0, 0.0);

            if (count == 1)
            {
                var m = Users.Sum(x => x.DateDifference);
                return (m, m + 0.001, m + 0.001);
            }

            double med, p10, p90;

            // Если чётные, то берём 2 соседних (пропускаем половину)
            if (count % 2 == 0)
                med = Users.OrderBy(x => x.DateDifference).Skip(count / 2 - 1).Take(2).Sum(x => x.DateDifference) / 2;
            else
                med = Users.OrderBy(x => x.DateDifference).Skip(count / 2).Take(1).Select(x => x.DateDifference).FirstOrDefault();

            p10 = Users.OrderBy(x => x.DateDifference).Skip(count / 10).Take(1).Select(x => x.DateDifference).FirstOrDefault();
            p90 = Users.OrderBy(x => x.DateDifference).Skip(count *  9 / 10).Take(1).Select(x => x.DateDifference).FirstOrDefault();

            return (med, p10, p90);
        }

        internal double Retention(int days)
        {
            var today = DateTime.Now.Date;
            var retdate = today.AddDays(-days);
            var total = Users.Count(x => x.RegisterDate <= retdate);
            var retN = Users.Count(x => x.DateDifference >= days);
            return total == 0 ? 0.0 : Math.Round((double)retN / total, 3);
        }

        internal Dictionary<int, int> Histogramm()
        {
            var res = new Dictionary<int, int>();
            var tmp = Users.GroupBy(g => g.DateDifference).Select(g => new { Diff = g.Key, Count = g.Count() }).OrderBy(x => x.Diff);
            foreach (var v in tmp) { res[v.Diff] = v.Count; }
            return res;
        }
    }
}