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
using UnityEngine.UI;
using TMPro;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(CanvasGroup))]
	public class SimpleHealthMeter : MonoBehaviour {
		[SerializeField]
		private Image _meterFill = null;
		[SerializeField]
		private TMP_Text _percentText = null;
		[SerializeField]
		private bool _showPercent = true;
		//~mat make this look ath the device camera
		[SerializeField]
		private bool _facePlayer = false;
		// [SerializeField]
		// private bool _animate = true;
		private CanvasGroup _canvasGroup;
		// private Tween _updateTween;

		[AutoInject]
		private IGameManager _gameManager = null;

		private void Awake() {
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start() {
			Container.AutoInject(this);
			SetValue(1.0f);
		}

		// private IPromise UpdateValue(float percent) {
		// 	if (_updateTween != null) { _updateTween.Kill(); }
		// 	_updateTween = DOTween.To ()
		// }

		public void ShowInstant() { _canvasGroup.ShowInstant(); }
		public void HideInstant() { _canvasGroup.HideInstant(); }

		public IPromise Show() { return _canvasGroup.Show(0.1f); }
		public IPromise Hide() { return _canvasGroup.Hide(0.1f); }

		private void Update() {
			if (_facePlayer) {
				this.transform.forward = _gameManager.DeviceCamera.transform.forward;
			}
		}

		private void SetValue(float percent) {
			_meterFill.fillAmount = percent;
			if (_showPercent) {
				_percentText.SetText(string.Format("{0:P0}", percent));
			}
		}

		public void OnKilled(IKillable us) { Destroy(this.gameObject); }
		public void OnDamaged(IKillable us) { SetValue((float)us.CurrentHealth / (float)us.MaxHealth); }
	}
}