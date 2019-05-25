using UnityEngine;
using UnityEngine.UI;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using DG.Tweening;
using TMPro;
using System.Linq;

namespace Sojourn.ARDefense.Components {
	//`Mat raycast things inside the reticule
	public class Reticule : MonoBehaviour {
		[SerializeField]
		private Image _reloadMeter = null;
		[SerializeField]
		private CanvasGroup _highlightGroup = null;
		[SerializeField]
		private TMP_Text _reloadPercentage = null;
		// [SerializeField]
		// private Image _reloadFill = null;
		[SerializeField]
		[Tooltip("How many seconds a full reload takes when reloding")]
		private float _reloadSpeed = 1.0f;
		[SerializeField]
		[Tooltip("How many seconds after firing until reoad starts (0 for never)")]
		private float _reloadDelay = 0.0f;
		[SerializeField]
		[Tooltip("Radius of the SphereCast used to determine what is in the reticule")]
		private float _radius = 0.2f;

		[AutoInject]
		private IGameManager _gameManager = null;

		private List<GameObject> _targetedObjects = new List<GameObject>();

		private RectTransform _rect;
		private CanvasGroup _group;
		public Vector2 ScreenPosition {
			get {
				return RectTransformUtility.WorldToScreenPoint(_gameManager.DeviceCamera, this.transform.position); ;
			}
		}

		public float ReloadSpeed { get => _reloadSpeed; set => _reloadSpeed = value; }

		private void Awake() {
			_group = GetComponent<CanvasGroup>();
			_rect = GetComponent<RectTransform>();
		}

		private void Start() {
			Container.Inject(this);
		}

		public void OnFire() {
			if (_reloadDelay > 0.0) {
				Invoke("StartReload", _reloadDelay);
			} else {
				Reload();
			}
		}

		[ContextMenu("Reload")]
		private void ResetReload() {
			_reloadMeter.fillAmount = 0.0f;
			Reload();
		}

		private void Update() {
			List<GameObject> newObjects = new List<GameObject>();
			Ray ray = _gameManager.DeviceCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			//everything BUT the player
			int mask = ~LayerMask.GetMask("Player");
			RaycastHit[] hits = Physics.SphereCastAll(ray, _radius, Mathf.Infinity, mask);
			if (hits.Length > 0) {
				foreach (RaycastHit hit in hits) {
					// Debug.LogErrorFormat("Looking at: {0}", hit.transform.name);
					newObjects.Add(hit.transform.gameObject);
				}
			}

			//Object that were targeted, but not anymore
			foreach (GameObject obj in _targetedObjects.Except(newObjects)) {
				//`Mat Broadcast message
				obj.BroadcastMessage("OnUntargeted", SendMessageOptions.DontRequireReceiver);
			}
			//The newly targeted obejcts
			foreach (GameObject obj in newObjects.Except(_targetedObjects)) {
				//`Mat Broadcast message
				obj.BroadcastMessage("OnTargeted", SendMessageOptions.DontRequireReceiver);
			}
			_targetedObjects = newObjects;
		}

		public IPromise ShowHighlight(float time = 0.1f) { return _highlightGroup.Show(time); }
		public IPromise HideHighlight(float time = 0.1f) { return _highlightGroup.Hide(time); }

		public IPromise Unload() {
			float time = 0.1f;
			Sequence seq = DOTween.Sequence();
			seq.Join(_reloadMeter.DOFillAmount(0.0f, time));
			seq.Join(DOTween.To(x => _reloadMeter.fillAmount = x, _reloadMeter.fillAmount, 0.0f, time)
			.OnUpdate(() => { _reloadPercentage.SetText(string.Format("{0:P0}", _reloadMeter.fillAmount)); }));
			seq.AppendCallback(() => { _reloadPercentage.SetText(""); });
			return seq.ToPromise();
		}

		public IPromise Reload() {
			float time = _reloadSpeed * (1.0f - _reloadMeter.fillAmount);
			Sequence seq = DOTween.Sequence();
			seq.Join(_reloadMeter.DOFillAmount(1.0f, time));
			seq.Join(DOTween.To(x => _reloadMeter.fillAmount = x, _reloadMeter.fillAmount, 1.0f, time)
			.OnUpdate(() => { _reloadPercentage.SetText(string.Format("{0:P0}", _reloadMeter.fillAmount)); }));
			seq.AppendCallback(() => { _reloadPercentage.SetText(""); });
			// _reloadPercentage.SetText(string.Format("%{0:P0}", val));
			// seq.Join(_reloadFill.DOGradientColor(1.0f, time));
			return seq.ToPromise();
		}

		public void SetReload(float val) {
			val = Mathf.Clamp01(val);
			_reloadMeter.fillAmount = val;
			_reloadPercentage.SetText(string.Format("{0:P0}", val));
		}
	}
}