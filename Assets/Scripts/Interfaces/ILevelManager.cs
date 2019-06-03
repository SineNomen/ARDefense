using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Interfaces {
	public delegate void GameObjectEvent(GameObject go);
	public interface ILevelManager {
		Ground Ground { get; }
		ARPlane GroundPlane { get; }
		Base PlayerBase { get; }

		List<GameObject> EnemyList { get; }
		GameObjectEvent OnEnemyCreated { get; set; }
		GameObjectEvent OnEnemyKilled { get; set; }

		void OnPlayerKilled();
		void RegisterEnemy(GameObject go);
		void UnregisterEnemy(GameObject go);

		IPromise SetupLevel();
		void StartLevel();
	}
}