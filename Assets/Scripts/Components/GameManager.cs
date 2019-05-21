using Sojourn.PicnicIOC;
using Extensions;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;

public class GameManager : MonoBehaviour, IGameManager {
	[Tooltip("How big the ground plane must be for us to use it")]
	[SerializeField]
	private float _groundAreaThreshold = 3.0f;
	[SerializeField]
	private GameObject DetectedPlanePrefab = null;
	[SerializeField]
	private Reticule _reticule = null;
	[SerializeField]
	private CanvasGroup _groundSetupUI = null;

	public float MinGroundArea { get { return _groundAreaThreshold; } }
	public DetectedPlane GroundPlane { get; private set; }
	public DetectedPlaneVisualizer GroundPlaneVisualizer { get; private set; }

	private void Start() {
		Container.Register<IGameManager>(this);
		StartNewGame().Then(() => { Debug.LogErrorFormat("Found the ground, area: {0}", GroundPlane.Area); });
	}


	//grab the new ones and put them in a hashset

	public IPromise StartNewGame() {
		return SetupGround()
		.Chain(PlaceBase);
	}

	public IPromise PlaceBase() {
		Promise p = new Promise();
		//just put it in the middle for now
		return p;
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
		_reticule.SetReload(0.0f);
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
				_reticule.SetReload(percent);
			}

			yield return null;
		}
		// Debug.LogErrorFormat("Found Ground, area: {0}", GroundPlane.Area);
		_groundSetupUI.Hide(0.25f)
		.Then(promise.Resolve);
	}
}
