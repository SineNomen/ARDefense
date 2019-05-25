using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace Sojourn.ARDefense.Components {
	// [[RequireComponent (typeof (MeshRenderer))]]
	public class ShowHealth : MonoBehaviour {
		[SerializeField]
		private GameObject _healthPrefab;
		[SerializeField]
		private Vector3 _meterOffset;
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
		private SimepleHealthMeter _meter = null;
		private Color _startColor = Color.white;
		private Tween _flashTween = null;
		private IPromise _showPromise = null;

		// [AutoInject]
		// private IGameManager _gameManager = null;

		private void Awake() {
			_meter = Instantiate(_healthPrefab, this.transform).GetComponent<SimepleHealthMeter>();
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