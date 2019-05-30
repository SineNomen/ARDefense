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

		private void Awake() {
			Body = GetComponent<Rigidbody>();
			_killable = GetComponent<IKillable>();
			gameObject.name = string.Format("Enemy #{0}", _enemyCount);
			_enemyCount++;
		}

		private void Start() {
			Container.AutoInject(this);
			_gameManager.RegisterEnemy(this.gameObject);
		}

		// public void OnDamaged(IKillable us) { }
		//`Mat need to only kill once
		public void OnKilled(IKillable us) {
			_gameManager.UnregisterEnemy(this.gameObject);
			this.gameObject.name += "- Killed";
			//let any other components run their course, we are gone as far as the game is concerned
			Destroy(Body);
			Invoke("Kill", 5.0f);
		}
		private void Kill() { Destroy(this.gameObject); }
	}
}