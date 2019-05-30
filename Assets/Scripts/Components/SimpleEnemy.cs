using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

/*
Has a list of weapons
	a current weapon
Is damagable
has a team

*/

namespace Sojourn.ARDefense.Components {
	// [RequireComponent(typeof(SimpleKillable))]
	// [RequireComponent(typeof(Rigidbody))]
	public class SimpleEnemy : MonoBehaviour {
		[SerializeField]

		public eKillableTeam Team { get => _killable.Team; }
		public Rigidbody Body { get; private set; }
		public Transform Transform { get => this.transform; }

		[AutoInject]
		private IGameManager _gameManager = null;
		private IKillable _killable = null;
		private static int _enemyCount = 0;

		protected virtual void Awake() {
			Body = GetComponent<Rigidbody>();
			_killable = GetComponent<IKillable>();
			gameObject.name = string.Format("Enemy #{0}", _enemyCount);
			_enemyCount++;
		}

		protected virtual void Start() {
			Container.AutoInject(this);
			_gameManager.RegisterEnemy(this.gameObject);
		}

		// public void OnDamaged(IKillable us) { }
		//`Mat need to only kill once
		public void OnKill(IKillable us) {
			Debug.LogFormat("Enemy {0} has been killed", this.gameObject.name);
			_gameManager.UnregisterEnemy(this.gameObject);
			Destroy(this.gameObject);
		}
	}
}