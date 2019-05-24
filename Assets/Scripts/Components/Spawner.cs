using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Sojourn.ARDefense.Components {
	public class Spawner : MonoBehaviour {
		[SerializeField]
		private GameObject _prefab = null;
		[SerializeField]

		[Tooltip("How many pbjects to spawn")]
		private int _count = 1;
		[SerializeField]
		[Tooltip("Delay before spawning")]
		private float _startDelay = 1.0f;
		[SerializeField]
		[Tooltip("Delay between objects")]
		private float _delay = 1;

		[SerializeField]
		private bool _spawnOnStart = true;


		// private List<GameObject> _spawnedObjects = new List<GameObject>();

		private void Start() {
			Container.AutoInject(this);
			if (_spawnOnStart) {
				Spawn();
			}
		}

		private void Spawn() {
			StartCoroutine(SpawnObjects());
		}

		private IEnumerator SpawnObjects() {
			yield return new WaitForSeconds(_startDelay);
			for (int i = 0; i < _count; i++) {
				Instantiate(_prefab, this.transform.position, this.transform.rotation);
				yield return new WaitForSeconds(_delay);
			}
		}
	}
}