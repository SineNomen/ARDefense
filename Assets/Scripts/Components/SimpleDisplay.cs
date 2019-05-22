using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleDisplay : MonoBehaviour, IDisplay {
	[SerializeField]
	private Reticule _reticule = null;

	public Transform Transform { get => transform; }
	public CanvasGroup Group { get; private set; }
	public Reticule Reticule { get => _reticule; }

	private void Awake() {
		Group = GetComponent<CanvasGroup>();
	}

	public void OnPreShow() { }
	public void OnPreHide() { }

	public IPromise Show() { return Group.Show(0.25f); }
	public IPromise Hide() { return Group.Hide(0.25f); }
}