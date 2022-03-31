using System.Collections.Generic;

namespace Project1
{
    public class FetchUserData
    {
        public string Error { get; set; }
        public IEnumerable<UserData> Datas { get; set; }
    }
}
