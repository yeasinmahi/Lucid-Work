using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    public class EmbedHandDataV1
    {
        public EmbedHandTypeDataV1 handType;

        public EmbedHandEventTypeDataV1 handEventType;

        public bool isTracked;

        public bool isActive;

        public bool isInteractive;

        public bool isPressed;

        public bool isPrimaryHandOfUser;

        public bool isPrimaryUser;

        public bool isInGripInteraction;

        public double pressExtent;

        public double X;

        public double Y;

        public EmbedHandDataV1()
        {
        }
    }
}
