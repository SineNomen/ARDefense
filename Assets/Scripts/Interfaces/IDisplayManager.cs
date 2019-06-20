using Sojourn.ARDefense.Components;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void DisplayCallback(IDisplay display);
	public interface IDisplay {
		Transform Transform { get; }
		CanvasGroup Group { get; }
		IReticule Reticule { get; }
		IFFTracker Tracker { get; }
		void OnPreShow();
		void OnHide();

		IPromise Show();
		IPromise Hide();
		IPromise HideAndDestroy();
	}

	public interface IDisplayManager {
		Transform DisplayParent { get; }
		RectTransform ModalParent { get; }
		IDisplay CurrentDisplay { get; }
		IDisplay DefaultDisplay { get; }
		DisplayCallback OnShowDisplay { get; set; }

		IPromise PushDisplay(GameObject prefab);
		IPromise<T> PushDisplay<T>(GameObject prefab) where T : IDisplay;

		//Kill the current one and show the previous one
		IPromise PopDisplay();

		//Reset the stack and show the default, mostly used for hard reset.
		IPromise PushDefault();
		IPromise<eModalOption> ShowOKCancelModal(string title, string bodyText, string okText = "OK", string cancelText = "Cancel");
		IPromise<eModalOption> ShowOKModal(string title, string bodyText, string okText = null);
	}
}