using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class SimpleEnemy : MonoBehaviour {
		public eKillableTeam Team { get => _killable.Team; }
		public Rigidbody Body { get; private set; }
		public Transform Transform { get => this.transform; }

		[AutoInject]
		protected IGameManager _gameManager = null;
		[AutoInject]
		protected ILevelManager _levelManager = null;

		private IKillable _killable = null;
		private static int _enemyCount = 0;

		protected virtual void Awake() {
			Body = GetComponent<Rigidbody>();
			_killable = GetComponent<IKillable>();
			// gameObject.name = string.Format("Enemy #{0}", _enemyCount);
			_enemyCount++;
		}

		protected virtual void Start() {
			Container.AutoInject(this);
			_levelManager.RegisterEnemy(this.gameObject);
		}

		// public void OnDamaged(IKillable us) { }
		public void OnKill(IKillable us) {
			Debug.LogFormat("Enemy {0} has been killed", this.gameObject.name);
			_levelManager.UnregisterEnemy(this.gameObject);
		}
	}
}