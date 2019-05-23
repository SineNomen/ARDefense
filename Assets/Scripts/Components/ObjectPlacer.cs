using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;


namespace Sojourn.ARDefense.Components {
	public class ObjectPlacer : MonoBehaviour, IObjectPlacer {
		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;

		[SerializeField]
		private GameObject _displayPrefab = null;

		private GameObject _prefab = null;
		private GameObject _placeObject = null;
		private IPromise<GameObject> _placePromise = null;
		private Coroutine _placeCoroutine = null;
		private Anchor _lastAnchor = null;

		private void Awake() {
			Container.Register<IObjectPlacer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise<GameObject> PlaceObjectOnPlane(GameObject prefab, DetectedPlane plane = null) {
			_prefab = prefab;
			PlacerDisplay display = Instantiate(_displayPrefab).GetComponent<PlacerDisplay>();
			display.PlaceButton.onClick.AddListener(OnPlaceButton);

			_displayManager.PushDisplay(display);

			_placeCoroutine = StartCoroutine(MovePlaceObject(plane));

			_placePromise = new Promise<GameObject>();
			return _placePromise;
		}

		public IPromise<T> PlaceObjectOnPlane<T>(GameObject prefab, DetectedPlane plane = null) where T : Component {
			IPromise<T> p = new Promise<T>();
			PlaceObjectOnPlane(prefab, plane).Then((GameObject go) => {
				p.Resolve(go.GetComponent<T>());
			});
			return p;
		}

		private void OnPlaceButton() {
			if (_placeObject != null && _placePromise == null) { return; }
			Debug.LogFormat("Placed Object!");
			StopCoroutine(_placeCoroutine);
			_displayManager.PopDisplay()
			.Then<GameObject>(_placePromise.Resolve, _placeObject);
			_placePromise = null;

		}

		private IEnumerator MovePlaceObject(DetectedPlane plane) {
			yield return null;
			if (plane == null) { plane = _gameManager.GroundPlane; }
			Anchor anchor = null;
			while (true) {
				Anchor newAnchor = PlaceObjectOnGround(plane);
				if (newAnchor != null) {
					if (anchor != null) { Destroy(anchor.gameObject); }
					anchor = newAnchor;
				}
				yield return null;
			}
		}
		private Anchor PlaceObjectOnGround(DetectedPlane plane) {
			//get the screen position of the reticule
			// Vector2 reticulePos = _displayManager.CurrentDisplay.Reticule.ScreenPosition;
			Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
			TrackableHit hit;
			// TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
			TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinInfinity;
			if (Frame.Raycast(reticulePos.x, reticulePos.y, raycastFilter, out hit)) {
				// Use hit pose and camera pose to check if hittest is from the
				// back of the plane, if it is, no need to create the anchor.
				Debug.LogFormat("Hit: {0}", hit.Trackable);
				if (hit.Trackable == plane &&
					Vector3.Dot(_gameManager.DeviceCamera.transform.position - hit.Pose.position,
					hit.Pose.rotation * Vector3.up) < 0) {
				} else {
					Debug.LogFormat("Moving object to new position: {0}", hit.Pose.position);

					if (_placeObject == null) {
						_placeObject = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
					}
					_placeObject.transform.SetPose(hit.Pose);
					Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
					_placeObject.transform.parent = anchor.transform;
					return anchor;
				}
			} else {
				Debug.Log("Not looking at the ground plane");
			}
			return null;
		}
	}
}