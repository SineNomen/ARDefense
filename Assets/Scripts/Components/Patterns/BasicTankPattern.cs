using Sojourn.ARDefense.Interfaces;
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
		private bool _randomSpread = true;
		[SerializeField]
		private float _spreadDistance = 5.0f;
		[SerializeField]
		[Range(0, 180.0f)]
		private float _spreadRange = -90.0f;

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
			if (_randomSpread) {
				rotation *= Quaternion.Euler(0.0f, Random.Range(-_spreadRange, _spreadRange), 0.0f);
			}

			transform.rotation = rotation;
			_startPos = transform.position;
		}

		public IEnumerator UpdatePattern() {
			while (true) {
				if (_randomSpread && DistanceDriven < _spreadDistance) {
					//move towards the first point
				} else {
					Vector3 relativePos = _levelManager.PlayerBase.CenterPosition - this.transform.position;
					Quaternion look = Quaternion.LookRotation(relativePos, this.transform.up);
					this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
					float angle = Quaternion.Angle(look, this.transform.rotation);
				}

				// transform.LookAt(_levelManager.PlayerBase.CenterPosition);
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