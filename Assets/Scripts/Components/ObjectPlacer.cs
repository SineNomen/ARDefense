using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;

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
		Container.Inject(this);
	}

	public IPromise<GameObject> PlaceObjectOnGround(GameObject prefab) {
		_prefab = prefab;
		PlacerDisplay display = Instantiate(_displayPrefab).GetComponent<PlacerDisplay>();
		display.PlaceButton.onClick.AddListener(OnPlaceButton);

		_displayManager.PushDisplay(display);

		_placeCoroutine = StartCoroutine(MovePlaceObject());

		_placePromise = new Promise<GameObject>();
		return _placePromise;
	}

	public IPromise<T> PlaceObjectOnGround<T>(GameObject prefab) where T : Component {
		IPromise<T> p = new Promise<T>();
		PlaceObjectOnGround(prefab).Then((GameObject go) => {
			p.Resolve(go.GetComponent<T>());
		});
		return p;
	}

	private void OnPlaceButton() {
		if (_placeObject != null && _placePromise == null) { return; }
		Debug.LogFormat("Placed Object!");
		StopCoroutine(_placeCoroutine);
		PlaceObjectOnGround();
		_displayManager.PopDisplay()
		.Then<GameObject>(_placePromise.Resolve, _placeObject);
		_placePromise = null;

	}

	private IEnumerator MovePlaceObject() {
		yield return null;
		while (true) {
			PlaceObjectOnGround();
			yield return null;
		}
	}
	private void PlaceObjectOnGround() {
		//get the screen position of the reticule
		// Vector2 reticulePos = _displayManager.CurrentDisplay.Reticule.ScreenPosition;
		Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
		TrackableHit hit;
		TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
		Debug.LogFormat("Doing raycast, pos: {0}", reticulePos);
		if (Frame.Raycast(reticulePos.x, reticulePos.y, raycastFilter, out hit)) {
			// Use hit pose and camera pose to check if hittest is from the
			// back of the plane, if it is, no need to create the anchor.
			Debug.LogFormat("Hit: {0}", hit.Trackable);
			if (hit.Trackable == _gameManager.GroundPlane &&
				Vector3.Dot(_gameManager.DeviceCamera.transform.position - hit.Pose.position,
				hit.Pose.rotation * Vector3.up) < 0) {
				Debug.Log("Hit at back of the current DetectedPlane");
			} else {
				Debug.LogFormat("Moving object to new position: {0}", hit.Pose.position);

				if (_placeObject == null) {
					_placeObject = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
				}
				if (_lastAnchor != null) {
					Debug.Log("Destroy Anchor");
					Destroy(_lastAnchor);
				}
				_placeObject.transform.SetPose(hit.Pose);
				Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
				_placeObject.transform.parent = anchor.transform;
				_lastAnchor = anchor;
			}
		} else {
			Debug.Log("Not looking at the ground plane");
		}
	}
}
