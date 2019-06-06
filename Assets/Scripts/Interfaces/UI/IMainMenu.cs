using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	interface IMainMenu {
		void ShowInstant();
		void HideInstant();
		IPromise Show();
		IPromise Hide();
	}
}