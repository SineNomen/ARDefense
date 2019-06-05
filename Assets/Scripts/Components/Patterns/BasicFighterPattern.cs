using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
	//make a figure 8, shooter when the base is lined up
	//1) Fly direct and shoot until within a acertain range of target
	//2) Turn around, until angle between us and target is about 180
	//3) Fly away until a certain distance away
	//4) Turn around until we are facing the target
	//repeat

	//1) Orbit around the target
	//2) Turn towards the target and fire
	//3) Veer away from target and enter new orbit


	// public enum eBasicFighterPatternType {
	// 	CircleAndDive,
	// 	Figure8,
	// 	Flower,
	// }
	public enum eBasicFighterPatternPhase {
		Entry,//start flying toward the target, only used at the start
		OrbitTarget,//fly in a circle around the target
		Dive,//Fy towards the target with intent to shoot
		Disengage//Fly away from the target to re-enter a circle
	}
	[RequireComponent(typeof(Fighter))]
	public class BasicFighterPattern : MonoBehaviour {
		[SerializeField]
		private float _flightSpeed = 3.0f;
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private bool _startOnCreate = true;
		[SerializeField]
		private bool _spreadOnStart = true;
		[SerializeField]
		private float _spreadDistance = 5.0f;
		[SerializeField]
		[Range(0, 45.0f)]
		private float _spreadRange = -30.0f;

		[SerializeField]
		private float _disengageDistance = 4.0f;
		[SerializeField]
		private float _orbitDistance = 10.0f;
		[SerializeField]
		private float _minOrbitTime = 2.0f;
		[SerializeField]
		private float _maxOrbitTime = 10.0f;
		[SerializeField]
		private float _rotationSpeedScale = 1.0f;


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
			Vector3 pos = _levelManager.PlayerBase.CenterPosition;
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
					if (distanceToTarget < _disengageDistance) {
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
			// Debug.LogErrorFormat("Switch phase {0} -> {1}", _currentPhase, newPhase);
			_currentPhase = newPhase;
			switch (_currentPhase) {
				case eBasicFighterPatternPhase.Entry:
					_diveTime = -1;
					Quaternion rotation = Quaternion.LookRotation(GetTargetPos(true), this.transform.up);
					if (_spreadOnStart) {
						rotation *= Quaternion.Euler(0.0f, Random.Range(-_spreadRange, _spreadRange), 0.0f);
					}

					transform.rotation = rotation;
					_startPos = transform.position;
					break;
				case eBasicFighterPatternPhase.OrbitTarget:
					_diveTime = Time.time + Random.Range(_minOrbitTime, _maxOrbitTime);
					break;
				//if we aim too high we will get far away before we hit the orbit distance
				case eBasicFighterPatternPhase.Disengage:
					float distanceToTarget = Vector3.Distance(this.transform.position, GetTargetPos(true));
					_diveTime = -1;
					//go a enough to the right or left side to avoid it
					float width = new Vector2(_levelManager.PlayerBase.Size.x + _fighter.Size.x,
											_levelManager.PlayerBase.Size.z + _fighter.Size.z).magnitude;//we are veering laterally
					float angle = Mathf.Atan2(width, distanceToTarget);
					//use a portion of the true distance to give us extra room
					//`Mat TODO: Take rotate speed into account
					Vector3 pos = GetTargetPos(false) + (transform.forward * (_orbitDistance * 0.5f));
					pos.y = _startPos.y;

					//rotate pos by angle
					Quaternion axis = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);
					Quaternion q = Quaternion.LookRotation(pos, this.transform.up) * axis;
					//need to figure out a point on the sphere of orbit
					_escapeRotation = q;
					break;
			}
		}

		//1) [Optional] Spread out in a fan
		//2) Move towards target
		//3) [Optional] Orbit around target
		public IEnumerator UpdatePattern() {
			while (true) {
				Quaternion lookRotation = this.transform.rotation;
				float rotAngle = 0.0f;
				Vector3 deltaPosition = GetTargetPos(true);
				Quaternion look = Quaternion.LookRotation(deltaPosition, Vector3.up);
				float distanceToTarget = Vector3.Distance(this.transform.position, deltaPosition);
				switch (_currentPhase) {
					case eBasicFighterPatternPhase.Entry:
						//fly along what we already decided unti we go the distance
						if (DistanceFlown > _spreadDistance) {
							lookRotation = Quaternion.LookRotation(GetTargetPos(true), this.transform.up);
						}
						break;
					case eBasicFighterPatternPhase.OrbitTarget:
						//go perpendicular to the the right or left
						Quaternion left = look * Quaternion.Euler(0.0f, -90.0f, 0.0f);
						Quaternion right = look * Quaternion.Euler(0.0f, 90.0f, 0.0f);
						rotAngle = Mathf.Min(_flightSpeed * _rotationSpeedScale, 90.0f);
						//pick the closer one
						if (Quaternion.Angle(transform.rotation, left) < Quaternion.Angle(transform.rotation, right)) {
							rotAngle = -rotAngle;
							look = left;
						} else {
							look = right;
						}
						this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
						break;
					case eBasicFighterPatternPhase.Dive:
						lookRotation = Quaternion.LookRotation(GetTargetPos(false), this.transform.up);
						//`Mat magic nuber! Just needs to be pretty close
						//once we are facing the base, start shooting
						if (Quaternion.Angle(this.transform.rotation, lookRotation) < 2.0f) {
							TryShootMain();
						}
						break;
					case eBasicFighterPatternPhase.Disengage:
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
			Debug.Log("TryShoot");
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