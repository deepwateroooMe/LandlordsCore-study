﻿ using System.Net;
namespace ETModel {

    // Session关联User对象组件
    // 用于Session断开时触发下线: 
    public class SessionUserComponent : Component {
        // User对象
        public User User { get; set; }
    }
}