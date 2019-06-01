using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

namespace Sojourn.ARDefense.Components {
	//Place an object on a plane, object is visible as a preview
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

		private void Awake() {
			Container.Register<IObjectPlacer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise<GameObject> PlaceObjectOnPlane(GameObject prefab, ARPlane plane = null) {
			_prefab = prefab;
			PlacerDisplay display = Instantiate(_displayPrefab).GetComponent<PlacerDisplay>();
			display.PlaceButton.onClick.AddListener(OnPlaceButton);

			_displayManager.PushDisplay(display);

			_placeCoroutine = StartCoroutine(MovePlaceObject(plane));

			_placePromise = new Promise<GameObject>();
			return _placePromise;
		}

		public IPromise<T> PlaceObjectOnPlane<T>(GameObject prefab, ARPlane plane = null) where T : Component {
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

		private IEnumerator MovePlaceObject(ARPlane plane) {
			yield return null;
			if (plane == null) { plane = _gameManager.GroundPlane; }
			ARReferencePoint point = null;
			while (true) {
				ARReferencePoint newPoint = PlaceObjectOnGround(plane);
				if (newPoint != null) {
					if (point != null) { Destroy(point.gameObject); }
					point = newPoint;
				}
				yield return null;
			}
		}
		private ARReferencePoint PlaceObjectOnGround(ARPlane plane) {
			// Vector2 reticulePos = _displayManager.CurrentDisplay.Reticule.ScreenPosition;
			Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
			List<ARRaycastHit> results = new List<ARRaycastHit>();
			if (_gameManager.RaycastManager.Raycast(reticulePos, results, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) {
				// Use hit pose and camera pose to check if hittest is from the
				// back of the plane, if it is, no need to create the anchor.
				foreach (ARRaycastHit hit in results) {
					Debug.LogFormat("Hit: {0}", hit.trackableId);
					if (hit.trackableId == plane.trackableId &&
						Vector3.Dot(_gameManager.DeviceCamera.transform.position - hit.pose.position,
						hit.pose.rotation * Vector3.up) < 0) {
					} else {
						Debug.LogFormat("Moving object to new position: {0}", hit.pose.position);

						if (_placeObject == null) {
							_placeObject = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
						}
						_placeObject.transform.SetPose(hit.pose);
						ARReferencePoint point = _gameManager.PointManager.AttachReferencePoint(plane, hit.pose);
						_placeObject.transform.parent = point.transform;
						return point;
					}
				}
			} else {
				Debug.Log("Not looking at the ground plane");
			}
			return null;
		}
	}
}