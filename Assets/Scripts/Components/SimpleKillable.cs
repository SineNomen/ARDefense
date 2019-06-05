using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[DisallowMultipleComponent]
	public class SimpleKillable : MonoBehaviour, IKillable {
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]
		private bool _teamDamage = false;
		[SerializeField]
		private int _maxHealth = 1;
		[SerializeField]
		private int _collisionDamageGiven = 1;

		private int _currentHealth;

		public eKillableTeam Team { get => _team; set => _team = value; }
		public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
		public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
		public int CollisionDamageGiven { get => _collisionDamageGiven; set => _collisionDamageGiven = value; }
		public bool TeamDamage { get => _teamDamage; }

		private void Start() {
			CurrentHealth = MaxHealth;
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.LogFormat("{0} Collided with {1}", this.gameObject, collision.gameObject);
			OnHit(collision.gameObject);
		}

		private void OnTriggerEnter(Collider collider) {
			Debug.LogFormat("{0} Hit {1}", this.gameObject, collider.gameObject);
			OnHit(collider.gameObject);
		}

		public void OnHit(GameObject go) {
			IKillable killable = go.GetComponent<IKillable>();
			if (killable != null) {
				bool doDamage = TeamDamage || killable.Team != Team;
				if (doDamage && killable.CollisionDamageGiven > 0) {
					_currentHealth -= killable.CollisionDamageGiven;
					if (_currentHealth <= 0) {
						//`Mat Broadcast message
						BroadcastMessage("OnKill", this, SendMessageOptions.RequireReceiver);
					} else {
						//`Mat Broadcast message
						BroadcastMessage("OnDamaged", this, SendMessageOptions.RequireReceiver);
					}
				}
			}
		}
	}
}