using UnityEngine;
using UnityEngine.UI;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class HeadsUpDisplay : MonoBehaviour {
	[Tooltip("How big the ground plane must be for us to use it")]
	[SerializeField]
	private Image _reloadMeter;

	private CanvasGroup _group;

	private void Awake() {
		_group = GetComponent<CanvasGroup>();
	}
}

