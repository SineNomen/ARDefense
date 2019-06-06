using System;
using System.IO;
using Sojourn.Interfaces;
using UnityEngine;


namespace Sojourn.Utility {
	public class PersistentDataManager : IPersistentDataManager {
		private string _rootPath = null;//usually the app name

		public PersistentDataManager(string path = null) {
			if (path != null) {
				_rootPath = Path.Combine(path, "SaveData");
			} else {
				_rootPath = "SaveData";
			}
			if (!Directory.Exists(GetRootPath())) {
				Directory.CreateDirectory(GetRootPath());
			}
		}

		public void SaveData(string key, object obj) {
			string json = JsonUtility.ToJson(obj);
			WriteJSON(key, json);
		}

		public T LoadData<T>(string key) {
			string json = ReadJSON(key);
			if (json == null) {
				return default(T);
			} else {
				T obj = JsonUtility.FromJson<T>(json);
				return obj;
			}
		}

		public void DeleteAll() {
			foreach (string file in Directory.EnumerateFiles(GetRootPath())) {
				DeleteData(file);
			}
		}
		public void DeleteData(string key) {
			string path = GetPathForKey(key);
			File.Delete(path);
		}

		private void WriteJSON(string key, string json) {
			string path = GetPathForKey(key);
			using (StreamWriter writer = new StreamWriter(path)) {
				writer.Write(json);
			}
		}

		private string GetRootPath() {
			return Path.Combine(Application.persistentDataPath, _rootPath);
		}

		private string GetPathForKey(string key) {
			return Path.Combine(Application.persistentDataPath, _rootPath, string.Format("{0}.json", key));
		}

		private string ReadJSON(string key) {
			string path = GetPathForKey(key);
			string json = null;
			if (File.Exists(path)) {
				using (StreamReader writer = new StreamReader(path)) {
					json = writer.ReadToEnd();
				}
			}
			return json;
		}
	}
}