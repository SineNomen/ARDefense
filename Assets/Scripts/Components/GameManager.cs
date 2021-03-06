﻿using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using Sojourn.Interfaces;
using Sojourn.Utility;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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
		[AutoInject]
		private IMainMenu _mainMenu = null;

		public ARSession ArSession { get => _arSession; }
		public Camera DeviceCamera { get => _deviceCamera; }

		public ARRaycastManager RaycastManager { get => _raycastManager; }
		public ARReferencePointManager PointManager { get => _pointManager; }
		public ARPlaneManager PlaneManager { get => _planeManager; }
		public ARSessionOrigin Origin { get => _origin; }
		public float OriginScale { get => _origin.transform.localScale.x; }
		public Transform WorldParent { get => null; }
		public float CameraHeight { get => DeviceCamera.transform.position.y; }

		private void Awake() {
			Container.Register<IPersistentDataManager>(new PersistentDataManager()).AsSingleton();
			Container.Register<IGameManager>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
#if !UNITY_EDITOR
			foreach (GameObject go in _debugObjects) {
				go.SetActive(false);
			}
#endif// !UNITY_EDITOR
			_mainMenu.ShowInstant();
		}
	}
}