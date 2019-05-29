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

/*
Has a list of weapons
	a current weapon
Is damagable
has a team

*/

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(Tank))]
	public class DriveDirectPattern : MonoBehaviour {
		[SerializeField]
		private float _speed = 10.0f;
		[SerializeField]
		private bool _startOnCreate = true;

		[AutoInject]
		private IGameManager _gameManager = null;

		private Tank _tank;
		private Transform _target = null;

		public float Speed { get => _speed; }

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
			// transform.LookAt(_gameManager.Player1Base.CenterPosition);
		}

		public IEnumerator UpdatePattern() {
			while (true) {
				Vector3 relativePos = _gameManager.Player1Base.CenterPosition - this.transform.position;
				Quaternion look = Quaternion.LookRotation(relativePos, this.transform.up);
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, look, _speed * Time.deltaTime);
				float angle = Quaternion.Angle(look, this.transform.rotation);

				// transform.LookAt(_gameManager.Player1Base.CenterPosition);
				_tank.Body.velocity = transform.forward * _speed;
				yield return null;
			}
		}

		[ContextMenu("StartPattern")]
		public void StartPattern() {
			StartCoroutine(UpdatePattern());
		}
	}
}