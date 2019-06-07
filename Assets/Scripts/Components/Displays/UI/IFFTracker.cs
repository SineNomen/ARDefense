using UnityEngine;
using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using AOFL.Promises.V1.Interfaces;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	public class IFFTracker : MonoBehaviour {
		[SerializeField]
		private GameObject _indicatorPrefab = null;
		[SerializeField]
		private RectTransform _ring = null;
		[SerializeField]
		private RectTransform _indicatorParent = null;
		[SerializeField]
		[Tooltip("Objects within the deadzone do not show an indicator")]
		private float _deadZone = 20.0f;

		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private ILevelManager _levelManager = null;
		private float _ringRadius = 0.0f;

		private Dictionary<GameObject, IFFIndicator> _objectMap = new Dictionary<GameObject, IFFIndicator>();

		private void Awake() {
			_ringRadius = (_ring.rect.size.x + (_indicatorPrefab.transform as RectTransform).rect.size.x) / 2.0f;
		}

		private void Start() {
			Container.AutoInject(this);
			_levelManager.OnEnemyCreated += OnEnemyCreated;
			_levelManager.OnEnemyKilled += OnEnemyKilled;
			OnPreShow();
		}

		public IPromise TrackObject(GameObject go, eIFFCategory cat) {
			IFFIndicator indicator = Instantiate(_indicatorPrefab, _indicatorParent).GetComponent<IFFIndicator>();
			indicator.SetUp(cat);
			_objectMap[go] = indicator;
			return indicator.Show();
		}

		private void OnPreShow() {
			if (_gameManager == null) { return; }
			// Debug.LogFormat("IFFTracker.OnPreShow: {0}, Base: {1}", this.gameObject.name, _levelManager.PlayerBase);
			if (_levelManager.PlayerBase != null) {
				TrackObject(_levelManager.PlayerBase.gameObject, eIFFCategory.Friend);
			}
			foreach (GameObject obj in _levelManager.EnemyList) {
				if (!_objectMap.ContainsKey(obj)) {
					TrackObject(obj, eIFFCategory.Enemy);
				}
			}
		}

		private void OnHide() {
			//delete everything
			foreach (IFFIndicator indicator in _objectMap.Values) {
				Destroy(indicator.gameObject);
			}
			_objectMap.Clear();
		}

		private void OnEnemyCreated(GameObject enemy) {
			if (!_objectMap.ContainsKey(enemy)) {
				TrackObject(enemy, eIFFCategory.Enemy);
			}
		}

		private void OnEnemyKilled(GameObject enemy) {
			IFFIndicator indicator = null;
			if (_objectMap.TryGetValue(enemy, out indicator)) {
				indicator.Hide()
				.Then(() => { Destroy(indicator.gameObject); });
			}
		}

		private void Update() {
			foreach (KeyValuePair<GameObject, IFFIndicator> pair in _objectMap) {
				if (pair.Key != null && pair.Value != null) {
					UpdateObject(pair.Key, pair.Value);
				}
			}
		}

		//generator is alwyas in the deadzone for some reason

		//set the new position, if it is in the reticule, hide the indicator
		private void UpdateObject(GameObject go, IFFIndicator indicator) {
			Vector3 deltaPos = go.transform.position - _gameManager.DeviceCamera.transform.position;
			Vector3 localPosition = _gameManager.DeviceCamera.transform.InverseTransformPoint(go.transform.position).normalized;
			float angleRad = Mathf.Atan2(localPosition.y, localPosition.x);

			if (deltaPos != Vector3.zero) {
				Quaternion look = Quaternion.LookRotation(deltaPos, Vector3.up);
				float lookDelta = Quaternion.Angle(_gameManager.DeviceCamera.transform.rotation, look);
				if (lookDelta < _deadZone) {
					indicator.Hide();
				} else {
					indicator.Show();
				}
			} else {
				indicator.Hide();
			}

			Vector2 trig = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
			indicator.Rect.anchoredPosition = trig * _ringRadius;
			indicator.Rect.localEulerAngles = Vector3.forward * angleRad * Mathf.Rad2Deg;
		}
	}
}