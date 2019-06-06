using Sojourn.ARDefense.Components;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void DisplayCallback(IDisplay display);
	public interface IDisplay {
		Transform Transform { get; }
		CanvasGroup Group { get; }
		Reticule Reticule { get; }
		IFFTracker Tracker { get; }
		void OnPreShow();
		void OnHide();

		IPromise Show();
		IPromise Hide();
	}

	public interface IDisplayManager {
		Transform DisplayParent { get; }
		IDisplay CurrentDisplay { get; }
		IDisplay DefaultDisplay { get; }
		DisplayCallback OnShowDisplay { get; set; }

		//always show and immediately end the curent one
		IPromise PushDisplay(IDisplay display);

		//Kill the current one and show the previous one
		IPromise PopDisplay();

		//Reset the stack and show the default, mostly used for hard reset.
		IPromise PushDefault();
	}
}