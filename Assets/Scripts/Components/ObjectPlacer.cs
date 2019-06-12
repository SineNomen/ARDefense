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
		[SerializeField]
		private GameObject _displayPrefab = null;

		private GameObject _prefab = null;
		private GameObject _placeObject = null;
		private IPromise<GameObject> _placePromise = null;
		private Coroutine _placeCoroutine = null;


		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;
		[AutoInject]
		private ILevelManager _levelManager = null;

		private void Awake() {
			Container.Register<IObjectPlacer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise<GameObject> PlaceObject(GameObject prefab, Ground ground) {
			_prefab = prefab;
			PlacerDisplay display = Instantiate(_displayPrefab).GetComponent<PlacerDisplay>();
			display.PlaceButton.onClick.AddListener(OnPlaceButton);

			_displayManager.PushDisplay(display);

			_placeCoroutine = StartCoroutine(MovePlaceObject(ground));

			_placePromise = new Promise<GameObject>();
			return _placePromise;
		}

		public IPromise<T> PlaceObject<T>(GameObject prefab, Ground ground) where T : Component {
			IPromise<T> p = new Promise<T>();
			PlaceObject(prefab, ground).Then((GameObject go) => {
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

		private IEnumerator MovePlaceObject(Ground ground) {
			yield return null;
			if (ground.Plane == null) { ground.Plane = _levelManager.GroundPlane; }
			ARReferencePoint point = null;
			while (true) {
				ARReferencePoint newPoint = PlaceObjectOnGround(ground);
				if (newPoint != null) {
					if (point != null) { Destroy(point.gameObject); }
					point = newPoint;
				}
				yield return null;
			}
		}
		private ARReferencePoint PlaceObjectOnGround(Ground ground) {
			Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
			List<ARRaycastHit> results = new List<ARRaycastHit>();
			Transform camera = _gameManager.DeviceCamera.transform;
			Ray ray = _gameManager.DeviceCamera.ScreenPointToRay(reticulePos);
			RaycastHit hit;
			int mask = LayerMask.GetMask("Ground");
			if (Physics.Raycast(camera.position, camera.forward, out hit, Mathf.Infinity, mask)) {
				// if (_gameManager.RaycastManager.Raycast(reticulePos, results, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) {
				if (hit.transform == ground.transform) {
					Pose pose = new Pose(hit.point, Quaternion.identity);
					// Debug.LogFormat("Moving object to new position: {0}", pose.position);

					if (_placeObject == null) {
						_placeObject = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
					}
					_placeObject.transform.SetPose(pose);
					if (ground.Plane != null) {
						ARReferencePoint point = _gameManager.PointManager.AttachReferencePoint(ground.Plane, pose);
						_placeObject.transform.parent = point.transform;
						return point;
					}
				}
			}
			return null;
		}
	}
}