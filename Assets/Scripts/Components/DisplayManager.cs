using Sojourn.Utility;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	//Manages display that are meant to be used during gameplay (like an airplane HUD)
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
			Container.Register<IDisplayManager>(this).AsSingleton();
		}

		private void Start() {
			PushDefault();
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
			//`Mat Broadcast message
			display.Transform.BroadcastMessage("OnPreShow", this, SendMessageOptions.RequireReceiver);
			//`Mat Broadcast message
			previous?.Transform.BroadcastMessage("OnPreShow", this, SendMessageOptions.RequireReceiver);
			// previous?.OnPreHide();
			return Utilities.PromiseGroupSafe(
				display.Show(),
				previous?.Hide()
			)
			.Then(() => {
				//`Mat Broadcast message
				previous.Transform.BroadcastMessage("OnHide", this, SendMessageOptions.RequireReceiver);
			});
		}

		//Kill the current one and show the previous one, mostly used for hard reset
		public IPromise PopDisplay() {
			if (_displayStack.Count == 1) {
				return PushDefault();
			}
			IDisplay old = _displayStack.Pop();
			//`Mat Broadcast message
			CurrentDisplay.Transform.BroadcastMessage("OnPreShow", this, SendMessageOptions.RequireReceiver);
			return Utilities.PromiseGroupSafe(
				old.Hide(),
				CurrentDisplay.Show()
			)
			.Then(() => {
				//`Mat Broadcast message
				old.Transform.BroadcastMessage("OnHide", this, SendMessageOptions.RequireReceiver);
			});
		}

		//Kill the current one and show the default, mostly used for hard reset. This will reset the stack
		public IPromise ClearAll() {
			List<IPromise> list = new List<IPromise>();
			foreach (IDisplay display in _displayStack) {
				display.OnHide();
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
			return Utilities.PromiseGroup(
				ClearAll(),
				PushDisplay(display)
			);
		}
	}
}