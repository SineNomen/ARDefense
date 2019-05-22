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
	public class Player : MonoBehaviour, IKillable {
		[AutoInject]
		private IGameManager _gameManager;
		private Collider _collider;

		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]
		private int _maxHealth = 100;
		private int _currentHealth;


		public eKillableTeam Team { get => _team; }
		public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
		public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

		private void Start() {
			Container.AutoInject(this);
			_currentHealth = _maxHealth;
			_collider = GetComponent<Collider>();
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.LogFormat("Collider Enter: {0}", collision.gameObject);
		}

		private void OnTriggerEnter(Collider collider) {
			Debug.LogFormat("Trigger Enter: {0}", collider.gameObject);
		}
	}
}