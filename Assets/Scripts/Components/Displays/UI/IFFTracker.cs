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

namespace Sojourn.ARDefense.Components {
	public class IFFTracker : MonoBehaviour {
		[SerializeField]
		private GameObject _indicatorPrefab = null;
		[SerializeField]
		private RectTransform _ring = null;
		[SerializeField]
		private RectTransform _indicatorParent = null;
		[SerializeField]
		private Transform _testPlaneTransform = null;
		[SerializeField]
		[Range(0.0f, 360.0f)]
		private float _angle = 0.0f;
		[SerializeField]
		[Tooltip("Objects within the deadzone do not show and indicator")]
		private float _deadZone = 0.0f;
		// [SerializeField]
		// [Tooltip("Whether closer objects should have a larger indicator")]
		// private bool _scaleForDistance = false;

		[AutoInject]
		private IGameManager _gameManager = null;
		private float _ringRadius = 0.0f;

		private Dictionary<GameObject, IFFIndicator> _objectMap = new Dictionary<GameObject, IFFIndicator>();

		private void Awake() {
			_ringRadius = (_ring.rect.size.x + (_indicatorPrefab.transform as RectTransform).rect.size.x) / 2.0f;
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise TrackObject(GameObject go, eIFFCategory cat) {
			IFFIndicator indicator = Instantiate(_indicatorPrefab, _indicatorParent).GetComponent<IFFIndicator>();
			indicator.SetUp(cat);
			_objectMap[go] = indicator;
			return indicator.Show();
		}

		private void Update() {
			foreach (KeyValuePair<GameObject, IFFIndicator> pair in _objectMap) {
				UpdateObject(pair.Key, pair.Value);
			}
		}

		//set the new position, if it is in the reticule, hide the indicator
		private void UpdateObject(GameObject go, IFFIndicator indicator) {
			Transform viewport = _gameManager.DeviceCamera.transform;
			Vector3 projectedPos = Vector3.ProjectOnPlane(go.transform.position, viewport.forward);
			float magnitude = projectedPos.magnitude;
			projectedPos.Normalize();
			// if (_scaleForDistance) {
			// indicator.SetScale()
			// }

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
			Debug.LogFormat("Magnitude: {0}", magnitude);
		}
	}
}