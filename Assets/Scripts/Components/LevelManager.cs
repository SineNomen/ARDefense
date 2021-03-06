﻿using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using Sojourn.Interfaces;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.ARDefense.Data;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	//mananges spawning, scoring and game over
	public class LevelManager : MonoBehaviour, ILevelManager {
		[SerializeField]
		private GameObject _groundPrefab = null;
		[SerializeField]
		private GameObject _basePrefab = null;
		[SerializeField]
		private GameObject[] _dropshipPrefabs = null;
		[SerializeField]
		private bool _spawnDropshipsRandomly = false;
		[SerializeField]
		private RandomFloat _spawnRadius = new RandomFloat(0.5f, 1.0f, 0.0f, 1.0f);
		[SerializeField]
		private RandomFloat _spawnHeightScale = new RandomFloat(-0.8f, 1.25f, 0.5f, 3.0f);
		[SerializeField]
		private RandomFloat _dropshipWaitTime = new RandomFloat(20.0f, 40.0f);
		[SerializeField]
		private bool _playerPlacesBase = false;

		public Base PlayerBase { get; private set; }
		public ARPlane GroundPlane { get; private set; }
		public Ground Ground { get; private set; }
		public List<GameObject> EnemyList { get => _enemyList; }
		public GameObjectEvent OnEnemyCreated { get; set; }
		public GameObjectEvent OnEnemyKilled { get; set; }
		public LevelEvent OnLevelStarted { get; set; }
		public LevelEvent OnLevelEnded { get; set; }
		public int CurrentScore { get => _currentScore; }

		private List<GameObject> _enemyList = new List<GameObject>();


		private const int _dropshipScore = 50;
		private const int _tankScore = 10;
		private const int _fighterScore = 20;
		private int _currentScore = 0;

		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;
		[AutoInject]
		private IObjectPlacer _objectPlacer = null;
		[AutoInject]
		private IPlaneSelector _planeSelector = null;
		[AutoInject]
		private IPersistentDataManager _saveDataManager = null;
		[AutoInject]
		private IPlayerHUD _playerHUD = null;
		[AutoInject]
		private IMainMenu _mainMenu = null;

		private int _dropshipIndex = 0;

		private void Awake() {
			Container.Register<ILevelManager>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise SetupLevel() {
#if UNITY_EDITOR
			//Only do test in the editor
			CreateTestLevel();
			// return this.Wait(5.0f)
			return Promise.Resolved()
#else// UNITY_EDITOR
			return CreateBasicLevel()
#endif// UNITY_EDITOR
			.Chain(SetupBase);
		}

		public void StartLevel() {
			_playerHUD.Show();
			PlayerBase.OnBaseKilled += OnBaseKilled;
			StartCoroutine(SpawnDropships(100));

			_currentScore = 0;
			if (OnLevelStarted != null) { OnLevelStarted(); }
		}

		private void CreateTestLevel() {
			Ground = Instantiate(_groundPrefab, Vector3.zero, Quaternion.identity, _gameManager.WorldParent).GetComponent<Ground>();
			Ground.Radius = 30.0f;
		}

		public IPromise CreateBasicLevel() {
			return _planeSelector.SelectPlane()
			.Then((ARPlane plane) => {
				Debug.LogFormat("Plane Selected: {0}", plane);
				GroundPlane = plane;
				Ground = Instantiate(_groundPrefab, Vector3.zero, Quaternion.identity, _gameManager.WorldParent).GetComponent<Ground>();
				Ground.Radius = Mathf.Sqrt(plane.extents.magnitude) * _gameManager.OriginScale;
				Debug.LogFormat("Creating Ground, Radius: {0}, Plane Size: {1}, Plane Extents: {2}", Ground.Radius, plane.size, plane.extents);
				// Ground.Radius = Mathf.Min(plane.size.x, plane.size.y) * _gameManager.OriginScale;
				Pose pose = new Pose(plane.center, Quaternion.identity);
				ARReferencePoint anchor = _gameManager.PointManager.AttachReferencePoint(plane, pose);
				Ground.transform.SetPose(pose);
				Ground.transform.SetParent(anchor.transform);
			});
		}

		private IPromise SetupBase() {
			return (_playerPlacesBase ? ChoosePlaceBase() : PlaceBase())
			.Then(delegate (Base b) {
				Debug.Log(b);
				Debug.LogFormat("Base Placed: {0}", b.Transform.position);
				PlayerBase = b;
			});
		}

		//pu the base in the center
		public IPromise<Base> PlaceBase() {
			Base b = Instantiate(_basePrefab, Ground.Center, Quaternion.identity, _gameManager.WorldParent).GetComponent<Base>();
			return Promise<Base>.Resolved(b);
		}

		//use the object placer to let the user put it where they want
		public IPromise<Base> ChoosePlaceBase() {
			Debug.Log("Place Base");
			return _objectPlacer.PlaceObject<Base>(_basePrefab, Ground);
		}
		private IEnumerator SpawnDropships(int count) {
			for (int i = 0; i < count; i++) {
				SpawnDropship();
				yield return new WaitForSeconds(_dropshipWaitTime.Pick());
			}
		}

		private void SpawnDropship() {
			GameObject prefab = null;
			if (_spawnDropshipsRandomly) {
				prefab = _dropshipPrefabs[UnityEngine.Random.Range(0, _dropshipPrefabs.Length)];
			} else {
				prefab = _dropshipPrefabs[_dropshipIndex];
				_dropshipIndex++;
				if (_dropshipIndex >= _dropshipPrefabs.Length) {
					_dropshipIndex = 0;
				}
			}
			float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
			float height = Mathf.Max((_gameManager.CameraHeight * _spawnHeightScale.Pick()), 15.0f);
			// float distance = Random.Range(Ground.Radius * 0.5f, Ground.Radius);
			float distance = Ground.Radius;// * _spawnRadius.Pick();
			Vector3 point = Ground.GetPositionAt(angle, distance, height);
			Debug.LogFormat("New Dropship at [{0}, {1}], pos: {2}", angle, distance, point);
			Pose p = new Pose(point, Quaternion.identity);

			Dropship ship = Instantiate(prefab, _gameManager.WorldParent).GetComponent<Dropship>();
			ship.transform.SetPose(p);
#if !UNITY_EDITOR
			ARReferencePoint anchor = _gameManager.PointManager.AddReferencePoint(p);
			ship.transform.parent = anchor.transform;
#endif// !UNITY_EDITOR
		}

		private void OnBaseKilled(Base b) {
			Debug.Log("OnBaseKilled");
			EndGame();
		}

		public void OnPlayerKilled() {
			EndGame();
		}

		public void EndGame() {
			LevelSaveData data = _saveDataManager.LoadData<LevelSaveData>("TestLevelData");
			string body = null;
			if (_currentScore > data.HighScore) {
				data.HighScore = _currentScore;
				_saveDataManager.SaveData("TestLevelData", data);
				body = string.Format("You base was destroyed.\n\nNew High Score: {0}", _currentScore);
			} else {
				body = string.Format("You base was destroyed.\n\nScore: {0}", _currentScore);
			}

			_playerHUD.Hide();
			_displayManager.PushDefault();
			_displayManager.ShowOKCancelModal("Game Over", body, "Main Menu", null)
			.Then((eModalOption option) => {
				_mainMenu.Show().Then(() => {
					CleanupLevel();
					if (OnLevelEnded != null) { OnLevelEnded(); }
				});
			});
		}

		private void CleanupLevel() {
			foreach (GameObject obj in _enemyList) {
				Destroy(obj);
			}
			if (PlayerBase != null) {
				Destroy(PlayerBase.gameObject);
			}
			Destroy(Ground.gameObject);
			_enemyList.Clear();
		}

		public void RegisterEnemy(GameObject go) {
			_enemyList.Add(go);
			if (OnEnemyCreated != null) {
				OnEnemyCreated(go);
			}
		}
		public void UnregisterEnemy(GameObject go) {
			_enemyList.Remove(go);

			Dropship dropship = go.GetComponent<Dropship>();
			Fighter fighter = go.GetComponent<Fighter>();
			Tank tank = go.GetComponent<Tank>();
			if (dropship != null) {
				_currentScore += _dropshipScore;
			} else if (fighter != null) {
				_currentScore += _fighterScore;
			} else if (tank != null) {
				_currentScore += _tankScore;
			}

			if (OnEnemyKilled != null) {
				OnEnemyKilled(go);
			}
		}
	}
}