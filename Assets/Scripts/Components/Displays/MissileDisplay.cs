using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.Extensions;
using Sojourn.PicnicIOC;
using UnityEngine;
using UnityEngine.UI;
using AOFL.Promises.V1.Interfaces;

[RequireComponent(typeof(CanvasGroup))]
public class MissileDisplay : MonoBehaviour, IDisplay {
	[SerializeField]
	private TrackingReticule _reticule = null;
	[SerializeField]
	private Button _fireButton = null;
	[SerializeField]
	private IFFTracker tracker = null;

	public Transform Transform { get => transform; }
	public CanvasGroup Group { get; private set; }
	public IReticule Reticule { get => _reticule; }
	public IFFTracker Tracker { get => tracker; }

	[AutoInject]
	private IPlayer _player1 = null;
	[AutoInject]
	private IDisplayManager _displayManager;

	protected virtual void Awake() {
		Group = GetComponent<CanvasGroup>();
	}

	public void OnPreShow() { }
	public void OnHide() { }

	public IPromise Show() { return Group.Show(0.25f); }
	public IPromise Hide() { return Group.Hide(0.25f); }
	public IPromise HideAndDestroy() {
		return Group.Hide(0.25f)
		.Then(() => { Destroy(this.gameObject); });
	}

	protected virtual void Start() {
		Container.AutoInject(this);
		_fireButton.onClick.AddListener(FireCannon);
	}

	private void FireCannon() {
		if (Reticule.LockedObject != null) {
			IPromise p = _player1.RequestFireCannon();
			if (p != null) {
				p = p.Chain(_reticule.Unload);
				p.Then(() => {
					_reticule.ReloadSpeed = _player1.CurrentWeapon.ReloadTime;
					_reticule.Reload();
				});
			}
		}
	}
}