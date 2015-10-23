using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Winterdom.IO.FileMap;
using winterdom_filemap;
using MemoryMappedFile = Winterdom.IO.FileMap.MemoryMappedFile;

namespace memoryMapFile_Test
{
    public partial class Form1 : Form
    {
        private MemoryMappedFile mapFile;
        private Mutex mutex;
        private bool mutexCreated;
        private Stream reader;
        private const string fileName = "testFile";

        public Form1()
        {
            InitializeComponent();
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    mutex = Mutex.OpenExisting("MyMutex");
                }
                catch
                {
                    mutex = new Mutex(false, "MyMutex", out mutexCreated);
                }

                if (mutexCreated == false)
                {
                    Console.WriteLine("Mutex error");
                }
                TestData obj= new TestData();
                
                Thread writeThread = new Thread(delegate()
                {
                    while (true)
                    {
                        openOrCreateMemoryAndWriteData(obj);
                        //Thread.Sleep(2000);
                    }
                    
                });
                
                writeThread.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @"Writing Error occured");
            }
        }

        private void readButton_Click(object sender, EventArgs e)
        {
            try
            {
                mutex = Mutex.OpenExisting("MyMutex");
            }
            catch
            {
                mutex = new Mutex(false, "MyMutex", out mutexCreated);
            }
            if (mutexCreated == false)
            {
                Console.WriteLine("Mutex error");
                return;
            }

            
            Thread readThread = new Thread(delegate()
            {
                
                while (true)
                {
                    mutex.WaitOne();
                    readDataFromMemory();
                    //Thread.Sleep(2000);
                }

            });

            readThread.Start();

            

        }

        private void readDataFromMemory()
        {
            if (mapFile == null)
            {
                try
                {
                    mapFile = MemoryMappedFile.Open(MapAccess.FileMapAllAccess, fileName);
                }
                catch
                {
                    mapFile = null;
                    throw new Exception("File " + fileName + " is not opened yet.");
                }
            }

            try
            {
                reader = mapFile.MapView(MapAccess.FileMapRead, 0, 8 * 1024);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in Reading");
            }
            getObjectFromMMF<TestData>();
        }
        private void openOrCreateMemoryAndWriteData<T>(T data)
        {
            mutex.WaitOne();
            try
            {
                mapFile = MemoryMappedFile.Open(MapAccess.FileMapAllAccess, fileName);
            }
            catch
            {
                mapFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, 8 * 1024, fileName);
            }

            Stream writer = mapFile.MapView(MapAccess.FileMapAllAccess, 0, 8 * 1024);
            char[] charArrayOfObject = serializeToXml<T>(data).ToCharArray();

            
            for (int i = 0; i < charArrayOfObject.Length; i++)
            {
                writer.WriteByte((byte)charArrayOfObject[i]);
            }
            writer.Flush();
            writer.Close();
            mutex.ReleaseMutex();
        }

        private string serializeToXml<T>(T data)
        {
            string xmlData = null;

            StringWriter stringWriter = null;

            XmlSerializer serializer = new XmlSerializer(data.GetType());
            stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, data);

            xmlData = stringWriter.ToString();
            Debug.WriteLine("[Writing]: "+xmlData);
            Debug.WriteLine("");
            //MessageBox.Show(xmlData);
            stringWriter.Close();

            return xmlData;
        }

        private T deserializeFromXML<T>(string xmlData)
        {
            StringReader stringReader = null;
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            stringReader = new StringReader(xmlData);
            //MessageBox.Show(xmlData);
            Debug.WriteLine("[Reading]:"+ xmlData );
            T data = (T)deserializer.Deserialize(stringReader);
            stringReader.Close();

            return data;
        }

        private T getObjectFromMMF<T>()
        {
            int a;
            List<char> d = new List<char>();
            while (true)
            {
                a = getData();
                if (a <= 0)
                {
                    break;
                }
                d.Add((char)a);
            }
            reader.Close();
            
            mapFile.Dispose();
            mutex.ReleaseMutex();
            String fdata = new string(d.ToArray());
            try
            {
                return deserializeFromXML<T>(fdata);
            }
            catch (Exception e)
            {
                throw;// new Exception();
            }

        }

        private int getData()
        {
            int data = -1;
            try
            {
                return reader.ReadByte();
            }
            catch (Exception e)
            {
                return data;
            }

        }

    }
}
