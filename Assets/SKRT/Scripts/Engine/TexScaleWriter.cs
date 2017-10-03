using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
public class TexScaleWriter
{
	
	private static TexScaleWriter instance;
	public static TexScaleWriter singleton {
		get {
			if (instance == null) {
				instance = new TexScaleWriter ();
			}
			return instance;
		}
	}

	const string c_fileName = "/texScale.data";

	SerializableDictionary<int, float> texScaleMapX;
	SerializableDictionary<int, float> texScaleMapY;

	public TexScaleWriter(){
		texScaleMapX = new SerializableDictionary <int, float> (); 
		texScaleMapY = new SerializableDictionary <int, float> (); 

		if (File.Exists (Application.dataPath + c_fileName)) {
			FileStream file = File.Open (Application.dataPath + c_fileName, FileMode.Open);
			//XmlDocument doc = new XmlDocument ();
			XmlReader xmlReader = XmlReader.Create (file);
			xmlReader.ReadStartElement ("document");
			texScaleMapX.ReadXml (xmlReader);
			texScaleMapY.ReadXml (xmlReader);
			file.Close ();
		} else {
			Debug.LogWarning ("Tex scale data file not found.");
		}
	}

	public void UpdateValue(int uid, Vector3 texScale){
		Vector2 scale = new Vector2 (texScale.x, texScale.y);
		if (texScaleMapX.ContainsKey (uid)) {
			texScaleMapX [uid] = scale.x;
			texScaleMapY [uid] = scale.y;
		} else {
			texScaleMapX.Add(uid, scale.x);
			texScaleMapY.Add(uid, scale.y);
		}
	}

	public void WriteMapToFile(){
		FileStream file = File.Create(Application.dataPath + c_fileName);
		XmlWriterSettings settings = new XmlWriterSettings ();
		settings.NewLineHandling = NewLineHandling.Entitize;
		settings.Indent = true;
		XmlWriter xmlWriter = XmlWriter.Create (file, settings);
		xmlWriter.WriteStartElement ("document");
		//xmlWriter.WriteStartDocument ();
		//XmlDocument doc = new XmlDocument ();
		texScaleMapX.WriteXml (xmlWriter);
		texScaleMapY.WriteXml (xmlWriter);
		xmlWriter.WriteEndElement ();
		//xmlWriter.WriteEndDocument ();
		xmlWriter.Close ();
		file.Close ();

		Debug.LogWarning ("Writing tex scale data to file");
	}

	public Vector3 ReadValue(int uid){
		if (texScaleMapX.ContainsKey (uid)) {
			Vector2 scale = new Vector2 (texScaleMapX [uid], texScaleMapY [uid]);
			return new Vector3 (scale.x, scale.y, 1);
		} else {
			Debug.Log ("key not found: " + uid);
			texScaleMapX.Add (uid, 1);
			texScaleMapY.Add (uid, 1);
			return Vector3.one;
		}
	}
}


