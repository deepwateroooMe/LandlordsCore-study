﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace ETModel {

    public struct Timer {
        public long Id { get; set; }
        public long Time { get; set; }
        public TaskCompletionSource<bool> tcs;
    }

    [ObjectSystem]
    public class TimerComponentUpdateSystem : UpdateSystem<TimerComponent> {
        public override void Update(TimerComponent self) {
            self.Update();
        }
    }

    public class TimerComponent : Component {
        private readonly Dictionary<long, Timer> timers = new Dictionary<long, Timer>();
        // key: time, value: timer id
        private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();
        private readonly Queue<long> timeOutTime = new Queue<long>();
        
        private readonly Queue<long> timeOutTimerIds = new Queue<long>();
        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTime;
        public void Update() {
            if (this.timeId.Count == 0) {
                return;
            }
            long timeNow = TimeHelper.Now();
            if (timeNow < this.minTime) {
                return;
            }
            
            foreach (KeyValuePair<long, List<long>> kv in this.timeId.GetDictionary()) {
                long k = kv.Key;
                if (k > timeNow) {
                    minTime = k;
                    break;
                }
                this.timeOutTime.Enqueue(k);
            }
            while(this.timeOutTime.Count > 0) {
                long time = this.timeOutTime.Dequeue();
                foreach(long timerId in this.timeId[time]) {
                    this.timeOutTimerIds.Enqueue(timerId);    
                }
                this.timeId.Remove(time);
            }
            while(this.timeOutTimerIds.Count > 0) {
                long timerId = this.timeOutTimerIds.Dequeue();
                Timer timer;
                if (!this.timers.TryGetValue(timerId, out timer)) {
                    continue;
                }
                this.timers.Remove(timerId);
                timer.tcs.SetResult(true);
            }
        }
        private void Remove(long id) {
            Timer timer;
            if (!this.timers.TryGetValue(id, out timer)) {
                return;
            }
            this.timers.Remove(id);
        }
        public Task WaitTillAsync(long tillTime, CancellationToken cancellationToken) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime) {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }
        public Task WaitTillAsync(long tillTime) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime) {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }
        public Task WaitAsync(long time, CancellationToken cancellationToken) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime) {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }
        public Task WaitAsync(long time) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime) {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }
    }
}