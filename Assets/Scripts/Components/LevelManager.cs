// using Sojourn.PicnicIOC;
// using Sojourn.ARDefense.Interfaces;
// using Sojourn.ARDefense.ScriptableObjects;
// using Sojourn.Extensions;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using AOFL.Promises.V1.Core;
// using AOFL.Promises.V1.Interfaces;
// using System.Collections;
// using System.Collections.Generic;

// namespace Sojourn.ARDefense.Components {
// 	public class LevelManager : MonoBehaviour, ILevelManager {
// 		[SerializeField]
// 		private GameObject[] _dropshipPrefabs = null;
// 		[SerializeField]
// 		private float _minSpawnDistance = 2.0f;
// 		[AutoInject]
// 		private IGameManager _gameManager = null;
// 		// [AutoInject]
// 		// private float _maxSpawnDistance = 50.0f;

// 		private void Awake() {
// 			// Container.Register<ILevelManager>(this).AsSingleton();
// 		}

// 		private void Start() {
// 			Container.AutoInject(this);
// 		}

// 		private void StartGame() {
// 			_gameManager.Player1Base.OnBaseKilled += OnBaseKilled;
// 			StartCoroutine(SpawnDropships(100));
// 		}

// 		private IEnumerator SpawnDropships(int count) {
// 			for (int i = 0; i < count; i++) {
// 				SpawnDropship();
// 				yield return new WaitForSeconds(30.0f);
// 			}
// 		}

// 		private void OnBaseKilled(Base b) {
// 			Debug.LogError("Game Over!");
// 		}

// 		private void SpawnDropship() {
// 			GameObject prefab = _dropshipPrefabs[UnityEngine.Random.Range(0, _dropshipPrefabs.Length)];
// 			float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
// #if UNITY_EDITOR
// 			float max = 30.0f;
// #else// UNITY_EDITOR
// 			float max = _gameManager.GroundPlane.size.magnitude;
// #endif// UNITY_EDITOR

// 			float distance = Random.Range(_minSpawnDistance, max);
// 			Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

// 			Vector2 localPos = _gameManager.GroundPlane.centerInPlaneSpace + offset;
// 			Vector2 worldPos = _gameManager.GroundPlane.transform.TransformPoint(localPos);

// 			// Vector3 point =  * 3.0f;
// 			Vector2 offset = new Vector3(UnityEngine.Random.Range(-30.0f, 30.0f), UnityEngine.Random.Range(-30.0f, 30.0f));
// 			float height = Mathf.Max((_gameManager.CameraHeight * 1.25f), 15.0f);
// 			Vector3 point = new Vector3(offset.x, height, offset.y);
// 			Dropship ship = Instantiate(prefab, _gameManager.WorldParent).GetComponent<Dropship>();
// 			ship.transform.position = point;
// 			// #else
// 			//pick a random one and put the dropship there
// 			Vector2 offset = GroundPlane.centerInPlaneSpace + GroundPlane.boundary[UnityEngine.Random.Range(0, GroundPlane.boundary.Length)];
// 			Vector3 point = GroundPlane.transform.TransformPoint(offset);
// 			point.y = (CameraHeight * 1.25f);
// 			Pose p = new Pose(point, Quaternion.identity);
// 			ARReferencePoint anchor = PointManager.AddReferencePoint(p);
// 			Dropship ship = Instantiate(_dropShipPrefab).GetComponent<Dropship>();
// 			// ship.transform.position = Vector3.up * 1.0f;
// 			ship.transform.SetPose(p);
// 			ship.transform.parent = anchor.transform;
// 			// #endif// UNITY_EDITOR
// 		}

// 		private IPromise CreateTestGame() {
// 			// GroundPlane = Instantiate(_planeManager.planePrefab, Vector3.zero, Quaternion.identity).GetComponent<ARPlane>();
// 			Player1Base = Instantiate(_basePrefab, Vector3.zero, Quaternion.identity, WorldParent).GetComponent<Base>();
// 			return Promise.Resolved();
// 		}

// 		//grab the new ones and put them in a hashset
// 		public IPromise StartNewGame() {
// 			return _planeSelector.SelectPlane()
// 			.Then((ARPlane plane) => {
// 				Debug.LogErrorFormat("Plane Selected: {0}", plane);
// 				GroundPlane = plane;
// 			}
// 			)
// 			.Then(() => {
// 				foreach (GameObject go in _debugObjects) {
// 					go.SetActive(false);
// 				}
// 			})
// 			.Chain(PlaceBase)
// 			.Then(delegate (Base b) {
// 				Player1Base = b;
// 				Debug.LogErrorFormat("Base Placed: {0}", b.Transform.position);
// 			})
// 			.Then(() => { Debug.LogError("Setup complete!"); });
// 		}

// 		public void OnPlayerKilled() {
// 			Debug.LogError("Game Over!");
// 		}

// 		public void DEBUG_SetWeapon(int index) {
// 			Weapon weapon = _testWeapons[index];
// 			_player1.CurrentWeapon = _testWeapons[index];
// 			IDisplay display = Instantiate(weapon.DisplayPrefab).GetComponent<IDisplay>();
// 			_displayManager.PushDisplay(display);
// 		}

// 		public IPromise<Base> PlaceBase() {
// 			Debug.LogError("Place Base");
// 			Base b = Instantiate(_basePrefab, Vector3.zero, Quaternion.identity, WorldParent).GetComponent<Base>();

// 			Pose center = new Pose(GroundPlane.center, Quaternion.identity);
// 			b.transform.position = GroundPlane.center;
// 			ARReferencePoint point = PointManager.AttachReferencePoint(GroundPlane, center);
// 			b.transform.parent = point.transform;
// 			return Promise<Base>.Resolved(b);
// 		}
// 		public IPromise<Base> ChoosePlaceBase() {
// 			Debug.LogError("Place Base");
// 			return _objectPlacer.PlaceObjectOnPlane<Base>(_basePrefab, GroundPlane);
// 			// Promise p = new Promise();
// 			// //just put it in the middle for now, later, use a Placer class, same as for turrets
// 			// Instantiate(BasePrefab, Vector3.zero, Quaternion.identity);
// 			// return p;
// 		}

// 		public void RegisterEnemy(GameObject go) {
// 			_enemyList.Add(go);
// 			if (OnEnemyCreated != null) {
// 				OnEnemyCreated(go);
// 			}
// 		}
// 		public void UnregisterEnemy(GameObject go) {
// 			_enemyList.Remove(go);
// 			if (OnEnemyKilled != null) {
// 				OnEnemyKilled(go);
// 			}
// 		}
// 	}
// }