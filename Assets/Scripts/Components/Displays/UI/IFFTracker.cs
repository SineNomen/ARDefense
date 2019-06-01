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
		private float _deadZone = 0.0f;

		[AutoInject]
		private IGameManager _gameManager = null;
		private float _ringRadius = 0.0f;

		private Dictionary<GameObject, IFFIndicator> _objectMap = new Dictionary<GameObject, IFFIndicator>();

		private void Awake() {
			_ringRadius = (_ring.rect.size.x + (_indicatorPrefab.transform as RectTransform).rect.size.x) / 2.0f;
		}

		private void Start() {
			Container.AutoInject(this);
			_gameManager.OnEnemyCreated += OnEnemyCreated;
			_gameManager.OnEnemyKilled += OnEnemyKilled;
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
			// Debug.LogFormat("IFFTracker.OnPreShow: {0}, Base: {1}", this.gameObject.name, _gameManager.Player1Base);
			if (_gameManager.Player1Base != null) {
				TrackObject(_gameManager.Player1Base.gameObject, eIFFCategory.Friend);
			}
			foreach (GameObject obj in _gameManager.EnemyList) {
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
			Transform viewport = _gameManager.DeviceCamera.transform;
			Vector2 projectedPos = Vector3.ProjectOnPlane(go.transform.position, viewport.forward);
			float magnitude = projectedPos.magnitude;

			if (magnitude < _deadZone) {
				indicator.Hide();
			} else {
				indicator.Show();
			}

			//shift by 90 degrees to account for Unity's orientation
			float angleRad = -Mathf.Atan2(projectedPos.x, projectedPos.y) + (Mathf.PI / 2.0f);
			Vector2 trig = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
			indicator.Rect.anchoredPosition = trig * _ringRadius;
			indicator.Rect.localEulerAngles = Vector3.forward * angleRad * Mathf.Rad2Deg;
			// Debug.LogFormat("Plane: {0}, ProjectedPos: {1}, Angle: {2}", plane, projectedPos, angleDeg);
			// Debug.LogFormat("{0}[{1}] Magnitude: {2}", go.name, projectedPos, magnitude);
		}
	}
}