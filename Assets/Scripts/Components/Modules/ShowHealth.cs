using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using DG.Tweening;

namespace Sojourn.ARDefense.Components {
	// [[RequireComponent (typeof (MeshRenderer))]]
	[DisallowMultipleComponent]
	public class ShowHealth : MonoBehaviour {
		[SerializeField]
		private GameObject _healthPrefab = null;
		[SerializeField]
		private Vector3 _meterOffset = Vector3.zero;
		[SerializeField]
		private bool _showOnDamage = true;
		[SerializeField]
		private bool _showOnTargeted = true;
		[SerializeField]
		private bool _showAlways = false;
		[SerializeField]
		[Tooltip("How long to wait before hiding")]
		private float _hideDelay = 2.0f;

		//need to mke this int a component
		private SimpleHealthMeter _meter = null;
		private Color _startColor = Color.white;
		private Tween _flashTween = null;
		private IPromise _showPromise = null;

		// [AutoInject]
		// private IGameManager _gameManager = null;

		private void Awake() {
			_meter = Instantiate(_healthPrefab, this.transform).GetComponent<SimpleHealthMeter>();
			_meter.transform.localPosition = _meterOffset;
			if (!_showAlways) {
				_meter.HideInstant();
			}
		}

		public void OnDamage(IKillable us) {
			if (_showOnDamage && !_showAlways) {
				if (_showPromise != null) { _showPromise.RequestCancel(); }
				_showPromise = _meter.Show();
				// .Chain<float>(this.Wait, _hideDelay)
				// .Chain(_meter.Hide);
			}
		}
		public void OnTargeted() {
			if (_showOnTargeted && !_showAlways) {
				if (_showPromise != null) { _showPromise.RequestCancel(); }
				_showPromise = _meter.Show();
				// .Chain<float>(this.Wait, _hideDelay)
				// .Chain(_meter.Hide);
			}
		}
		public void OnUntargeted() {
			if (!_showAlways) {
				_meter.Hide();
			}
		}
	}
}