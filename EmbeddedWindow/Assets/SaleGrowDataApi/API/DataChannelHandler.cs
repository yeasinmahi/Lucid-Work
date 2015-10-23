using FileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    /// <summary>
    /// Client (say, unity) will use this class for any communication with the parent application.
    /// </summary>
    public class DataChannelHandler
    {
        private static DataChannelHandler instance = new DataChannelHandler();


        private SharedStreamDataReader<EmbedHandDataV1> handDataReader;
        private SharedStreamDataReader<EmbedSkeletonDataV1> skeletonDataReader;
        private SharedDataReader<EmbedResponseDataV1> eventDataReader;

        private DataChannelHandler()
        {

            this.handDataReader = new SharedStreamDataReader<EmbedHandDataV1>("handCursorData");
            this.skeletonDataReader = new SharedStreamDataReader<EmbedSkeletonDataV1>("skeletonData");
            this.eventDataReader = new SharedDataReader<EmbedResponseDataV1>("eventData");
        }

        public static DataChannelHandler getInstance()
        {
            return instance;
        }

        public void init()
        {
	        this.handDataReader.init();
            this.skeletonDataReader.init();
            this.eventDataReader.init();
        }

        public void addKinectHandDataListener(SharedDataListener<EmbedHandDataV1> listener)
        {
            this.handDataReader.addListener(listener);
        }

        public void addKinectSkeletonDataListener(SharedDataListener<EmbedSkeletonDataV1> listener)
        {
            this.skeletonDataReader.addListener(listener);
        }

        public void addKinectEventDataListener(SharedDataListener<EmbedResponseDataV1> listener)
        {
            // TODO: Process the response here ==> Extract the event object ==> Cast to correct Embed class ==> Send to the consumer.

            this.eventDataReader.addListener(listener);
        }

        public void enableKinectGrip()
        {
        }

        public void disableKinectGrip()
        {
        }

        public void enableKinectHandData()
        {
        }

        public void disableKinectHandData()
        {
        }

        public void enableKinectSkeletonData()
        {
        }

        public void disableKinectSkeletonData()
        {
        }

        private static EmbedRequestDataV1 createEmbedRequest(string requestType)
        {
            EmbedRequestDataV1 requestData = new EmbedRequestDataV1();
            requestData.requestId = ""; // TODO: Get random string.
            requestData.requestType = requestType;
            return requestData;
        }
    }
}
