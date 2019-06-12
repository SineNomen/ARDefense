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
		private float _startSpeed = 5.0f;
		[SerializeField]
		private float _flightSpeed = 10.0f;
		[SerializeField]
		private RandomFloat _deployTime = new RandomFloat(1.0f, 1.0f, 0.0f, 5.0f);
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private float _rotationSpeedScale = 1.0f;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private eMissileProjectilePhase _currentPhase = eMissileProjectilePhase.Deploy;
		private Vector3 _startPos;
		private float _startTime;
		private Transform _target;

		public float FlightSpeed { get => _flightSpeed; }
		public float TurnSpeed { get => _turnSpeed; }
		private float DistanceFlown { get => (transform.position - _startPos).magnitude; }
		private float FlightTime { get => (Time.time - _startTime); }

		private void Start() {
			Container.AutoInject(this);
		}

		public override void Launch(Cannon cannon, Transform target) {
			SetPhase(eMissileProjectilePhase.Deploy);
		}

		private Vector3 GetTargetPos() {
			return _target.position - this.transform.position;
		}

		private void CheckPhase() {
			eMissileProjectilePhase newPhase = _currentPhase;
			switch (_currentPhase) {
				case eMissileProjectilePhase.Deploy:
					if (FlightTime > _deployTime) { newPhase = eMissileProjectilePhase.ChaseTarget; }
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
					Quaternion rotation = Quaternion.LookRotation(GetTargetPos(), this.transform.up);

					transform.rotation = rotation;
					_startPos = transform.position;
					break;
				case eMissileProjectilePhase.ChaseTarget:
					break;
			}
		}

		public IEnumerator UpdatePattern() {
			while (true) {
				Quaternion lookRotation = this.transform.rotation;
				float rotAngle = 0.0f;
				Vector3 deltaPosition = GetTargetPos();
				Quaternion look = Quaternion.LookRotation(deltaPosition, Vector3.up);
				float distanceToTarget = Vector3.Distance(this.transform.position, deltaPosition);
				switch (_currentPhase) {
					case eMissileProjectilePhase.Deploy:
						//move forward, increase speed
						//fly along what we already decided unti we go the distance
						break;
					case eMissileProjectilePhase.ChaseTarget:
						//go perpendicular to the the right or left
						//increase speed to maximum, move towads the target

						//maybe just spi  as you go
						// rotAngle = Mathf.Min(_flightSpeed * _rotationSpeedScale, 90.0f);
						//pick the closer one
						// if (Quaternion.Angle(transform.rotation, look)) {
						// 	rotAngle = -rotAngle;
						// 	look = left;
						// } else {
						// 	look = right;
						// }
						break;
				}

				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);

				_body.velocity = transform.forward * _flightSpeed;
				// Quaternion lerp = Quaternion.Lerp(_fighter.Model.localRotation, Quaternion.AngleAxis(rotAngle, Vector3.forward), Time.deltaTime);
				// _fighter.Model.localRotation = lerp;
				yield return null;
				// Debug.LogFormat("Distance: {0}", distanceToTarget);
				CheckPhase();
			}
		}
	}
}