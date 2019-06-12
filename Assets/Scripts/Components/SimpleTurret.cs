using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;


//`Mat fires when something on the other team is in range
namespace Sojourn.ARDefense.Components {
	public enum eTurretTargetMode {
		PlayerBase,
		BaseAndPlayer,
		GroundEnemies,
		AirEnemies,
		GroundAndAirEnemies
	}
	public class SimpleTurret : MonoBehaviour {
		[SerializeField]
		private Transform _cannonTransform = null;
		[SerializeField]
		private Cannon _cannon = null;
		[SerializeField]
		private Weapon _weapon = null;
		// [SerializeField]
		// private bool _shootAtBase = false;
		// [SerializeField]
		// private bool _shootAtPlayer = false;
		// [SerializeField]
		private float _speed = 2.0f;
		[SerializeField]
		[Tooltip("How close to point at the target we need to be before trying to fire")]
		private float _fireThreshold = 2.0f;

		private Transform _target = null;
		private Coroutine _trackCoroutine = null;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private IKillable _killable = null;

		private void Start() {
			Container.AutoInject(this);
			_killable = GetComponent<IKillable>();
			_cannon.Killable = _killable;
			_cannon.Weapon = _weapon;
			StartCoroutine(CheckForTarget());
		}

		private IEnumerator CheckForTarget() {
			while (true) {
				if (Vector3.Distance(_levelManager.PlayerBase.Transform.position, this.transform.position) <= _fireThreshold) {
					SetTarget(_levelManager.PlayerBase.Transform);
				} else {
					SetTarget(null);
				}
				yield return null;
			}
		}

		// private void OnTriggerEnter(Collider collider) {
		// 	IPlayer player = collider.gameObject.GetComponent<IPlayer>();
		// 	Base theirBase = collider.gameObject.GetComponent<Base>();
		// 	if (_shootAtBase && theirBase != null && theirBase.Team != _killable.Team) {
		// 		//target acquired
		// 		SetTarget(collider.transform);
		// 	} else if (_shootAtPlayer && player != null) {
		// 		SetTarget(collider.transform);
		// 	}
		// }

		private void SetTarget(Transform tr) {
			// Debug.LogFormat("{0}, new target: {1}", this.name, tr);
			_target = tr;
			if (_target == null && _trackCoroutine != null) {
				StopCoroutine(_trackCoroutine);
			} else {
				_trackCoroutine = StartCoroutine(TrackTarget());
			}
		}

		[ContextMenu("TrackBase")]
		private void StartTracking() {
			_target = _levelManager.PlayerBase.Transform;
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