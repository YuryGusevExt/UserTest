using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Project1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly ILogger<UserDataController> _logger;
        private readonly UserDataContext ctx;

        public UserDataController(ILogger<UserDataController> logger, UserDataContext context)
        {
            _logger = logger;
            ctx = context;
        }

        [HttpGet]
        public FetchUserData Get()
        {
            try
            {
                return new FetchUserData { Datas = ctx.Users.ToList(), Error = null };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new FetchUserData { Datas = new List<UserData>(), Error = ex.Message };
            }
        }

        [HttpPost]
        public FetchUserData Post(IEnumerable<UserData> data)
        {
            try
            {
                foreach (var d in data)
                {
                    d.RegisterDate = d.RegisterDate.GetLocalDate();
                    d.LastSeenDate = d.LastSeenDate.GetLocalDate();
                }
                var error = ctx.SaveUsers(data);
                return new FetchUserData { Datas = new List<UserData>(), Error = error };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new FetchUserData { Datas = new List<UserData>(), Error = ex.Message };
            }
        }

        [HttpGet("calc")]
        public CalcResult GetRes()
        {
            try
            {
                var tmp = new UserDataCalc(ctx, DateTime.Now.Date);
                return tmp.GetCalcResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new CalcResult { HistX = Array.Empty<string>(), HistY = Array.Empty<int>(), Error = ex.Message };
            }
        }
    }
}