using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;

public class GameManager : MonoBehaviour, IGameManager {
	[SerializeField]
	private GameObject[] _debugObjects = null;
	[SerializeField]
	private ARCoreSession _arSession = null;
	[SerializeField]
	private Camera _deviceCamera = null;
	[Tooltip("How big the ground plane must be for us to use it")]
	[SerializeField]
	private float _groundAreaThreshold = 3.0f;
	[SerializeField]
	private GameObject DetectedPlanePrefab = null;
	[SerializeField]
	private GameObject BasePrefab = null;
	[SerializeField]
	private CanvasGroup _groundSetupUI = null;

	[AutoInject]
	private IDisplayManager _displayManager = null;
	[AutoInject]
	private IObjectPlacer _objectPlacer = null;

	public float MinGroundArea { get => _groundAreaThreshold; }
	public DetectedPlane GroundPlane { get; private set; }
	public DetectedPlaneVisualizer GroundPlaneVisualizer { get; private set; }

	public ARCoreSession ArSession { get => _arSession; }
	public Camera DeviceCamera { get => _deviceCamera; }
	private Reticule Reticule { get => _displayManager.CurrentDisplay.Reticule; }

	private void Awake() {
		Container.Register<IGameManager>(this).AsSingleton();
	}

	private void Start() {
		Container.AutoInject(this);
		//`Mat hack for now
		StartNewGame();
	}

	//grab the new ones and put them in a hashset
	public IPromise StartNewGame() {
		return _displayManager.PushDefault()
		.Chain(SetupGround)
		.Then(() => {
			foreach (GameObject go in _debugObjects) {
				go.SetActive(false);
			}
		})
		.Chain(PlaceBase)
		.Then(delegate (GameObject obj) { Debug.LogErrorFormat("Base Placed: {0}", obj.transform.position); })
		.Then(() => { Debug.LogError("Setup complete!"); });
	}

	public IPromise<GameObject> PlaceBase() {
		Debug.LogError("Place Base");
		return _objectPlacer.PlaceObjectOnGround(BasePrefab);
		// Promise p = new Promise();
		// //just put it in the middle for now, later, use a Placer class, same as for turrets
		// Instantiate(BasePrefab, Vector3.zero, Quaternion.identity);
		// return p;
	}

	public IPromise SetupGround() {
		Promise p = new Promise();
		StartCoroutine(FindGround(p));
		return p;
	}

	private IEnumerator FindGround(IPromise promise) {
		List<DetectedPlane> planes = new List<DetectedPlane>();
		HashSet<DetectedPlane> planeSet = new HashSet<DetectedPlane>();
		_groundSetupUI.Show(0.25f);
		Reticule.SetReload(0.0f);
		while (GroundPlane == null || GroundPlane.Area < _groundAreaThreshold) {
			Session.GetTrackables<DetectedPlane>(planes, TrackableQueryFilter.New);
			foreach (DetectedPlane p in planes) { planeSet.Add(p); }

			foreach (DetectedPlane plane in planeSet) {
				if (GroundPlane == null || GroundPlane.Area < plane.Area) {
					Debug.LogFormat("Switching to new Plane with Area {0}", plane.Area);
					if (GroundPlaneVisualizer != null) {
						Destroy(GroundPlaneVisualizer.gameObject);
					}
					GroundPlane = plane;

					GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
					GroundPlaneVisualizer = planeObject.GetComponent<DetectedPlaneVisualizer>();
					GroundPlaneVisualizer.Initialize(plane);
				} else {
					//after some time, tell the user to move more so as to geta bigger plane
				}

				float percent = GroundPlane.Area / _groundAreaThreshold;
				Reticule.SetReload(percent);
			}

			yield return null;
			if (GroundPlane != null) {
				Debug.LogFormat("Plane Area: {0}", GroundPlane.Area);
			}
		}
		Debug.LogErrorFormat("Found Ground, area: {0}", GroundPlane.Area);
		Utility.PromiseGroup(
			_groundSetupUI.Hide(0.25f),
			Reticule.StartReload()
		)
		.Then(promise.Resolve);
	}
}
