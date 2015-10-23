using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Container
{
    public class SharedDataWriter<T>
    {
        private Thread writerThread;
        private MemoryMappedFilesEditor<T> writer;
        private ConcurrentQueue<T> dataToSend;

        public SharedDataWriter(string writingFileName)
        {
            this.writerThread = new Thread(startWritingData);
            this.writer = new MemoryMappedFilesEditor<T>(writingFileName);
            this.dataToSend = new ConcurrentQueue<T>();

            this.init();
        }

        protected void init()
        {
            this.writerThread.Start();
        }

        public void writeData(T data)
        {
            this.dataToSend.Enqueue(data);
        }

        private void startWritingData()
        {
            while (true)
            {
                try
                {
                    T obj;
                    if (!this.dataToSend.TryDequeue(out obj) || obj == null)
                    {
                        Thread.Sleep(2000); // The data is not ready yet. So, try after some time.
                        continue;
                    }

                    Debug.WriteLine("[ WRITE ]: " + obj);
                    writer.write(obj);
                }
                catch
                {
                    Thread.Sleep(2000); // The reader got error. So, try after some time.
                    continue;
                }
            }
        }
    }
}
