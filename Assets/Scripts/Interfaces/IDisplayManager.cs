using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void DisplayCallback(IDisplay display);
	public interface IDisplay {
		Transform Transform { get; }
		CanvasGroup Group { get; }
		Reticule Reticule { get; }
		void OnPreShow();
		void OnPreHide();

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

		//Kill the current one and show the default, mostly used for hard reset. This will reset the stack
		IPromise PushDefault();
	}
}