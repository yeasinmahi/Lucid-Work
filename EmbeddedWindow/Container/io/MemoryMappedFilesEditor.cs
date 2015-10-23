using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Winterdom.IO.FileMap;

namespace Container
{
    public class MemoryMappedFilesEditor<T>
    {
        private Mutex mutex;
        private bool mutexCreated;
        private MemoryMappedFile mapFile;
        private Stream reader;
        private string fileName;

        public MemoryMappedFilesEditor(string fileName)
        {
            this.fileName = fileName;
        }

        public void write(T data)
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

                openOrCreateMemoryAndWriteData(data);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            }
        }

        public T read()
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

            reader = mapFile.MapView(MapAccess.FileMapRead, 0, 8 * 1024);

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
                //return;
            }

            return getObjectFromMMF();

        }

        public void close()
        {
            reader.Flush();
            mapFile.Close();
        }

        private T getObjectFromMMF()
        {
            int a;
            List<char> d = new List<char>();
            mutex.WaitOne();
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
            string fdata = new string(d.ToArray());
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

        private void openOrCreateMemoryAndWriteData(T data)
        {
            try
            {
                mapFile = MemoryMappedFile.Open(MapAccess.FileMapWrite, fileName);
            }
            catch
            {
                mapFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, 8 * 1024, fileName);
            }

            Stream writer = mapFile.MapView(MapAccess.FileMapWrite, 0, 8 * 1024);
            char[] charArrayOfObject = serializeToXml<T>(data).ToCharArray();

            mutex.WaitOne();
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

            stringWriter.Close();

            return xmlData;
        }

        private T deserializeFromXML<T>(string xmlData)
        {
            StringReader stringReader = null;

            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            stringReader = new StringReader(xmlData);
            T data = (T)deserializer.Deserialize(stringReader);
            stringReader.Close();

            return data;
        }
    }
}
