using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Project1;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void RetentionTest()
        {
            var today = new DateTime(2020, 1, 1);
            var tmp = new List<UserData>();
            var diffs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            GetDatas(today, tmp, diffs);
            var users = tmp.AsQueryable();
            var mockUsers = new Mock<DbSet<UserData>>();
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mock = new Mock<UserDataContext>();
            mock.Setup(x => x.Users).Returns(mockUsers.Object);
            var calc = new UserDataCalc(mock.Object, today);
            // 100% = 4 / 4
            Assert.Equal(100.0, calc.Retention(7));
            GetDatas(today.AddDays(-3), tmp, diffs);
            // 57.1% = 4 / 7
            Assert.Equal(Math.Round(100.0 * 4.0 / 7.0, 1), calc.Retention(7));
        }

        [Fact]
        public void CalcTest()
        {
            var today = new DateTime(2020, 1, 1);
            var tmp = new List<UserData>();
            var diffs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            GetDatas(today, tmp, diffs);
            var users = tmp.AsQueryable();
            var mockUsers = new Mock<DbSet<UserData>>();
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsers.As<IQueryable<UserData>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mock = new Mock<UserDataContext>();
            mock.Setup(x => x.Users).Returns(mockUsers.Object);
            var calc = new UserDataCalc(mock.Object, today);

            var res = calc.CalcStat();

            Assert.Equal(5.5, res.Item1);
            Assert.Equal(1.5, res.Item2);
            Assert.Equal(9.5, res.Item3);

            diffs = new int[] { 1, 2, 3 };
            GetDatas(today, tmp, diffs);
            res = calc.CalcStat();

            Assert.Equal(2.0, res.Item1);
            Assert.Equal(1.5, res.Item2);
            Assert.Equal(2.5, res.Item3);
        }

        private static void GetDatas(DateTime date, List<UserData> tmp, int[] diffs)
        {
            var n = 1;
            tmp.Clear();
            foreach (var i in diffs)
            {
                tmp.Add(new UserData { UserID = n++, DateDifference = i, LastSeenDate = date, RegisterDate = date.AddDays(-i) });
            }
        }
    }
}
