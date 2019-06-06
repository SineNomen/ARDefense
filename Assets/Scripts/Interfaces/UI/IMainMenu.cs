using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	interface IMainMenu {
		void ShowInstant();
		void HideInstant();
		IPromise Show();
		IPromise Hide();
	}
}