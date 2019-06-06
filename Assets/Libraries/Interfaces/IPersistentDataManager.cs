using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;

namespace Sojourn.Interfaces {
	interface IPersistentDataManager {
		void DeleteAll();
		void DeleteData(string key);
		void SaveData(string key, object obj);
		T LoadData<T>(string key);
	}
}