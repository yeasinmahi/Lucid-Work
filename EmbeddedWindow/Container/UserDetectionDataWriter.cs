using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Container
{
    public class UserDetectionDataWriter
    {
        private static UserDetectionDataWriter instance = new UserDetectionDataWriter();

        private SharedDataWriter<TestingData> handDataWriter;
        private SharedDataWriter<TestingData> skeletonDataWriter;

        private UserDetectionDataWriter()
        {
            this.handDataWriter = new SharedDataWriter<TestingData>("handCursorData");
            this.skeletonDataWriter = new SharedDataWriter<TestingData>("skeletonData");
        }

        public static UserDetectionDataWriter getInstance()
        {
            return instance;
        }

        public void writeHandData(TestingData handData)
        {
            this.handDataWriter.writeData(handData);
        }

        public void writeSkeletonData(TestingData skeletonData)
        {
            this.skeletonDataWriter.writeData(skeletonData);
        }
    }
}
