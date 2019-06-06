using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	public class SimpleProjectile : MonoBehaviour, IProjectile {
		[SerializeField]
		private Rigidbody _body = null;
		private SimpleKillable _killable = null;

		public Transform Transform { get => this.transform; }
		public Weapon Weapon { get; set; }
		public Rigidbody Body { get => _body; }

		private void Awake() {
			_killable = GetComponent<SimpleKillable>();
		}

		// private void Start() {
		// }

		public void OnFire() {
			Invoke("Destroy", Weapon.ProjectileLifetime);
		}

		//we may have been destroyed already
		private void Destroy() {
			if (this != null && this.gameObject != null) {
				Destroy(this.gameObject);
			}
		}

		public void OnDamaged(IKillable us) { }
	}
}