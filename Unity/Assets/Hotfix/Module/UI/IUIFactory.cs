using UnityEngine;
namespace ETHotfix {

    public interface IUIFactory { // 工厂类型的接口：两个能力，生产，与 清理门户
        
        UI Create(Scene scene, string type, GameObject parent);
        void Remove(string type);
    }
}