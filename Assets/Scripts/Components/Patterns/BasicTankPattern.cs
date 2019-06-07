using Sojourn.ARDefense.Interfaces;
using Sojourn.Utility;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(Tank))]
	public class BasicTankPattern : MonoBehaviour {
		[SerializeField]
		private float _driveSpeed = 1.5f;
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private bool _startOnCreate = true;
		[SerializeField]
		private bool _spreadOnStart = true;
		[SerializeField]
		private RandomFloat _spreadDistance = new RandomFloat(5.0f, 5.0f, 0.0f, 100.0f);
		[SerializeField]
		private RandomFloat _spreadRange = new RandomFloat(-45.0f, 45.0f, -180.0f, 180.0f);

		[SerializeField]
		private bool _orbitTarget = true;
		[SerializeField]
		private float _orbitDistance = 10.0f;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private Tank _tank;
		private Vector3 _startPos;
		private Quaternion _spreadDirection;

		public float DriveSpeed { get => _driveSpeed; }
		public float TurnSpeed { get => _turnSpeed; }
		private float DistanceDriven { get => (transform.position - _startPos).magnitude; }

		private void Awake() {
			_tank = GetComponent<Tank>();
			_spreadDistance.Pick();
		}

		private void Start() {
			Container.AutoInject(this);
			if (_startOnCreate) {
				Setup();
				StartPattern();
			}
		}

		public void Setup() {
			Vector3 relativePos = _levelManager.PlayerBase.CenterPosition - this.transform.position;
			Quaternion look = Quaternion.LookRotation(relativePos, this.transform.up);
			Quaternion rotation = look;
			if (_spreadOnStart) {
				rotation *= Quaternion.Euler(0.0f, _spreadRange.Pick(), 0.0f);
			}

			transform.rotation = rotation;
			_startPos = transform.position;
		}

		//1) [Optional] Spread out in a fan
		//2) Move towards target
		//3) [Optional] Orbit around target
		public IEnumerator UpdatePattern() {
			while (true) {
				Vector3 deltaPosition = _levelManager.PlayerBase.CenterPosition - this.transform.position;
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

				_tank.Body.velocity = transform.forward * _driveSpeed;
				yield return null;
			}
		}

		[ContextMenu("StartPattern")]
		public void StartPattern() {
			StartCoroutine(UpdatePattern());
		}
	}
}