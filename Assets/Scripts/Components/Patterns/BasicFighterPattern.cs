using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Utility;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
	//1) Orbit around the target
	//2) Turn towards the target and fire
	//3) Veer away from target and enter new orbit
	//repeat

	public enum eBasicFighterPatternPhase {
		Entry,//start flying toward the target, only used at the start
		OrbitTarget,//fly in a circle around the target
		Dive,//Fy towards the target with intent to shoot
		Disengage//Fly away from the target to re-enter a circle
	}
	[RequireComponent(typeof(Fighter))]
	public class BasicFighterPattern : MonoBehaviour {
		[SerializeField]
		private RandomFloat _flightSpeed = new RandomFloat(8.0f, 8.0f, 0.0f, 100.0f);
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private bool _startOnCreate = true;
		[SerializeField]
		private bool _spreadOnStart = true;
		[SerializeField]
		private RandomFloat _spreadDistance = new RandomFloat(5.0f, 7.0f, 0.0f, 10.0f);
		[SerializeField]
		private RandomFloat _spreadRange = new RandomFloat(-30.0f, 30.0f, -45.0f, 45.0f);
		[SerializeField]
		private RandomFloat _orbitDistance = new RandomFloat(10.0f, 12.0f, 5.0f);
		[SerializeField]
		private RandomFloat _orbitTime = new RandomFloat(2.0f, 10.0f, 0.0f, 60.0f);
		[SerializeField]
		private RandomFloat _orbitHeightVariance = new RandomFloat(-10.0f, 10.0f);

		[SerializeField]
		private float _disengageRange = 4.0f;
		[SerializeField]
		private float _rotationSpeedScale = 1.0f;
		[SerializeField]
		private float _shootRange = 5.0f;


		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private ILevelManager _levelManager = null;

		private eBasicFighterPatternPhase _currentPhase = eBasicFighterPatternPhase.Entry;
		private Fighter _fighter;
		private Vector3 _startPos;
		private float _diveTime = -1;
		private Quaternion _escapeRotation;

		public float FlightSpeed { get => _flightSpeed; }
		public float TurnSpeed { get => _turnSpeed; }
		private float DistanceFlown { get => (transform.position - _startPos).magnitude; }

		private void Awake() {
			_fighter = GetComponent<Fighter>();
			_flightSpeed.Pick();
		}

		private void Start() {
			Container.AutoInject(this);
			if (_startOnCreate) {
				Setup();
				StartPattern();
			}
		}

		public void Setup() {
			SetPhase(eBasicFighterPatternPhase.Entry);
		}

		private Vector3 GetTargetPos(bool keepHeight) {
			Vector3 pos = _levelManager.PlayerBase.TopPosition;
			if (keepHeight) { pos.y = this.transform.position.y; }
			return pos - this.transform.position;
		}

		private void CheckPatternPhase() {
			eBasicFighterPatternPhase newPhase = _currentPhase;
			float distanceToTarget = Vector3.Distance(this.transform.position, GetTargetPos(true));
			switch (_currentPhase) {
				case eBasicFighterPatternPhase.Entry:
					//we need to get INSIDE orbit distance
					if (distanceToTarget < _orbitDistance) { newPhase = eBasicFighterPatternPhase.OrbitTarget; }
					break;
				case eBasicFighterPatternPhase.OrbitTarget:
					if (_diveTime > 0.0f && Time.time > _diveTime) {
						_diveTime = -1;
						newPhase = eBasicFighterPatternPhase.Dive;
					}
					break;
				case eBasicFighterPatternPhase.Dive:
					float range = _disengageRange;
#if UNITY_EDITOR
					range /= 2.0f;
#endif// UNITY_EDITOR
					Debug.LogWarningFormat("Diving, distance: {0}, range: {1}", distanceToTarget, range);
					if (distanceToTarget < range) {
						newPhase = eBasicFighterPatternPhase.Disengage;
					}
					break;
				case eBasicFighterPatternPhase.Disengage:
					//we need to get PAST orbit distance
					if (distanceToTarget > _orbitDistance) { newPhase = eBasicFighterPatternPhase.OrbitTarget; }
					break;
			}

			if (newPhase != _currentPhase) {
				SetPhase(newPhase);
			}
		}

		private void SetPhase(eBasicFighterPatternPhase newPhase) {
			// Use with "Error Pause" option for easy debugging
			// Debug.LogErrorFormat("[{0}] Switch phase {1} -> {2}", gameObject.name, _currentPhase, newPhase);
			_currentPhase = newPhase;
			switch (_currentPhase) {
				case eBasicFighterPatternPhase.Entry:
					_diveTime = -1;
					_orbitDistance.Pick();
					_spreadDistance.Pick();
					Quaternion rotation = Quaternion.LookRotation(GetTargetPos(true), this.transform.up);
					if (_spreadOnStart) {
						rotation *= Quaternion.Euler(0.0f, _spreadRange.Pick(), 0.0f);
					}

					transform.rotation = rotation;
					_startPos = transform.position;
					break;
				case eBasicFighterPatternPhase.OrbitTarget:
					_diveTime = Time.time + _orbitTime.Pick();
					break;
				//if we aim too high we will get far away before we hit the orbit distance
				case eBasicFighterPatternPhase.Disengage:
					float distanceToTarget = Vector3.Distance(this.transform.position, GetTargetPos(true));
					_diveTime = -1;
					Vector3 offset = (Random.value > 0.5f ? transform.right : -transform.right);
					Vector3 dir = transform.forward + offset;
					Vector3 pos = GetTargetPos(false) + (dir * (_orbitDistance.Pick() * 0.5f));
					pos.y = (_startPos.y + _orbitHeightVariance.Pick());
					Quaternion q = Quaternion.LookRotation(pos, this.transform.up);
					//need to figure out a point on the sphere of orbit
					_escapeRotation = q;
					Debug.LogErrorFormat("Disengage, distance: {0}, angle: {1}, height variance: {2}", distanceToTarget, Quaternion.Angle(this.transform.rotation, _escapeRotation), _orbitHeightVariance);
					break;
			}
		}

		//1) Spread out in a fan
		//2) Move towards target
		//3) Orbit around target
		public IEnumerator UpdatePattern() {
			while (true) {
				Quaternion lookRotation = this.transform.rotation;
				Vector3 deltaPosition = GetTargetPos(true);
				float distanceToTarget = Vector3.Distance(this.transform.position, deltaPosition);
				float rotAngle = Mathf.Min(_flightSpeed * _rotationSpeedScale, 90.0f);
				switch (_currentPhase) {
					case eBasicFighterPatternPhase.Entry:
						//fly along what we already decided unti we go the distance
						if (DistanceFlown > _spreadDistance) {
							lookRotation = Quaternion.LookRotation(GetTargetPos(true), this.transform.up);
						}
						break;
					case eBasicFighterPatternPhase.OrbitTarget:
						//go perpendicular to the the right or left
						//pick the closer one
						Quaternion look = Quaternion.LookRotation(deltaPosition, Vector3.up);
						Quaternion left = look * Quaternion.Euler(0.0f, -90.0f, 0.0f);
						Quaternion right = look * Quaternion.Euler(0.0f, 90.0f, 0.0f);
						if (Quaternion.Angle(transform.rotation, left) < Quaternion.Angle(transform.rotation, right)) {
							rotAngle = -rotAngle;
							look = left;
						} else {
							look = right;
						}
						this.transform.rotation = Quaternion.Slerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
						break;
					case eBasicFighterPatternPhase.Dive:
						lookRotation = Quaternion.LookRotation(GetTargetPos(false), this.transform.up);
						rotAngle = 0.0f;
						//once we are facing the base, start shooting
						if (Quaternion.Angle(this.transform.rotation, lookRotation) < _shootRange) {
							TryShootMain();
						}
						break;
					case eBasicFighterPatternPhase.Disengage:
						//pick the closer one
						Vector3 axis;
						float angle;
						_escapeRotation.ToAngleAxis(out angle, out axis);
						if (Vector3.Dot(transform.forward, axis) > 0.0f) {
							rotAngle = -rotAngle;
						}
						lookRotation = _escapeRotation;
						break;
				}

				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);

				_fighter.Body.velocity = transform.forward * _flightSpeed;
				Quaternion lerp = Quaternion.Lerp(_fighter.Model.localRotation, Quaternion.AngleAxis(rotAngle, Vector3.forward), Time.deltaTime);
				_fighter.Model.localRotation = lerp;
				yield return null;
				// Debug.LogFormat("Distance: {0}", distanceToTarget);
				CheckPatternPhase();
			}
		}

		private void TryShootMain() {
			// Debug.Log("TryShoot");
			foreach (Cannon c in _fighter.MainCannons) {
				if (c.IsReadyToFire) {
					// Debug.Log("Fire");
					StartCoroutine(c.Fire());
				}
			}
		}

		[ContextMenu("StartPattern")]
		public void StartPattern() {
			StartCoroutine(UpdatePattern());
		}
	}
}