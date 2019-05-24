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
	public class SimpleKillable : MonoBehaviour, IKillable {
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]
		private int _maxHealth = 1;
		private int _currentHealth;
		[SerializeField]
		private int _collisionDamageGiven = 1;

		public eKillableTeam Team { get => _team; set => _team = value; }
		public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
		public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
		public int CollisionDamageGiven { get => _collisionDamageGiven; set => _collisionDamageGiven = value; }

		private void Start() {
			CurrentHealth = MaxHealth;
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.LogFormat("{0} Collided with {0}", this.gameObject, collision.gameObject);
			OnHit(collision.gameObject);
		}

		private void OnTriggerEnter(Collider collider) {
			Debug.LogFormat("{0} Hit {0}", this.gameObject, collider.gameObject);
			OnHit(collider.gameObject);
		}

		public void OnHit(GameObject go) {
			IKillable killable = go.gameObject.GetComponent<IKillable>();
			if (killable != null && killable.Team != Team) {
				_currentHealth -= killable.CollisionDamageGiven;
				if (_currentHealth <= 0) {
					BroadcastMessage("OnKilled", this, SendMessageOptions.RequireReceiver);
				} else {
					BroadcastMessage("OnDamaged", this, SendMessageOptions.RequireReceiver);
				}
			}
		}
	}
}