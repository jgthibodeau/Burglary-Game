using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SavableScript : UniqueId {
	public SaveData savedata;
	
	public virtual void UpdateSaveData(){
		savedata.position = transform.position;
		savedata.rotation = transform.rotation.eulerAngles;
		savedata.uid = uid;
		//TODO set all other savables
	}
	
	public virtual void SetFromSaveData(){
		UnityEngine.Debug.Log (transform.position);
		transform.position = savedata.position;
		Vector3 rotation = savedata.rotation;
		transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
//		uniqueId = savedata.uniqueId;
		//TODO set all other loadables
	}
}

// === This is the info container class ===
[Serializable ()]
public class SaveData : ISerializable {
	public string uid;

	public Vector3 position;
	public Vector3 rotation;

	// The default constructor. Included for when we call it during Save() and Load()
	public SaveData () {}
	
	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveData (SerializationInfo info, StreamingContext ctxt)
	{
		foreach(FieldInfo field in this.GetType().GetFields()){
//			UnityEngine.Debug.Log(field.Name+" "+field.FieldType);
			if(field.FieldType == typeof(Vector3))
				field.SetValue(this, LoadVector3(info, field.Name));
			else if(field.FieldType == typeof(Quaternion))
				field.SetValue(this, LoadQuaternion(info, field.Name));
			else if(field.FieldType == typeof(Transform))
				LoadTransform(info, field);
			else
				field.SetValue(this, info.GetValue(field.Name, field.FieldType));
		}
	}
	
	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
		foreach(FieldInfo field in this.GetType().GetFields()){
			if(field.FieldType == typeof(Vector3))
				SaveVector3(info, (Vector3)field.GetValue(this), field.Name);
			else if(field.FieldType == typeof(Quaternion))
				SaveQuaternion(info, (Quaternion)field.GetValue(this), field.Name);
			else if(field.FieldType == typeof(Transform))
				SaveTransform(info, (Transform)field.GetValue(this), field.Name);
			else
				info.AddValue(field.Name, field.GetValue(this));
		}
	}

	public void SaveVector3(SerializationInfo info, Vector3 vector, String name){
		info.AddValue(name+"x", vector.x);
		info.AddValue(name+"y", vector.y);
		info.AddValue(name+"z", vector.z);
	}
	public Vector3 LoadVector3(SerializationInfo info, String name){
		float x = (float)info.GetValue(name+"x", typeof(float));
		float y = (float)info.GetValue(name+"y", typeof(float));
		float z = (float)info.GetValue(name+"z", typeof(float));
		return new Vector3 (x, y, z);
	}
	public void SaveQuaternion(SerializationInfo info, Quaternion quaternion, String name){
		info.AddValue(name+"w", quaternion.w);
		info.AddValue(name+"x", quaternion.x);
		info.AddValue(name+"y", quaternion.y);
		info.AddValue(name+"z", quaternion.z);
	}
	public Quaternion LoadQuaternion(SerializationInfo info, String name){
		float w = (float)info.GetValue(name+"w", typeof(float));
		float x = (float)info.GetValue(name+"x", typeof(float));
		float y = (float)info.GetValue(name+"y", typeof(float));
		float z = (float)info.GetValue(name+"z", typeof(float));
		return new Quaternion (x, y, z, w);
	}
	public void SaveTransform(SerializationInfo info, Transform transform, String name){
		SaveVector3(info, (Vector3)transform.position, name+"position");
		SaveQuaternion(info, (Quaternion)transform.rotation, name+rotation);
	}
	public void LoadTransform(SerializationInfo info, FieldInfo field){
		Transform transform = (Transform)field.GetValue (this);
		transform.position = LoadVector3 (info, field.Name + "position");
		transform.rotation = LoadQuaternion (info, field.Name + "rotation");
		field.SetValue (this, transform);
	}
}

[Serializable ()]
public class LoadedLevel : ISerializable {
	public string level;

//	public LoadedLevel () : base () {}
//	public LoadedLevel (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
	public LoadedLevel () {}
	
	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public LoadedLevel (SerializationInfo info, StreamingContext ctxt)
	{
		foreach(FieldInfo field in this.GetType().GetFields()){
			field.SetValue(this, info.GetValue(field.Name, field.FieldType));
		}
	}
	
	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
		foreach (FieldInfo field in this.GetType().GetFields()) {
			info.AddValue (field.Name, field.GetValue (this));
		}
	}
	
}

// === This is the class that will be accessed from scripts ===
public class SaveLoad {

	public static string saveDir = "Save\\";
	public static string defaultSaveFile = "SaveData";    // Edit this for different save files

	public static FileInfo[] GetSaves(){
		DirectoryInfo dir = new DirectoryInfo("Save");
		FileInfo[] info = dir.GetFiles("*.bly");
		return info;
	}

	public static void Save (Stream stream, object data) {
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		bformatter.Serialize(stream, data);
	}
	public static void SaveAll() {
		DateTime currentTime = DateTime.Now;
		SaveAll (defaultSaveFile + " " + currentTime.Month + "-" + currentTime.Day + "-" + currentTime.Year + " " + currentTime.Hour + "-" + currentTime.Minute + "-" + currentTime.Second);
	}
	public static void SaveAll (string filePath) {
		filePath += ".bly";
		Stream stream = File.Open (saveDir+filePath, FileMode.Create);

		UnityEngine.Debug.Log ("saving to "+filePath);
		UnityEngine.Debug.Log ("saving level");

		LoadedLevel level = new LoadedLevel ();
		level.level = Application.loadedLevelName;
		Save (stream, level);

		UnityEngine.Debug.Log ("saving objects");

		foreach (SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ()) {
			obj.UpdateSaveData();
			UnityEngine.Debug.Log ("saving "+obj.name+" with id "+obj.savedata.uid);
			Save (stream, obj.savedata);
		}
		stream.Close ();

		UnityEngine.Debug.Log ("done saving");
	}
	
	// Call this to load from a file into "data"
	public static object Load (Stream stream) {
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		return bformatter.Deserialize(stream);
	}
	public static void LoadAll(){
		LoadAll (defaultSaveFile);
	}
	public static void LoadAll (string filePath) {
		StaticCoroutine.DoCoroutine(RealLoadAll(filePath));
	}
	static IEnumerator RealLoadAll (string filePath){
		Stream stream = File.Open(saveDir+filePath, FileMode.Open);

		UnityEngine.Debug.Log ("loading from "+filePath);
		UnityEngine.Debug.Log ("loading level");

		LoadedLevel level = (LoadedLevel)Load (stream);
		GlobalScript.LoadLevel (level.level);
		bool loading = Application.isLoadingLevel;
		while (loading) {
			yield return null;
			yield return null;
			loading = Application.isLoadingLevel;
		}

		UnityEngine.Debug.Log ("loading objects");

		while (stream.Position < stream.Length) {
			SaveData data = (SaveData)Load (stream);
			UnityEngine.Debug.Log("finding "+data.uid);
			SavableScript obj = GetObjectWithId(data.uid);
			if(obj == null)
				UnityEngine.Debug.LogError("couldn't find object with id "+data.uid);
			else{
				UnityEngine.Debug.Log("found "+obj.name+" with id "+data.uid);
				obj.savedata = data;
				obj.SetFromSaveData ();
			}
		}

		stream.Close();
		UnityEngine.Debug.Log ("done loading");
	}

	static SavableScript GetObjectWithId(string id){
		foreach (SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ()) {
			if(String.Compare(obj.uid, id) == 0)
				return obj;
		}
		return null;
	}
}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder 
{ 
	public override Type BindToType( string assemblyName, string typeName )
	{ 
		if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
		{ 
			Type typeToDeserialize = null; 
			
			assemblyName = Assembly.GetExecutingAssembly().FullName; 
			
			// The following line of code returns the type. 
			typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
			
			return typeToDeserialize; 
		} 
		
		return null; 
	} 
}
