using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Winterdom.IO.FileMap;
using UnityEngine;

namespace FileMap
{
    public class MemoryMappedFilesEditor<T>
    {
        private Mutex mutex;
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
				mutex = Mutex.OpenExisting("MyMutex");
			}
			catch
			{
				bool mutexCreated;
				mutex = new Mutex(false, "MyMutex", out mutexCreated);
				if (mutexCreated == false)
				{
					Console.WriteLine("Mutex error");
					return;
				}
			}

            try
            {
				mutex.WaitOne();
				openOrCreateMemoryFileAndWriteData(data);
            }
            finally
            {
				mutex.ReleaseMutex();
            }
        }

        public T read()
        {
			try
			{
				mutex = Mutex.OpenExisting("MyMutex");
			}
			catch
			{
				bool mutexCreated;
				mutex = new Mutex(false, "MyMutex", out mutexCreated);
				if (mutexCreated == false)
				{
					Console.WriteLine("Mutex error");
					throw new Exception("mutex creation exception khaise");;
				}
			}
			try {
				mutex.WaitOne();
	            return openMemoryFileAndReadData ();
			} finally {
				mutex.ReleaseMutex();
			}
        }

        public void close()
        {
            reader.Flush();
            mapFile.Close();
		}
		
		T openMemoryFileAndReadData ()
		{
			if (mapFile == null) {
				try {
					mapFile = MemoryMappedFile.Open (MapAccess.FileMapAllAccess, fileName);
					UnityEngine.Debug.Log ("read mutex e dhukse");
				}
				catch {
					UnityEngine.Debug.Log ("mmf read exception");
					mapFile = null;
					throw new Exception ("File " + fileName + " is not opened yet.");
				}
			}
			reader = mapFile.MapView (MapAccess.FileMapRead, 0, 8 * 1024);
			UnityEngine.Debug.Log ("mmf read");
			return getObjectFromMMF ();
		}

        private T getObjectFromMMF()
        {
            int a;
            List<char> d = new List<char>();
            while (true)
            {
                a = getData();
                if (a < 0)
                {
                    break;
                }
                d.Add((char)a);
            }
			string fdata = new string(d.ToArray());

            reader.Close();
			mapFile.Close ();
            mapFile.Dispose();
            
            try
            {
				UnityEngine.Debug.Log ("fetched"+fdata);
                return deserializeFromXML<T>(fdata);
            }
            catch (Exception)
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

        private void openOrCreateMemoryFileAndWriteData(T data)
        {
            try
            {
                mapFile = MemoryMappedFile.Open(MapAccess.FileMapWrite, fileName);
            }
            catch
            {
                mapFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, 8 * 1024, fileName);
            }

           
            char[] charArrayOfObject = serializeToXml<T>(data).ToCharArray();
			Stream writer = mapFile.MapView(MapAccess.FileMapWrite, 0, charArrayOfObject.Length);

            for (int i = 0; i < charArrayOfObject.Length; i++)
            {
                writer.WriteByte((byte)charArrayOfObject[i]);
            }
            writer.Flush();
            writer.Close();
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
