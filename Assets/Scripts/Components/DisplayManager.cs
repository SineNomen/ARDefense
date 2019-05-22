using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;


public class DisplayManager : MonoBehaviour, IDisplayManager {
	[SerializeField]
	private Transform _displayParent = null;
	[SerializeField]
	private GameObject _defaultDisplay = null;

	public Transform DisplayParent { get => _displayParent; }
	public IDisplay CurrentDisplay { get => _displayStack.Peek(); }
	public IDisplay DefaultDisplay { get; private set; }
	public DisplayCallback OnShowDisplay { get; set; }

	public Stack<IDisplay> _displayStack = new Stack<IDisplay>();

	private void Awake() {
		PushDefault();
		Container.Register<IDisplayManager>(this).AsSingleton();
	}

	//always show and immediately end the curent one
	public IPromise PushDisplay(IDisplay display) {
		IDisplay previous = (_displayStack.Count > 0 ? _displayStack.Peek() : null);
		_displayStack.Push(display);

		display.Group.HideInstant();
		display.Transform.SetParent(DisplayParent);
		RectTransform rect = display.Transform as RectTransform;
		rect.localPosition = Vector2.zero;
		rect.localScale = Vector2.one;
		//`Mat this assumes it is set to stretch to the corners
		rect.sizeDelta = Vector2.zero;
		//neep a set of promise
		display.OnPreShow();
		previous?.OnPreHide();
		return Utility.PromiseGroupSafe(
			display.Show(),
			previous?.Hide()
		);
	}

	//Kill the current one and show the previous one, mostly used for hard reset
	public IPromise PopDisplay() {
		if (_displayStack.Count == 1) {
			return PushDefault();
		}
		IDisplay old = _displayStack.Pop();
		CurrentDisplay.OnPreShow();
		old.OnPreHide();
		return Utility.PromiseGroupSafe(
			old.Hide(),
			CurrentDisplay.Show()
		);
	}

	//Kill the current one and show the default, mostly used for hard reset. This will reset the stack
	public IPromise ClearAll() {
		List<IPromise> list = new List<IPromise>();
		foreach (IDisplay display in _displayStack) {
			display.OnPreHide();
			list.Add(display.Hide());
		}
		_displayStack.Clear();
		return new Promise().All(list);
	}

	public IPromise PushDefault() {
		//the first one is always the default one
		if (_displayStack.Count == 1) {
			return Promise.Resolved();
		}
		IDisplay display = Instantiate(_defaultDisplay).GetComponent<IDisplay>();
		return Utility.PromiseGroup(
			ClearAll(),
			PushDisplay(display)
		);
	}
}
