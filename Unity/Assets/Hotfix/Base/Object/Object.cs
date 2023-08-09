namespace ETHotfix {
    public interface IDisposable {
        void Dispose();
    }
    public interface ISupportInitialize { // 这个接口里定义了这两个【初始化】前后的回调方法 
        void BeginInit();
        void EndInit();
    }
    public abstract class Object: ISupportInitialize {
        public virtual void BeginInit() {
        }
        public virtual void EndInit() {
        }
        public override string ToString() {
            return JsonHelper.ToJson(this);
        }
    }
}