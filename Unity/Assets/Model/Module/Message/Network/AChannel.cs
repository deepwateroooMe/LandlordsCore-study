﻿using System;
using System.IO;
using System.Net;
namespace ETModel {

 // 类型只有两种 方向:  或发送,或接收
    public enum ChannelType {
        Connect,
        Accept,
    }

    public abstract class AChannel: ComponentWithId {

        public ChannelType ChannelType { get; }

        private AService service;
        public AService Service {
            get {
                return this.service;
            }
        }
        public abstract MemoryStream Stream { get; }
        
        public int Error { get; set; }
        public IPEndPoint RemoteAddress { get; protected set; }

        private Action<AChannel, int> errorCallback;
        public event Action<AChannel, int> ErrorCallback {
            add {
                this.errorCallback += value;
            }
            remove {
                this.errorCallback -= value;
            }
        }
        
        private Action<MemoryStream> readCallback; // 内存流的可读回调，（有可读的内存流就会自动触发回调）
        public event Action<MemoryStream> ReadCallback {
            add {
                this.readCallback += value;
            }
            remove {
                this.readCallback -= value;
            }
        }
        
        protected void OnRead(MemoryStream memoryStream) {
            this.readCallback.Invoke(memoryStream);
        }
        protected void OnError(int e) { // 出错回调，大数情况下可能是把异步抛出来
            this.Error = e;
            this.errorCallback?.Invoke(this, e);
        }

        protected AChannel(AService service, ChannelType channelType) {
            this.Id = IdGenerater.GenerateId();
            this.ChannelType = channelType;
            this.service = service;
        }

        public abstract void Start();
        
        public abstract void Send(MemoryStream stream);
        
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.service.Remove(this.Id);
        }
    }
}