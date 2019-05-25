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
using System;

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
		[SerializeField]
		private GameObject _testSpawner = null;
		// private GameObject[] _testSpawners = null;
		private List<GameObject> _enemyList = new List<GameObject>();

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
		public Base Player1Base { get; private set; }

		public DetectedPlane GroundPlane { get; private set; }
		public List<GameObject> EnemyList { get => _enemyList; }

		public GameObjectEvent OnEnemyCreated { get; set; }
		public GameObjectEvent OnEnemyKilled { get; set; }

		private void Awake() {
			Container.Register<IGameManager>(this).AsSingleton();
		}

		private void Start() {
			_weaponObject.SetActive(false);
			Container.AutoInject(this);
#if UNITY_EDITOR
			//Only do test in the editor
			// CreateTestGame()
			StartNewGame()
#else// UNITY_EDITOR
			StartNewGame()
#endif// UNITY_EDITOR
			// StartNewGame()
			.Then(() => {
				_weaponObject.SetActive(true);
				DEBUG_SetWeapon(0);
				_testSpawner.transform.SetParent(Player1Base.Transform);
				_testSpawner.transform.localPosition = Vector3.zero;
				// _testSpawner.SetActive(true);
				// SetupSpawners();
			});
		}

		// private void SetupSpawners() {
		// 	foreach (Spawner sp in _testSpawners) {
		// 		Instantiate (Spawner)
		// 	}
		// }

		private IPromise CreateTestGame() {
			GroundPlane = null;
			Player1Base = Instantiate(_basePrefab, new Vector3(0.0f, -2.0f, 15.0f), Quaternion.identity).GetComponent<Base>();
			return Promise.Resolved();
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
			.Then(delegate (Base b) {
				Player1Base = b;
				Debug.LogErrorFormat("Base Placed: {0}", b.Transform.position);
			})
			.Then(() => { Debug.LogError("Setup complete!"); });
		}

		public void OnPlayerKilled() {
			Debug.LogError("Game Over!");
		}

		public void DEBUG_SetWeapon(int index) {
			Weapon weapon = _testWeapons[index];
			_player1.CurrentWeapon = _testWeapons[index];
			IDisplay display = Instantiate(weapon.DisplayPrefab).GetComponent<IDisplay>();
			_displayManager.PushDisplay(display);
		}

		public IPromise<Base> PlaceBase() {
			Debug.LogError("Place Base");
			return _objectPlacer.PlaceObjectOnPlane<Base>(_basePrefab, GroundPlane);
			// Promise p = new Promise();
			// //just put it in the middle for now, later, use a Placer class, same as for turrets
			// Instantiate(BasePrefab, Vector3.zero, Quaternion.identity);
			// return p;
		}

		public void RegisterEnemy(GameObject go) {
			_enemyList.Add(go);
			if (OnEnemyCreated != null) {
				OnEnemyCreated(go);
			}
		}
		public void UnregisterEnemy(GameObject go) {
			_enemyList.Remove(go);
			if (OnEnemyKilled != null) {
				OnEnemyKilled(go);
			}
		}
	}
}