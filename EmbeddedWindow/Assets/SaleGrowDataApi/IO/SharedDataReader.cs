using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace FileMap
{
    public class SharedDataReader<T>
    {
        private Thread readerThread;
        private MemoryMappedFilesEditor<T> reader;
        private List<SharedDataListener<T>> listeners;

        public SharedDataReader(string readingFileName)
        {
            this.readerThread = new Thread(startReadingData);
            this.reader = new MemoryMappedFilesEditor<T>(readingFileName);
            this.listeners = new List<SharedDataListener<T>>();
        }

        public void addListener(SharedDataListener<T> listener)
        {
            this.listeners.Add(listener);
        }

        public void removeListener(SharedDataListener<T> listener)
        {
            this.listeners.Remove(listener);
        }

        public void init()
        {
            this.readerThread.Start();
        }

        private void startReadingData()
        {
            while (true)
            {
                try
                {
					UnityEngine.Debug.Log("writing ");
                    T obj = reader.read();
					UnityEngine.Debug.Log("writing "+obj);
                    if (obj == null)
                    {
                        Thread.Sleep(getSleepTimeOnNoData()); // The writer is not ready yet. So, try after some time.
                        continue;
                    }

              //      Debug.WriteLine("[ READ ]: " + obj);
                    onReceivedNewData(obj);
                }
                catch (Exception e)
                {
//                    Debug.WriteLine("[ READ ]: Exception occurred: " + e.StackTrace);
                    Thread.Sleep(getSleepTimeOnError()); // The reader got error. So, try after some time.
                    continue;
                }
            }
        }

        protected int getSleepTimeOnNoData()
        {
            return 50;
        }

        protected int getSleepTimeOnError()
        {
            return 2000;
        }

        protected void onReceivedNewData(T receivedData)
        {
          //  Debug.WriteLine("Data received: " + receivedData);

            foreach (SharedDataListener<T> listener in this.listeners)
            {
                listener.onDataReceived(receivedData);
            }
        }
    }
}
