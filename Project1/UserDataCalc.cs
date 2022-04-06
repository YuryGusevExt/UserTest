using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public class UserDataCalc
    {
        private readonly UserDataContext ctx;
        private readonly DateTime _today;

        public UserDataCalc(UserDataContext context, DateTime today)
        {
            ctx = context;
            _today = today;
        }

        public CalcResult GetCalcResult()
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

        internal Dictionary<int, int> Histogramm()
        {
            var res = new Dictionary<int, int>();
            var tmp = ctx.Users.GroupBy(g => g.DateDifference).Select(g => new { Diff = g.Key, Count = g.Count() }).OrderBy(x => x.Diff);
            foreach (var v in tmp) { res[v.Diff] = v.Count; }
            return res;
        }

        public double Retention(int days)
        {
            var retdate = _today.AddDays(-days);
            var total = ctx.Users.Count(x => x.RegisterDate <= retdate);
            var retN = ctx.Users.Count(x => x.DateDifference >= days);
            return total == 0 ? 0.0 : Math.Round((double)retN / total * 100, 1);
        }

        public (double, double, double) CalcStat()
        {
            var count = ctx.Users.Count();
            if (count == 0)
                return (0.0, 0.0, 0.0);

            if (count == 1)
            {
                var m = ctx.Users.Sum(x => x.DateDifference);
                return (m, m + 0.001, m + 0.001);
            }

            double med, p10, p90;

            // Если чётные, то берём 2 соседних (пропускаем половину)
            if (count % 2 == 0)
                med = ctx.Users.OrderBy(x => x.DateDifference).Skip(count / 2 - 1).Take(2).Average(x => x.DateDifference);
            else
                med = ctx.Users.OrderBy(x => x.DateDifference).Skip(count / 2).Take(1).Select(x => x.DateDifference).FirstOrDefault();

            var skip0 = count / 10 - 1;
            p10 = ctx.Users.OrderBy(x => x.DateDifference).Skip(skip0 < 0 ? 0 : skip0).Take(2).Select(x => x.DateDifference).Average();
            p90 = ctx.Users.OrderBy(x => x.DateDifference).Skip(count < 2 ? 1 : count * 9 / 10 - 1).Take(2).Select(x => x.DateDifference).Average();

            return (med, p10, p90);
        }
    }
}