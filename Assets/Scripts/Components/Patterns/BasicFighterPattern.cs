using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
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
		[Range(0, 180.0f)]
		private float _spreadRange = -90.0f;

		[SerializeField]
		private bool _orbitTarget = true;
		[SerializeField]
		private float _orbitDistance = 10.0f;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private Fighter _fighter;
		private Vector3 _startPos;
		private Quaternion _spreadDirection;

		public float FlightSpeed { get => _flightSpeed; }
		public float TurnSpeed { get => _turnSpeed; }
		private float DistanceDriven { get => (transform.position - _startPos).magnitude; }

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
			Vector3 relativePos = GetTargetPos(true);
			Quaternion look = Quaternion.LookRotation(relativePos, this.transform.up);
			Quaternion rotation = look;
			if (_spreadOnStart) {
				rotation *= Quaternion.Euler(0.0f, Random.Range(-_spreadRange, _spreadRange), 0.0f);
			}

			transform.rotation = rotation;
			_startPos = transform.position;
		}

		private Vector3 GetTargetPos(bool keepHeight) {
			if (keepHeight) {
				Vector3 pos = _levelManager.PlayerBase.CenterPosition;
				pos.y = this.transform.position.y;
				return pos - this.transform.position;
			} else {
				return _levelManager.PlayerBase.CenterPosition - this.transform.position;
			}
		}

		//1) [Optional] Spread out in a fan
		//2) Move towards target
		//3) [Optional] Orbit around target
		public IEnumerator UpdatePattern() {
			while (true) {
				Vector3 deltaPosition = GetTargetPos(true);
				Quaternion look = Quaternion.LookRotation(deltaPosition, this.transform.up);

				if (_spreadOnStart && DistanceDriven < _spreadDistance) {
					//move towards the first point
				} else if (_orbitTarget && deltaPosition.magnitude < _orbitDistance) {
					//go perpendicular to the the right or left
					Quaternion left = look * Quaternion.Euler(0.0f, 90.0f, 0.0f);
					Quaternion right = look * Quaternion.Euler(0.0f, -90.0f, 0.0f);
					//pick the closer one
					look = (Quaternion.Angle(transform.rotation, left) < Quaternion.Angle(transform.rotation, right) ? left : right);
					this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
				} else {
					//go straight toward the target
					this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
				}

				_fighter.Body.velocity = transform.forward * _flightSpeed;
				yield return null;
			}
		}

		[ContextMenu("StartPattern")]
		public void StartPattern() {
			StartCoroutine(UpdatePattern());
		}
	}
}