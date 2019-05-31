using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
//`Mat hack, shouldn't need components
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void GameObjectEvent(GameObject go);
	public interface IGameManager {
		ARPlane GroundPlane { get; }
		ARSession ArSession { get; }
		ARRaycastManager RaycastManager { get; }
		ARReferencePointManager PointManager { get; }
		ARPlaneManager PlaneManager { get; }
		ARSessionOrigin Origin { get; }
		Transform WorldParent { get; }
		Vector3 GroundPosition { get; }
		Camera DeviceCamera { get; }
		Base Player1Base { get; }
		List<GameObject> EnemyList { get; }

		IPromise StartNewGame();
		GameObjectEvent OnEnemyCreated { get; set; }
		GameObjectEvent OnEnemyKilled { get; set; }
		void OnPlayerKilled();
		void RegisterEnemy(GameObject go);
		void UnregisterEnemy(GameObject go);
	}
}