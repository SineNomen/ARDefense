using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using VolumetricLines;
using DG.Tweening;

namespace Sojourn.ARDefense.Components {
	public class Dropship : SimpleEnemy {
		[SerializeField]
		private VolumetricLineBehavior _line = null;
		[SerializeField]
		private GameObject _prefab = null;
		[SerializeField]
		[Tooltip("How many opbjects to spawn")]
		private int _count = 1;
		[SerializeField]
		[Tooltip("Delay before spawning")]
		private float _startDelay = 5.0f;
		[Tooltip("Local position to spawn objects")]
		[SerializeField]
		private Transform _spawnTransform = null;
		[SerializeField]
		[Tooltip("Delay between objects")]
		private float _delay = 1;

		[SerializeField]
		private bool _spawnOnStart = true;
		[SerializeField]
		private bool _spawnOnGround = true;

		private static int _createCount = 0;

		protected override void Awake() {
			base.Awake();
			gameObject.name = string.Format("Dropship #{0}", _createCount);
			_createCount++;
		}

		protected override void Start() {
			base.Start();
			if (_spawnOnStart && _count > 0) {
				Spawn();
			}
		}

		[ContextMenu("Spawn")]
		private void Spawn() {
			this.Wait(_startDelay)
			.Chain(DoSpawnAnimation)
			.Then(StartSpawning);
		}

		private IPromise DoSpawnAnimation() {
			float height = this.transform.position.y - _levelManager.Ground.Center.y;

			return DOVirtual.Float(0.0f, height, 3.0f, (val) => {
				Vector3 pos = _line.EndPos;
				pos.z = val;
				_line.EndPos = pos;
			}).ToPromise();
		}

		private void StartSpawning() {
			StartCoroutine(SpawnObjects());
		}
		private IEnumerator SpawnObjects() {
			Vector3 spawnPos = _spawnTransform.position;
			if (_spawnOnGround) {
				spawnPos.y = _levelManager.Ground.Center.y;
			}
			Debug.LogWarningFormat("Spawn at {0}, from {1}", spawnPos, this.transform.position);
			for (int i = 0; i < _count; i++) {
				Instantiate(_prefab, spawnPos, this.transform.rotation, _gameManager.WorldParent);
				yield return new WaitForSeconds(_delay);
			}
		}
	}
}