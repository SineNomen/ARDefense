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
using DG.Tweening;

/*
Has a list of weapons
	a current weapon
Is damagable
has a team

*/

//`Mat fires when something on the other team is in range
namespace Sojourn.ARDefense.Components {
	// [RequireComponent(typeof(SphereCollider))]
	public class SimpleTurret : MonoBehaviour {
		[SerializeField]
		private Transform _cannonTransform = null;
		[SerializeField]
		private Cannon _cannon = null;
		[SerializeField]
		private Weapon _weapon = null;
		[SerializeField]
		private bool _shootAtBase = false;
		[SerializeField]
		private bool _shootAtPlayer = false;
		[SerializeField]
		private float _speed = 2.0f;
		[SerializeField]
		[Tooltip("How close to point at the target we need to be before trying to fire")]
		private float _fireThreshold = 2.0f;

		private Transform _target = null;
		private Coroutine _trackCoroutine = null;

		[AutoInject]
		private IGameManager _gameManager = null;

		private IKillable _killable = null;

		private void Start() {
			Container.AutoInject(this);
			_killable = GetComponent<IKillable>();
			_cannon.Killable = _killable;
			_cannon.Weapon = _weapon;
		}

		private void OnTriggerEnter(Collider collider) {
			IPlayer player = collider.gameObject.GetComponent<IPlayer>();
			Base theirBase = collider.gameObject.GetComponent<Base>();
			if (_shootAtBase && theirBase != null && theirBase.Team != _killable.Team) {
				//target acquired
				SetTarget(collider.transform);
			} else if (_shootAtPlayer && player != null) {
				SetTarget(collider.transform);
			}
		}

		private void SetTarget(Transform tr) {
			Debug.LogFormat("{0}, new target: {1}", this.name, tr);
			_target = tr;
			if (_target == null && _trackCoroutine != null) {
				StopCoroutine(_trackCoroutine);
			} else {
				_trackCoroutine = StartCoroutine(TrackTarget());
			}
		}

		[ContextMenu("TrackBase")]
		private void StartTracking() {
			_target = _gameManager.Player1Base.Transform;
			StartCoroutine(TrackTarget());
		}

		private IEnumerator TrackTarget() {
			while (_target != null) {
				Vector3 relativePos = _target.position - transform.position;
				Quaternion look = Quaternion.LookRotation(relativePos, _cannonTransform.up);
				_cannonTransform.rotation = Quaternion.Lerp(_cannonTransform.rotation, look, _speed * Time.deltaTime);
				float angle = Quaternion.Angle(look, _cannonTransform.rotation);
				if (angle < _fireThreshold) {
					TryShoot();
				}
				// _cannonTransform.LookAt(_target);
				yield return null;
			}
		}


		private void TryShoot() {
			// Debug.LogFormat("TryShoot: {0}", _cannon.IsReadyToFire);
			if (_cannon.IsReadyToFire) {
				// Debug.Log("Fire");
				StartCoroutine(_cannon.Fire());
			}
		}

		private void OnTriggerExit(Collider collider) {
			if (_target == collider.transform) {
				SetTarget(null);
			}
		}
	}
}