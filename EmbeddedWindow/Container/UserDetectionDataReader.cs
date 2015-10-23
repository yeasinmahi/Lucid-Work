using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Container
{
    public class UserDetectionDataReader
    {
        private static UserDetectionDataReader instance = new UserDetectionDataReader();

        private SharedDataReader<TestingData> handCursorDataReceiver;
        private SharedDataReader<TestingData> skeletonDataReceiver;

        private UserDetectionDataReader()
        {
            this.handCursorDataReceiver = new SharedDataReader<TestingData>("handCursorData");
            this.skeletonDataReceiver = new SharedDataReader<TestingData>("skeletonData");

            this.handCursorDataReceiver.init();
            //this.skeletonDataReceiver.init();
        }

        public static UserDetectionDataReader getInstance()
        {
            return instance;
        }

        public void addHandDataListener(SharedDataListener<TestingData> listener)
        {
            this.handCursorDataReceiver.addListener(listener);
        }

        public void addSkeletonDataListener(SharedDataListener<TestingData> listener)
        {
            this.skeletonDataReceiver.addListener(listener);
        }
    }
}
