using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;
using VolumetricLines;
using DG.Tweening;

/*
Has a list of weapons
	a current weapon
Is damagable
has a team

*/

namespace Sojourn.ARDefense.Components {
	public class DropShip : SimpleEnemy {
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
		[Tooltip("Local position to spawn objects, if not spawning on the ground")]
		private Vector3 _spawnOffset = Vector3.zero;
		[SerializeField]
		[Tooltip("Delay between objects")]
		private float _delay = 1;

		[SerializeField]
		private bool _spawnOnStart = true;
		[SerializeField]
		private bool _spawnOnGround = true;

		private Anchor _anchor = null;


		private void Awake() {
		}

		private void Start() {
			Container.AutoInject(this);
			if (_spawnOnStart) {
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
			float height = -this.transform.localPosition.z;
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
			Vector3 spawnPos = this.transform.position + _spawnOffset;
			if (_spawnOnGround) {
				spawnPos.z = 0.0f;
			}
			Debug.LogFormat("Spawn at {0}", spawnPos);
			for (int i = 0; i < _count; i++) {
				Instantiate(_prefab, spawnPos, this.transform.rotation, null);
				yield return new WaitForSeconds(_delay);
			}
		}
	}
}