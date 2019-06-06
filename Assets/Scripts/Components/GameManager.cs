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
using Sojourn.Utility;

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
		private IDisplayManager _displayManager = null;
		[AutoInject]
		private ILevelManager _levelManager = null;

		[SerializeField]

		public ARSession ArSession { get => _arSession; }
		public Camera DeviceCamera { get => _deviceCamera; }
		private Reticule Reticule { get => _displayManager.CurrentDisplay.Reticule; }

		public ARRaycastManager RaycastManager { get => _raycastManager; }
		public ARReferencePointManager PointManager { get => _pointManager; }
		public ARPlaneManager PlaneManager { get => _planeManager; }
		public ARSessionOrigin Origin { get => _origin; }
		public Transform WorldParent { get => null; }
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
			Container.AutoInject(this);
#if !UNITY_EDITOR
			foreach (GameObject go in _debugObjects) {
				go.SetActive(false);
			}
#endif// !UNITY_EDITOR
		}
	}
}