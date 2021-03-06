using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(CanvasGroup))]
	public class SimpleDisplay : MonoBehaviour, IDisplay {
		[SerializeField]
		protected SimpleReticule _reticule = null;
		[SerializeField]
		private IFFTracker tracker = null;

		public Transform Transform { get => transform; }
		public CanvasGroup Group { get; private set; }
		public IReticule Reticule { get => _reticule; }
		public IFFTracker Tracker { get => tracker; }

		protected virtual void Awake() {
			Group = GetComponent<CanvasGroup>();
		}

		protected virtual void Start() {
			Container.AutoInject(this);
		}

		public void OnPreShow() { }
		public void OnHide() { }

		public IPromise Show() { return Group.Show(0.25f); }
		public IPromise Hide() { return Group.Hide(0.25f); }
		public IPromise HideAndDestroy() {
			return Group.Hide(0.25f)
			.Then(() => { Destroy(this.gameObject); });
		}
	}
}