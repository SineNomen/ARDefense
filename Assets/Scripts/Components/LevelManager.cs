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

namespace Sojourn.ARDefense.Components {
	public class LevelManager : MonoBehaviour, ILevelManager {
		[SerializeField]
		private GameObject _groundPrefab = null;
		[SerializeField]
		private GameObject _basePrefab = null;
		[SerializeField]
		private GameObject[] _dropshipPrefabs = null;
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _minSpawnRadiusScale = 0.5f;
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _maxSpawnRadiusScale = 1.0f;
		[SerializeField]
		private bool _playerPlacesBase = false;

		public Base PlayerBase { get; private set; }
		public ARPlane GroundPlane { get; private set; }
		public Ground Ground { get; private set; }
		public List<GameObject> EnemyList { get => _enemyList; }
		public GameObjectEvent OnEnemyCreated { get; set; }
		public GameObjectEvent OnEnemyKilled { get; set; }

		private List<GameObject> _enemyList = new List<GameObject>();

		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private IObjectPlacer _objectPlacer = null;
		[AutoInject]
		private IPlaneSelector _planeSelector = null;

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
			PlayerBase.OnBaseKilled += OnBaseKilled;
			StartCoroutine(SpawnDropships(100));
		}


		private void CreateTestLevel() {
			Ground = Instantiate(_groundPrefab, Vector3.zero, Quaternion.identity, _gameManager.WorldParent).GetComponent<Ground>();
			Ground.Radius = 60.0f;
			//hack for the to fix the prefab for testing on editor, scale is off
			foreach (Transform child in Ground.Transform) { child.localScale *= 2.0f; }
		}

		public IPromise CreateBasicLevel() {
			return _planeSelector.SelectPlane()
			.Then((ARPlane plane) => {
				Debug.LogErrorFormat("Plane Selected: {0}", plane);
				GroundPlane = plane;
				Ground = Instantiate(_groundPrefab, Vector3.zero, Quaternion.identity, _gameManager.WorldParent).GetComponent<Ground>();
				Ground.Radius = Mathf.Min(plane.size.x, plane.size.y) * _gameManager.Origin.transform.localScale.x;
				Pose pose = new Pose(plane.transform.position, Quaternion.identity);
				ARReferencePoint anchor = _gameManager.PointManager.AttachReferencePoint(plane, pose);
				Ground.transform.SetPose(pose);
				Ground.transform.SetParent(anchor.transform);
			});
		}

		private IPromise SetupBase() {
			return (_playerPlacesBase ? ChoosePlaceBase() : PlaceBase())
			.Then(delegate (Base b) {
				PlayerBase = b;
				Debug.LogErrorFormat("Base Placed: {0}", b.Transform.position);
				Debug.LogError("Setup complete!");
			});
		}

		//pu the base in the center
		public IPromise<Base> PlaceBase() {
			Debug.LogError("Place Base");
			Base b = Instantiate(_basePrefab, Ground.Center, Quaternion.identity, _gameManager.WorldParent).GetComponent<Base>();
#if !UNITY_EDITOR
			Pose center = new Pose(GroundPlane.center, Quaternion.identity);
			b.transform.position = GroundPlane.center;
			ARReferencePoint point = _gameManager.PointManager.AttachReferencePoint(GroundPlane, center);
			b.transform.parent = point.transform;
#endif// !UNITY_EDITOR
			return Promise<Base>.Resolved(b);
		}

		//use the object placer to let the user put it where they want
		public IPromise<Base> ChoosePlaceBase() {
			Debug.LogError("Place Base");
			return _objectPlacer.PlaceObjectOnPlane<Base>(_basePrefab, GroundPlane);
		}

		private IEnumerator SpawnDropships(int count) {
			for (int i = 0; i < count; i++) {
				SpawnDropship();
				yield return new WaitForSeconds(30.0f);
			}
		}

		private void OnBaseKilled(Base b) {
			Debug.LogError("Game Over!");
		}

		private void SpawnDropship() {
			GameObject prefab = _dropshipPrefabs[UnityEngine.Random.Range(0, _dropshipPrefabs.Length)];
			float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
			float height = Mathf.Max((_gameManager.CameraHeight * 1.25f), 15.0f);
			// float distance = Random.Range(Ground.Radius * 0.5f, Ground.Radius);
			float distance = Random.Range(Ground.Radius * _minSpawnRadiusScale, Ground.Radius * _maxSpawnRadiusScale);
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

		public void OnPlayerKilled() {
			Debug.LogError("Game Over!");
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