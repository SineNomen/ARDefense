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
		private RectTransform _modalParent = null;
		[SerializeField]
		private GameObject _defaultDisplay = null;
		[SerializeField]
		private GameObject _okCancelModalPrefab = null;

		public Transform DisplayParent { get => _displayParent; }
		public RectTransform ModalParent { get => _modalParent; }
		public IDisplay CurrentDisplay { get => _displayStack.Peek(); }
		public IDisplay DefaultDisplay { get; private set; }
		public DisplayCallback OnShowDisplay { get; set; }

		public Stack<IDisplay> _displayStack = new Stack<IDisplay>();
		public Dictionary<GameObject, IDisplay> _prefabMap = new Dictionary<GameObject, IDisplay>();

		private void Awake() {
			Container.Register<IDisplayManager>(this).AsSingleton();
		}

		private void Start() {
			PushDefault();
		}

		public IPromise PushDisplay(GameObject prefab) {
			IDisplay display = GetDisplay<IDisplay>(prefab);
			return ShowDisplay(display);
		}

		private T GetDisplay<T>(GameObject prefab) where T : IDisplay {
			T display;
			if (_prefabMap.ContainsKey(prefab)) {
				display = (T)_prefabMap[prefab];
			} else {
				display = Instantiate(prefab).GetComponent<T>();
				_prefabMap.Add(prefab, display);
			}

			return display;
		}

		public IPromise<T> PushDisplay<T>(GameObject prefab) where T : IDisplay {
			T display = GetDisplay<T>(prefab);
			Promise<T> p = new Promise<T>();
			ShowDisplay(display)
			.Then<T>(p.Resolve, display);
			return p;
		}

		//always show and immediately end the curent one
		private IPromise ShowDisplay(IDisplay display) {
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
				ShowDisplay(display)
			);
		}

		public IPromise<eModalOption> ShowOKCancelModal(string title, string bodyText, string okText = "OK", string cancelText = "Cancel") {
			OKCancelModal modal = Instantiate(_okCancelModalPrefab, _modalParent).GetComponent<OKCancelModal>();
			modal.Setup(title, bodyText, okText, cancelText);
			return modal.DoModal();
		}
		public IPromise<eModalOption> ShowOKModal(string title, string bodyText, string okText = null) {
			if (okText == null) { okText = "OK"; }
			return ShowOKCancelModal(title, bodyText, okText, null);
		}
	}
}