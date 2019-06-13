using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Utility;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {

	//1) Drop and begin to move (quickly ramping up to full speed)
	//2) Fly full-speed towards target
	public enum eMissileProjectilePhase {
		Deploy,//start at low speed, then ramp up going straight
		ChaseTarget,//Fy towards the target to hit it
	}
	//`Mat maybe add an ancticipate (aim along the target's forward vector)?
	public class MissileProjectile : SimpleProjectile, IProjectile {
		[SerializeField]
		private Transform _model = null;
		[SerializeField]
		private float _startSpeed = 5.0f;
		[SerializeField]
		private RandomFloat _deployTime = new RandomFloat(1.0f, 1.0f, 0.0f, 5.0f);
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private float _rotationSpeedScale = 1.0f;

		private eMissileProjectilePhase _currentPhase = eMissileProjectilePhase.Deploy;
		private Vector3 _startPos;
		private float _startTime;
		private Transform _target;
		private Quaternion _upRotation;
		private Vector3 _forward;
		private float _spin = 0.0f;

		public float TurnSpeed { get => _turnSpeed; }
		private float DistanceFlown { get => (transform.position - _startPos).magnitude; }
		private float FlightTime { get => (Time.time - _startTime); }

		private void Start() {
			Container.AutoInject(this);
		}

		public override void OnFire() {
			Invoke("Destroy", Weapon.ProjectileLifetime);
		}

		public override void Launch(Cannon cannon, Transform target) {
			_target = target;
			SetPhase(eMissileProjectilePhase.Deploy);
			StartPattern();
		}

		private Vector3 GetTargetPos() {
			return _target.position - this.transform.position;
		}

		private void CheckPhase() {
			eMissileProjectilePhase newPhase = _currentPhase;
			switch (_currentPhase) {
				case eMissileProjectilePhase.Deploy:
					if (FlightTime > _deployTime) {
						newPhase = eMissileProjectilePhase.ChaseTarget;
					}
					break;
			}

			if (newPhase != _currentPhase) {
				SetPhase(newPhase);
			}
		}

		private void SetPhase(eMissileProjectilePhase newPhase) {
			// Use with "Error Pause" option for easy debugging
			// Debug.LogErrorFormat("Switch phase {0} -> {1}", _currentPhase, newPhase);
			_currentPhase = newPhase;
			switch (_currentPhase) {
				case eMissileProjectilePhase.Deploy:
					_startTime = Time.time;
					_deployTime.Pick();
					_startPos = transform.position;
					_upRotation = Quaternion.LookRotation(transform.up, transform.up);
					_forward = transform.forward;
					// transform.rotation = Quaternion.LookRotation(transform.up, transform.up);
					break;
				case eMissileProjectilePhase.ChaseTarget:
					// _body.useGravity = false;
					// Quaternion rotation = Quaternion.LookRotation(GetTargetPos(), this.transform.up);
					// transform.rotation = rotation;
					break;
			}
		}

		public IEnumerator UpdatePattern() {
			while (true) {
				_spin += (_rotationSpeedScale * Time.deltaTime);
				Vector3 deltaPosition = GetTargetPos();
				Quaternion lookRotation = Quaternion.LookRotation(deltaPosition, Vector3.up);
				switch (_currentPhase) {
					case eMissileProjectilePhase.Deploy:
						//move forward, increase speed
						_body.velocity = (_forward + Vector3.down) * _startSpeed;
						_model.localEulerAngles = Vector3.forward * _spin * Mathf.Rad2Deg;
						Quaternion q = Quaternion.Lerp(this.transform.rotation, _upRotation, _turnSpeed * Time.deltaTime);
						this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _upRotation, Time.deltaTime);
						break;
					case eMissileProjectilePhase.ChaseTarget:
						_model.localEulerAngles = Vector3.forward * _spin * Mathf.Rad2Deg;
						this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);
						_body.velocity = transform.forward * Weapon.Speed;
						break;
				}

				yield return null;
				CheckPhase();
			}
		}

		public void StartPattern() {
			StartCoroutine(UpdatePattern());
		}

		private void Destroy() {
			if (this != null && this.gameObject != null) {
				Destroy(this.gameObject);
			}
		}
	}
}