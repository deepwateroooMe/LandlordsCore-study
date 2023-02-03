using System.Threading.Tasks;
namespace ETModel {

 // 抽象蕨类说：所有想要操作远程MongoDB数据库的异步任务包装，都得实现这个方法    
    public abstract class DBTask : ComponentWithId {

        public abstract Task Run();
    }
}