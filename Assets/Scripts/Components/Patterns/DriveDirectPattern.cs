using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(Tank))]
	public class DriveDirectPattern : MonoBehaviour {
		[SerializeField]
		private float _driveSpeed = 1.5f;
		[SerializeField]
		private float _turnSpeed = 3.0f;
		[SerializeField]
		private bool _startOnCreate = true;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private Tank _tank;
		// private Transform _target = null;

		public float DriveSpeed { get => _driveSpeed; }
		public float TurnSpeed { get => _turnSpeed; }

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
			// transform.LookAt(_levelManager.PlayerBase.CenterPosition);
		}

		public IEnumerator UpdatePattern() {
			while (true) {
				Vector3 relativePos = _levelManager.PlayerBase.CenterPosition - this.transform.position;
				Quaternion look = Quaternion.LookRotation(relativePos, this.transform.up);
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _turnSpeed * Time.deltaTime);
				float angle = Quaternion.Angle(look, this.transform.rotation);

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