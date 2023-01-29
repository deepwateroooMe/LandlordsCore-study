using System.ComponentModel;
namespace ETModel {
    public abstract class Object: ISupportInitialize {

// BeginInit + EndInit
        public virtual void BeginInit() {
        }
        public virtual void EndInit() {
        }

        public override string ToString() {
            return JsonHelper.ToJson(this);
        }
    }
}