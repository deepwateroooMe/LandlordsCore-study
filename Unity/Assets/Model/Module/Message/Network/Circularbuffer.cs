using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ETModel {
     // 圆形Buffer: 有什么好处呢？为什么会如此设计？
    public class CircularBuffer: Stream {

        public int ChunkSize = 8192; // 定义了一个常量，决定了每个缓存区块的大小为8KB。为什么是8KB呢？可能是8KB的数据传输比较快吧。

        // 我们一般只会取出缓冲区的首位，和往末位插入新的数据，应对这样的需求，Queue就已经符合要求了
        private readonly Queue<byte[]> bufferQueue = new Queue<byte[]>(); // 有效的,真正实际的 操作队列
        private readonly Queue<byte[]> bufferCache = new Queue<byte[]>(); // 后备军,缓存

        // 我们从Queue当中获取首位数据很方便，但是没法获取末位的数据，所以这里特意分配一块内存给末位的数据区块，方便操作。
        // 这里应该是标记同一块,最后一块缓冲区8192内部的起始与终止下标位置.这两个是写入写出时一个缓冲区对应字节的游标(动态变化的)
        public int LastIndex { get; set; }
        public int FirstIndex { get; set; }
        
        private byte[] lastBuffer;

        public CircularBuffer() {
            this.AddLast();
        }
        public override long Length {
            get {
                int c = 0;
                if (this.bufferQueue.Count == 0) {
                    c = 0;
                } else {
                    c = (this.bufferQueue.Count - 1) * ChunkSize + this.LastIndex - this.FirstIndex; // 最后一部分: 最后一个缓冲区写入了的字节的数量
                }
                if (c < 0) {
                    Log.Error("CircularBuffer count < 0: {0}, {1}, {2}".Fmt(this.bufferQueue.Count, this.LastIndex, this.FirstIndex));
                }
                return c;
            }
        }
        
        // 我们发现，如果我们要向bufferQueue中添加新的数据区块的时候，会先判断bufferCache中有没有可用的数据区块，如果有的话，则从bufferCache当中取出一个数据区块进队bufferQueue。
        // 其实这是一个类似对象池的设计，为了优化内存的调用，避免多次new一片新的内存。而bufferCache中的数据区块，应该就是从bufferQueue当中出队并已经没有用的数据区块了。
    public void AddLast() { // 实例化的时候，自动分配一个（或拿内存池中现有的，或分配一个，就是  确保  一定有一个可用的）
            byte[] buffer;
            if (this.bufferCache.Count > 0) {
                buffer = this.bufferCache.Dequeue();
            } else {
                buffer = new byte[ChunkSize];
            }
            this.bufferQueue.Enqueue(buffer);
            this.lastBuffer = buffer;
        }
        // 对数据区块的回收了，不用当心我们没有对进队的数据区块进行“清空”，因为我们以LastIndex自己控制游标位置，这些旧的数据会被新的数据所覆盖的
        public void RemoveFirst() {
            this.bufferCache.Enqueue(bufferQueue.Dequeue()); // 没有清空现有数据,但当需要拿来再用,就可以直接覆盖掉原数据,回收利用,减少不必要的操作
        }
 // 第一个，与  最后一个： bufferQueue中的字节数组        
        public byte[] First {
            get {
                if (this.bufferQueue.Count == 0) {
                    this.AddLast();
                }
                return this.bufferQueue.Peek();
            }
        }
        public byte[] Last {
            get {
                if (this.bufferQueue.Count == 0) {
                    this.AddLast();
                }
                return this.lastBuffer;
            }
        }
// 从CircularBuffer读到stream中: 异步读,以缓冲区8K大小为单位来一块一块地读,直到读完为止
        public async Task ReadAsync(Stream stream) {
            long buffLength = this.Length;
            int sendSize = this.ChunkSize - this.FirstIndex;
            if (sendSize > buffLength) {
                sendSize = (int)buffLength; // 这里其实就是说,以常量大小8K为块(或是说,以常量缓冲区大小为单位<=来)来异步读取
            }
            
            await stream.WriteAsync(this.First, this.FirstIndex, sendSize);
            
            this.FirstIndex += sendSize;
            if (this.FirstIndex == this.ChunkSize) {
                this.FirstIndex = 0;
                this.RemoveFirst();
            }
        }
// 从CircularBuffer读到stream:
        public void Read(Stream stream, int count) {
            if (count > this.Length) {
                throw new Exception($"bufferList length < count, {Length} {count}");
            }
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count) { // 还没有读完的情况下,循环
                int n = count - alreadyCopyCount; // 还剩余多少个字节
                if (ChunkSize - this.FirstIndex > n) { // 如果当前缓冲区足够大
                    stream.Write(this.First, this.FirstIndex, n); // 直接从FirstIndex写入所需要的大小n
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                } else { // 当前缓冲区所剩余的可用字节量 不足够大
                    stream.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex); // 先把当前缓存区写满
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0; // 更新起始下标为0,因为会从新缓冲区第一位置开始写
                    this.RemoveFirst(); // 移除第一块缓冲块
                }
            }
        }
// 从stream写入CircularBuffer
        public void Write(Stream stream) {
            int count = (int)(stream.Length - stream.Position);
            
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count) {
                if (this.LastIndex == ChunkSize) { // 如果当前缓冲区写满了,就拿块新的来用
                    this.AddLast();
                    this.LastIndex = 0;
                }
                int n = count - alreadyCopyCount; // 剩余需要写的字节量
                if (ChunkSize - this.LastIndex > n) { // 当前缓冲区足够大
                    stream.Read(this.lastBuffer, this.LastIndex, n); // 直接写到最后加入的字节数组里去,就是从当前缓冲区的可用位置开始写起,写完所需要字节量大小
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                } else {
                    stream.Read(this.lastBuffer, this.LastIndex, ChunkSize - this.LastIndex); // 当前缓冲区不足够大,先把当前缓存区写满级(,再分配一个新的缓冲区来继续写,剩下的就继续循环)
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }
// 从stream写入CircularBuffer: 返回的是异步写,当前缓冲区操作实际写入写成功了的大小,<= 8192 bytes
        public async Task<int> WriteAsync(Stream stream) {
            int size = this.ChunkSize - this.LastIndex;
            // Stream.ReadAsync ： 从当前流异步读取字节序列，并将流中的位置提升读取的字节数。            
            int n = await stream.ReadAsync(this.Last, this.LastIndex, size);
            if (n == 0) {
                return 0;
            }
            this.LastIndex += n; // 更新这次实际读到的大小与可读下标的位置
            if (this.LastIndex == this.ChunkSize) {
                this.AddLast();
                this.LastIndex = 0;
            }
            return n;
        }
        // 把CircularBuffer中数据写入buffer
        public override int Read(byte[] buffer, int offset, int count) {
            if (buffer.Length < offset + count) {
                throw new Exception($"bufferList length < coutn, buffer length: {buffer.Length} {offset} {count}");
            }
            long length = this.Length;
            if (length < count) {
                count = (int)length;
            }
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count) {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n) {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                } else {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
            return count;
        }
        // 把buffer写入CircularBuffer中
        public override void Write(byte[] buffer, int offset, int count) {
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count) {
                if (this.LastIndex == ChunkSize) {
                    this.AddLast();
                    this.LastIndex = 0;
                }
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n) {
                    Array.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, n);
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                } else {
                    Array.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, ChunkSize - this.LastIndex);
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }
        public override void Flush() {
            throw new NotImplementedException();
        }
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotImplementedException();
        }
        public override void SetLength(long value) {
            throw new NotImplementedException();
        }
        public override bool CanRead {
            get {
                return true;
            }
        }
        public override bool CanSeek {
            get {
                return false;
            }
        }
        public override bool CanWrite {
            get {
                return true;
            }
        }
        public override long Position { get; set; }
    }
}