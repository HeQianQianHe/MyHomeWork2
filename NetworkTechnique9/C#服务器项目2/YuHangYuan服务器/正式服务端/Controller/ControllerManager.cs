using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;
using GameServer.Servers;

namespace GameServer.Controller
{
    class ControllerManager
    {
        //这个字典用来装不同RequestCode所对应的不同Controller，用特定的Controller类来处理特定的请求
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;

        public ControllerManager(Server server)//构造方法
        {
            this.server = server;
            InitController();
        }

        void InitController()//把所有的类型的Controller实例化出来装进字典
        {
            DefaultController defcontroller=new DefaultController();
            controllerDict.Add(defcontroller.RequestCode, defcontroller);
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());
        }
                                                                                                                                         
        public void HandleRequest(RequestCode rc, ActionCode ac, string data, Client client)//这个方法通过传入的RequestCode和ActionCode来找到对应的Controller里面对应的方法然后调用,data是要处理的数据，后面两个参数是持有一下Client和Server的引用，以便于比如广播消息给其他客户端之类的事件的响应
        {                                                                                   //其中Server的引用在构造方法中传递                                 
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(rc, out controller);//通过RequestCode来在字典中找到对应的controller
            if (isGet == false)
            {
                Console.WriteLine("无法得到【" + rc + "】所对应的controller");
                return;
            }
            string methodname = Enum.GetName(typeof(ActionCode), ac);//这个方法用来把枚举类型转化成字符串，因为我们的ActionCode这个枚举其实就是用对应方法名来写的，所以一转化就是方法名
            MethodInfo mi = controller.GetType().GetMethod(methodname);//通过反射找到controller中的方法，返回一个MethodInfo类对象，通过这个对象可以调用刚刚寻找的方法
            if (mi == null)
            {
                Console.WriteLine("在controller【" + controller.GetType() + "】中没有对应的方法【" + methodname + "】");
                return;
            }
            object[] parameters=new object[]{data,client,server};
            object o = mi.Invoke(controller, parameters);//通过Invoke调用mi刚刚找到的方法，第一个参数是需要指定调用此方法的实例对象，第二个参数是object类型的数组，里面用来装该mi指向方法所需要的参数，返回一个object来确定是否对客户端做出一些回复
            if (o == null || string.IsNullOrEmpty(o as string))
            {
                return;
            }
            server.SendResponse(client, ac, o as string);//将发送请求的客户端和RequestCode以及服务端处理完data后的返回消息,这三个参数交给server中的方法来处理
        }

    }


}
