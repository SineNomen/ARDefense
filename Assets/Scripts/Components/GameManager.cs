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

namespace Sojourn.ARDefense.Components {
	public class GameManager : MonoBehaviour, IGameManager {
		[SerializeField]
		private GameObject[] _debugObjects = null;
		[SerializeField]
		private ARCoreSession _arSession = null;
		[SerializeField]
		private Camera _deviceCamera = null;
		[SerializeField]
		private GameObject _basePrefab = null;
		[SerializeField]
		private Weapon[] _testWeapons = null;
		[SerializeField]
		private GameObject _weaponObject = null;

		[AutoInject]
		private IDisplayManager _displayManager = null;
		[AutoInject]
		private IObjectPlacer _objectPlacer = null;
		[AutoInject]
		private IPlayer _player1 = null;
		[AutoInject]
		private IPlaneSelector _planeSelector = null;

		public ARCoreSession ArSession { get => _arSession; }
		public Camera DeviceCamera { get => _deviceCamera; }
		private Reticule Reticule { get => _displayManager.CurrentDisplay.Reticule; }

		public DetectedPlane GroundPlane { get; private set; }

		private void Awake() {
			Container.Register<IGameManager>(this).AsSingleton();
		}

		private void Start() {
			_weaponObject.SetActive(false);
			Container.AutoInject(this);
			//`Mat hack for now
			StartNewGame()
			.Then(() => {
				_weaponObject.SetActive(true);
				DEBUG_SetWeapon(0);
			});
		}

		//grab the new ones and put them in a hashset
		public IPromise StartNewGame() {
			return _planeSelector.SelectPlane()
			.Then((DetectedPlane plane) => {
				Debug.LogErrorFormat("Plane Selected: {0}", plane);
				GroundPlane = plane;
			}
			)
			.Then(() => {
				foreach (GameObject go in _debugObjects) {
					go.SetActive(false);
				}
			})
			.Chain(PlaceBase)
			.Then(delegate (GameObject obj) { Debug.LogErrorFormat("Base Placed: {0}", obj.transform.position); })
			.Then(() => { Debug.LogError("Setup complete!"); });
		}

		public void DEBUG_SetWeapon(int index) {
			Weapon weapon = _testWeapons[index];
			_player1.CurrentWeapon = _testWeapons[index];
			IDisplay display = Instantiate(weapon.DisplayPrefab).GetComponent<IDisplay>();
			_displayManager.PushDisplay(display);
		}


		public IPromise<GameObject> PlaceBase() {
			Debug.LogError("Place Base");
			return _objectPlacer.PlaceObjectOnGround(_basePrefab);
			// Promise p = new Promise();
			// //just put it in the middle for now, later, use a Placer class, same as for turrets
			// Instantiate(BasePrefab, Vector3.zero, Quaternion.identity);
			// return p;
		}
	}
}