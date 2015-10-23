using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System;
using System.IO.Pipes;
using Winterdom.IO.FileMap;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Samples.Kinect.ControlsBasics;
using FileMap;

public class Rotate : MonoBehaviour {
	public Text textField;

	NamedPipeClientStream clientProcess;
	MemoryMappedFile map; 

	Stream reader;
	MemoryMappedFilesEditor<EmbedHandDataV1> dataReader;
	// Use this for initialization
	void Start () {
		textField.text= "new text is assigned.";
		//reader= map.MapView(MapAccess.FileMapRead, 0, 8 * 1024);
		//Debug.Log ("reader lengthe: "+reader.Length);
		//ReadData ();
		//Debug.Log ("Again calling");

		//ReadData ();
//		while (true) {
//			a= GetData();
//			if(a==null || a==0){break;}
//			Debug.Log(a);
//		}
		
		//new Thread(new ThreadStart(receivingProcess)).Start();
		//StartCoroutine("Fade");
	//	DataChannelHandler.getInstance ().init ();
		dataReader=new MemoryMappedFilesEditor<EmbedHandDataV1>("handCursorData");
	}
	Mutex mutex;

	bool mutexCreated;

	void ReadData ()
	{
		EmbedHandDataV1 obj= dataReader.read ();
		Debug.Log ("received"+obj.X);
//		try
//		{
//			map = MemoryMappedFile.Open(MapAccess.FileMapRead, "handCursorData");
//		}
//		catch
//		{
//			map = MemoryMappedFile.Create(MapProtection.PageReadWrite, 8 * 1024, "handCursorData");
//		}
//		reader= map.MapView(MapAccess.FileMapRead, 0, 8 * 1024);
//		int a;
//		List<char> d = new List<char> ();
//		try
//		{
//			mutex = Mutex.OpenExisting("MyMutex");
//		}
//		catch
//		{
//			mutex = new Mutex(false, "MyMutex", out mutexCreated);
//		}
//		if (mutexCreated == false)
//		{
//			Console.WriteLine("Mutex error");
//			//return;
//		}
//		
//		mutex.WaitOne();
//		while(true) {
//			a = GetData ();
//			if (a <= 0) {
//				break;
//			}
//			d.Add ((char)a);
//			//Debug.Log ((char)a);
//		}
//		mutex.ReleaseMutex ();
//		string fdata = new string (d.ToArray ());
//		Debug.Log (fdata);
//		EmbedHandDataV1 obj=null;
//		try{
//		 obj = DeserializeFromXML (fdata);
//		}catch(Exception e)
//		{
//
//		}
//		//if (obj != null) {
//			//Debug.Log (obj.X);
//		//}

	}

	public static EmbedHandDataV1 DeserializeFromXML(string xmlData)
	{
		EmbedHandDataV1 data = null;
		
		StringReader stringReader = null;
		
		XmlSerializer deserializer = new XmlSerializer(typeof(EmbedHandDataV1));
		stringReader = new StringReader(xmlData);
		data = (EmbedHandDataV1)deserializer.Deserialize(stringReader);
		stringReader.Close();
		
		return data;
	}

	private int GetData()
	{
		int data = -1;
		//create file
		try
		{
			// Mutex mutex = Mutex.OpenExisting("MyMutex");
			
			//mutex.WaitOne();
			
			return reader.ReadByte();
			
		}
		catch (FileNotFoundException)
		{
			return data;
		}
		catch (WaitHandleCannotBeOpenedException)
		{
			return data;
		}
		catch (FileMapIOException)
		{
			return data;
		}
		
		//return
		return data;
	}
	int x=0;
	string temp="";
	IEnumerator Fade()
	{
		//yield return new WaitForSeconds (10);
		using (StreamReader sr = new StreamReader(ClientProcess.clientProcess))
	{
			//while ((temp= sr.ReadLine())!=null) {
				//temp= sr.ReadLine();
				x++;
				textField.text = x.ToString();
				yield return null;
			//}
	}
		
	}
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, Time.deltaTime * 80.0f);
		ReadData ();
	}

	void OnGUI()
	{
		GUILayout.Label("Build PC Standalone application to Export folder, and name the executable as Child.exe.");

	}

	private void receivingProcess()
	{
		clientProcess = new NamedPipeClientStream ("server");
		clientProcess.Connect ();
		//AnonymousPipeServerStream pipeServer = new
		//         AnonymousPipeServerStream(PipeDirection.Out);
		//string pipeHandle =
		//     pipeServer.GetClientHandleAsString();
		Debug.Log ("entered int thread.");
		using (StreamReader sr = new StreamReader(ClientProcess.clientProcess))
		{
			int a=0;
			// Display the read text to the console 
			string temp="";
			
			// Wait for 'sync message' from the server. 
			//do
			//{
			//    //Console.WriteLine("[CLIENT] Wait for sync...");
			//    temp = sr.ReadLine();
			//}
			//while (!temp.StartsWith("SYNC"));
			String x = "";
			// Read the server data and echo to the console. 
			while ((temp=sr.ReadLine())!=null)
			{
				//x = temp;
				Debug.Log("count: "+a++);
				//textField.text=temp;
				//this.Refresh();
				// Console.WriteLine("[CLIENT] Echo: " + temp);
			}
			
		}
	}
}
