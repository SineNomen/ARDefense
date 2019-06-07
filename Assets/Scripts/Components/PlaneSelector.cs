using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

namespace Sojourn.ARDefense.Components {
	//used to choose the plane to act as the game's ground, can automatically pick the one looked at if it's big enough
	public class PlaneSelector : MonoBehaviour, IPlaneSelector {
		[Tooltip("How big the ground plane must be for us to use it")]
		[SerializeField]
		private float _groundAreaThreshold = 3.0f;
		[SerializeField]
		private bool _autoPick = false;
		[SerializeField]
		private GameObject _displayPrefab = null;
		[SerializeField]
		private CanvasGroup _setupUI = null;
		[SerializeField]
		private CanvasGroup _tooSmallUI = null;

		public float MinGroundArea { get => _groundAreaThreshold; }
		public ARPlane BestPlane { get; private set; }

		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;

		private bool _chooseButtonPressed = false;
		private bool _cancelButtonPressed = false;
		private HashSet<ARPlane> _planes = new HashSet<ARPlane>();

		private void Awake() {
			Container.Register<IPlaneSelector>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise<ARPlane> SelectPlane() {
			PlaneSelectorDisplay display = Instantiate(_displayPrefab).GetComponent<PlaneSelectorDisplay>();
			if (_autoPick) {
				display.ChooseButton.gameObject.SetActive(false);
			}
			display.OnChooseButton += OnChoose;
			display.OnCancelButton += OnCancel;
			_displayManager.PushDisplay(display);

			_chooseButtonPressed = false;
			_cancelButtonPressed = false;

			_gameManager.PlaneManager.enabled = true;
			_gameManager.PlaneManager.planesChanged += OnPlanesChanged;

			return this.StartCoroutineAsPromise<ARPlane>(PickPlane()).Then((ARPlane p) => {
				_gameManager.PlaneManager.planesChanged -= OnPlanesChanged;
				_gameManager.PlaneManager.enabled = false;
				TurnOffPlanes(p);
			});
		}

		private void TurnOffPlanes(ARPlane chosen) {
			VisualPlane.DestroyAll();
		}

		private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
			foreach (ARPlane p in args.added) { _planes.Add(p); }
			foreach (ARPlane p in args.updated) { _planes.Add(p); }
		}

		//when the user is pointing at a plane that is big enough, they need to pick it (or we auto-pick)
		private IEnumerator<ARPlane> PickPlane() {
			// Check that motion tracking is tracking.
			_displayManager.CurrentDisplay.Reticule.SetReload(0.0f);

			_setupUI.Show(0.25f);
			while (!_cancelButtonPressed) {
				if (ARSession.state != ARSessionState.SessionTracking) {
					yield return null;
				}
				bool planeOK = false;

				//see which one we are looking at
				ARPlane selectedPlane = null;
				Vector2 reticulePos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
				List<ARRaycastHit> results = new List<ARRaycastHit>();
				if (_gameManager.RaycastManager.Raycast(reticulePos, results, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) {
					foreach (ARRaycastHit hit in results) {
						ARPlane plane = _gameManager.PlaneManager.GetPlane(hit.trackableId);
						if (plane != null) {
							//need to also check if it is big enough
							selectedPlane = plane;
						}
					}
				}
				if (selectedPlane != null) {
					_displayManager.CurrentDisplay.Reticule.SetReload(selectedPlane.size.magnitude / _groundAreaThreshold);
					if (selectedPlane.size.magnitude > _groundAreaThreshold) {
						_displayManager.CurrentDisplay.Reticule.ShowHighlight();
						planeOK = true;
						// Debug.LogErrorFormat("Good plane: {0}", selectedPlane);
					}
				} else {
					_displayManager.CurrentDisplay.Reticule.Unload();
					_displayManager.CurrentDisplay.Reticule.HideHighlight();
				}

				if (planeOK && (_chooseButtonPressed || _autoPick)) {
					yield return selectedPlane;
					EndPicking(selectedPlane);
					yield break;
				}
				//if we got this far the plane was no good
				if (_chooseButtonPressed) {
					//show that we need a bigger plane
					_tooSmallUI.Show(0.25f)
					.Chain<float>(this.Wait, 5.0f)
					.Chain<float>(_tooSmallUI.Hide, 0.25f);
				}
				if (_cancelButtonPressed) {
					yield return null;
					EndPicking(null);
					yield break;
				}
				_chooseButtonPressed = false;
				yield return null;
			}
		}

		private void EndPicking(ARPlane plane) {
			_setupUI.Hide(0.25f);
			_planes.Clear();
		}


		private void OnChoose() { _chooseButtonPressed = true; }
		private void OnCancel() { _cancelButtonPressed = true; }
	}
}



























