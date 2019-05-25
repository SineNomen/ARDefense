using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using System.Collections;
using System.Collections.Generic;
//`Mat hack, shouldn't need components
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void GameObjectEvent(GameObject go);
	public interface IGameManager {
		DetectedPlane GroundPlane { get; }
		ARCoreSession ArSession { get; }
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