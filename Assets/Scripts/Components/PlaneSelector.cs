using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;
using System.Linq;

namespace Sojourn.ARDefense.Components {
	public class PlaneSelector : MonoBehaviour, IPlaneSelector {
		[Tooltip("How big the ground plane must be for us to use it")]
		[SerializeField]
		private float _groundAreaThreshold = 3.0f;
		[SerializeField]
		private GameObject _detectedPlanePrefab = null;
		[SerializeField]
		private GameObject _displayPrefab = null;
		[SerializeField]
		private CanvasGroup _setupUI = null;
		[SerializeField]
		private CanvasGroup _tooSmallUI = null;

		public float MinGroundArea { get => _groundAreaThreshold; }
		public DetectedPlane BestPlane { get; private set; }
		public DetectedPlaneVisualizer Visualizer { get; private set; }

		[AutoInject]
		private IDisplayManager _displayManager = null;

		private bool _chooseButtonPressed = false;
		private bool _cancelButtonPressed = false;
		private List<DetectedPlane> _planes = new List<DetectedPlane>();

		private void Awake() {
			Container.Register<IPlaneSelector>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise<DetectedPlane> SelectPlane() {
			PlaneSelectorDisplay display = Instantiate(_displayPrefab).GetComponent<PlaneSelectorDisplay>();
			display.OnChooseButton += OnChoose;
			display.OnCancelButton += OnCancel;
			_displayManager.PushDisplay(display);

			_chooseButtonPressed = false;
			_cancelButtonPressed = false;

			return this.StartCoroutineAsPromise<DetectedPlane>(PickPlane());
		}

		private IEnumerator<DetectedPlane> PickPlane() {
			List<DetectedPlane> newPlanes = new List<DetectedPlane>();
			// Check that motion tracking is tracking.
			_displayManager.CurrentDisplay.Reticule.SetReload(0.0f);

			_setupUI.Show(0.25f);
			while (!_cancelButtonPressed) {
				if (Session.Status != SessionStatus.Tracking) {
					yield return null;
				}

				// Iterate over planes found in this frame and instantiate corresponding GameObjects to
				// visualize them.
				Session.GetTrackables<DetectedPlane>(newPlanes, TrackableQueryFilter.New);
				for (int i = 0; i < newPlanes.Count; i++) {
					DetectedPlane plane = newPlanes[i];
					_planes.Add(plane);
					plane.Visualizer = Instantiate(_detectedPlanePrefab,
													Vector3.zero,
													Quaternion.identity,
													transform).GetComponent<DetectedPlaneVisualizer>();
					plane.Visualizer.Initialize(plane);
				}
				//get the in order of biggest to smallest
				_planes.OrderBy(plane => float.MaxValue - plane.Area);
				for (int i = 0; i < _planes.Count; i++) {
					DetectedPlane plane = _planes[i];
					if (i == 0) {
						BestPlane = plane;
						plane.Visualizer.SetColor(Color.yellow);
					} else {
						plane.Visualizer.SetColor(Color.white);
					}
				}

				//see which one we are looking at
				Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
				TrackableHit hit;
				TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
				DetectedPlaneVisualizer visualizer = null;
				DetectedPlane selectedPlane = null;
				if (Frame.Raycast(reticulePos.x, reticulePos.y, raycastFilter, out hit)) {
					// Use hit pose and camera pose to check if hittest is from the
					// back of the plane, if it is, no need to create the anchor.
					// Debug.LogFormat("Hit: {0}", hit.Trackable);
					if (hit.Trackable is DetectedPlane) {
						DetectedPlane plane = hit.Trackable as DetectedPlane;
						//need to also check if it is big enough
						selectedPlane = plane;
						visualizer = plane.Visualizer;
					}
				}
				if (visualizer != null) {
					if (selectedPlane.Area > _groundAreaThreshold) {
						_displayManager.CurrentDisplay.Reticule.ShowHighlight();
					}
					_displayManager.CurrentDisplay.Reticule.SetReload(selectedPlane.Area / _groundAreaThreshold);
					visualizer.SetColor(Color.green);
				} else {
					_displayManager.CurrentDisplay.Reticule.Unload();
					_displayManager.CurrentDisplay.Reticule.HideHighlight();
				}

				if (_chooseButtonPressed && selectedPlane != null) {
					if (selectedPlane.Area >= _groundAreaThreshold) {
						yield return selectedPlane;
						EndPicking(selectedPlane);
						yield break;
					} else {
						//show that we need a bigger plane
						_tooSmallUI.Show(0.25f)
						.Chain<float>(this.Wait, 5.0f)
						.Chain<float>(_tooSmallUI.Hide, 0.25f);
					}
				}
				_chooseButtonPressed = false;
				yield return null;
			}
			if (_cancelButtonPressed) {
				yield return null;
				EndPicking(null);
				yield break;
			}
		}

		private void EndPicking(DetectedPlane plane) {
			_setupUI.Hide(0.25f);
			plane.Visualizer.SetColor(Color.gray);
			foreach (DetectedPlane p in _planes) {
				if (p != plane && p.Visualizer != null) {
					Destroy(p.Visualizer.gameObject);
				}
			}
			_planes.Clear();
		}


		private void OnChoose() { _chooseButtonPressed = true; }
		private void OnCancel() { _cancelButtonPressed = true; }
	}
}



























