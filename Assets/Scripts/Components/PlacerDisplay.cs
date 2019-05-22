using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.Extensions;
using UnityEngine;
using UnityEngine.UI;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class PlacerDisplay : SimpleDisplay, IDisplay {
	[SerializeField]
	private Button _placeButton = null;

	public Button PlaceButton { get => _placeButton; }
}