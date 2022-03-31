using System;
using System.Linq;

namespace Project1
{
    public static class DbInitializer
    {
        public static void Initialize(UserDataContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            const int userCount = 20;

            var datas = new UserData[userCount];
            var rand = new Random();
            for (var i = 0; i < userCount; ++i)
            {
                var d1 = rand.Next(0, 30);
                var d2 = rand.Next(0, 30);
                datas[i] = new UserData { UserID = i + 1, RegisterDate = DateTime.Now.Date.AddDays(-d1 - d2), LastSeenDate = DateTime.Now.Date.AddDays(-d1) };
            };
            context.Users.AddRange(datas);
            context.SaveChanges();
        }
    }
}