using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class User
    {
        public User(int i, string name, string pwd)
        {
            this.Id = i;
            this.Username = name;
            this.Password = pwd;
        }

        public int Id { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }

    }
}
