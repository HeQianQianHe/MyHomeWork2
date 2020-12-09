using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class Result
    {
        public Result(int Id, int UserId, int TotalCount, int WinCount)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.TotalCount = TotalCount;
            this.WinCount = WinCount;
        }

        public int Id { set; get; }
        public int UserId { set; get; }
        public int TotalCount { set; get; }
        public int WinCount { set; get; }
    }
}
