using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	public interface IPlayerHUD {
		void ShowInstant();
		void HideInstant();
		IPromise Show();
		IPromise Hide();
	}
}