using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PlacerDisplay : SimpleDisplay, IDisplay {
	[SerializeField]
	private Button _placeButton = null;

	public Button PlaceButton { get => _placeButton; }
}