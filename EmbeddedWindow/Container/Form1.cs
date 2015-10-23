using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.IO.Pipes;
using System.IO;
using Winterdom.IO.FileMap;
using System.Xml.Serialization;

namespace Container
{
    public partial class Form1 : Form, SharedDataListener<TestingData>
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, int wParam, uint lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private const int WA_ACTIVE = 1;
        private NamedPipeServerStream pipeServer;
        StreamWriter sw;
        bool mutexCreated;
        Mutex mutex;

        //create mutex

        public Form1()
        {
            InitializeComponent();
            initializeDataReceiver();
        }

        private void initializeDataReceiver()
        {
            // TODO: Send command to server for kinect data.

            UserDetectionDataReader.getInstance().addHandDataListener(this);
            UserDetectionDataReader.getInstance().addSkeletonDataListener(this);
        }

        public void onDataReceived(TestingData obj)
        {
            Debug.WriteLine("GOT GOT GOT: " + obj);
        }

        int x = 0;
        MemoryMappedFile map;
        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, 0);
            return 0;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, panel1.Width, panel1.Height, true);
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, 0);
        }

        // Close Unity application
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (process.HasExited == false)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e1)
        {
            try
            {
                // Read user input and send that to the client process. 



                // Send a 'sync message' and wait for client to receive it.
                //sw.WriteLine("SYNC");
                //pipeServer.WaitForPipeDrain();
                // Send the console input to the client process.
                // Console.Write("[SERVER] Enter text: ");
                sw.WriteLine(textBox1.Text.ToString());

            }
            // Catch the IOException that is raised if the pipe is broken 
            // or disconnected. 
            catch (IOException e)
            {
                Console.WriteLine("[SERVER] Error: {0}", e.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UserDetectionDataWriter.getInstance().writeHandData(new TestingData(10, "handData"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UserDetectionDataWriter.getInstance().writeSkeletonData(new TestingData(11, "writeHand"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }
    }
}

