using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller//这里要把创建的类库项目“Common”添加引用过来，右键“引用”-“添加引用”-“解决方案”-“项目”里面即可找到
{
    abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.None;
        public RequestCode RequestCode
        {
            get
            {
                return requestCode;
            }
        }
        public virtual string DefaultHandle(string data,Client client,Server server) { return null; }

    }
}
