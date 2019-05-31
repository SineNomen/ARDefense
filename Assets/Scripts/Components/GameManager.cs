using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.Extensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Sojourn.ARDefense.Components {
	public class GameManager : MonoBehaviour, IGameManager {
		[SerializeField]
		private GameObject[] _debugObjects = null;
		[SerializeField]
		private ARSession _arSession = null;
		[SerializeField]
		private ARReferencePointManager _pointManager = null;
		[SerializeField]
		private ARPlaneManager _planeManager = null;
		[SerializeField]
		private ARRaycastManager _raycastManager = null;
		[SerializeField]
		private ARSessionOrigin _origin = null;

		[SerializeField]
		private Camera _deviceCamera = null;
		[SerializeField]
		private GameObject _basePrefab = null;
		[SerializeField]
		private GameObject _dropShipPrefab = null;
		[SerializeField]
		private Weapon[] _testWeapons = null;
		[SerializeField]
		private GameObject _weaponObject = null;
		[SerializeField]
		private List<GameObject> _enemyList = new List<GameObject>();

		[AutoInject]
		private IDisplayManager _displayManager = null;
		[AutoInject]
		private IObjectPlacer _objectPlacer = null;
		[AutoInject]
		private IPlayer _player1 = null;
		[AutoInject]
		private IPlaneSelector _planeSelector = null;

		public ARSession ArSession { get => _arSession; }
		public Camera DeviceCamera { get => _deviceCamera; }
		private Reticule Reticule { get => _displayManager.CurrentDisplay.Reticule; }
		public Base Player1Base { get; private set; }

		public ARPlane GroundPlane { get; private set; }
		public List<GameObject> EnemyList { get => _enemyList; }

		public GameObjectEvent OnEnemyCreated { get; set; }
		public GameObjectEvent OnEnemyKilled { get; set; }
		public ARRaycastManager RaycastManager { get => _raycastManager; }
		public ARReferencePointManager PointManager { get => _pointManager; }
		public ARPlaneManager PlaneManager { get => _planeManager; }
		public ARSessionOrigin Origin { get => _origin; }
		public Transform WorldParent { get => null; }
		public Vector3 GroundPosition { get => GroundPlane.transform.position; }
		public float CameraHeight { get => DeviceCamera.transform.position.y; }

		private void Awake() {
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
			Container.Register<IGameManager>(this).AsSingleton();
		}

		private void Start() {
			_weaponObject.SetActive(false);
			Container.AutoInject(this);
#if UNITY_EDITOR
			//Only do test in the editor
			CreateTestGame()
			.Chain<float>(this.Wait, 5.0f)
			// StartNewGame()
#else// UNITY_EDITOR
			StartNewGame()
#endif// UNITY_EDITOR
			// StartNewGame()
			.Then(OnGameStart);
		}

		private void OnGameStart() {
			Player1Base.OnBaseKilled += OnBaseKilled;
			_weaponObject.SetActive(true);
			DEBUG_SetWeapon(0);
			StartCoroutine(SpawnDropships(100));
		}

		private IEnumerator SpawnDropships(int count) {
			for (int i = 0; i < count; i++) {
				SpawnDropShip();
				yield return new WaitForSeconds(30.0f);
			}
		}

		private void OnBaseKilled(Base b) {
			Debug.LogError("Game Over!");
		}

		private void SpawnDropShip() {
#if UNITY_EDITOR
			// Vector3 point =  * 3.0f;
			Vector2 offset = new Vector3(UnityEngine.Random.Range(-30.0f, 30.0f), UnityEngine.Random.Range(-30.0f, 30.0f));
			float height = Mathf.Max((CameraHeight * 1.25f), 15.0f);
			Vector3 point = new Vector3(offset.x, height, offset.y);
			Dropship ship = Instantiate(_dropShipPrefab, WorldParent).GetComponent<Dropship>();
			ship.transform.position = point;
#else
			//pick a random one and put the dropship there
			Vector2 offset = GroundPlane.centerInPlaneSpace + GroundPlane.boundary[UnityEngine.Random.Range(0, GroundPlane.boundary.Length)];
			Vector3 point = GroundPlane.transform.TransformPoint(offset);
			point.y = (CameraHeight * 1.25f);
			Pose p = new Pose(point, Quaternion.identity);
			ARReferencePoint anchor = PointManager.AddReferencePoint(p);
			Dropship ship = Instantiate(_dropShipPrefab).GetComponent<Dropship>();
			// ship.transform.position = Vector3.up * 1.0f;
			ship.transform.SetPose(p);
			ship.transform.parent = anchor.transform;
#endif// UNITY_EDITOR
		}

		private IPromise CreateTestGame() {
			// GroundPlane = Instantiate(_planeManager.planePrefab, Vector3.zero, Quaternion.identity).GetComponent<ARPlane>();
			Player1Base = Instantiate(_basePrefab, Vector3.zero, Quaternion.identity, WorldParent).GetComponent<Base>();
			return Promise.Resolved();
		}

		//grab the new ones and put them in a hashset
		public IPromise StartNewGame() {
			return _planeSelector.SelectPlane()
			.Then((ARPlane plane) => {
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
			Base b = Instantiate(_basePrefab, Vector3.zero, Quaternion.identity, WorldParent).GetComponent<Base>();

			Pose center = new Pose(GroundPlane.center, Quaternion.identity);
			b.transform.position = GroundPlane.center;
			ARReferencePoint point = PointManager.AttachReferencePoint(GroundPlane, center);
			b.transform.parent = point.transform;
			return Promise<Base>.Resolved(b);
		}
		public IPromise<Base> ChoosePlaceBase() {
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