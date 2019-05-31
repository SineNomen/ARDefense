using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public delegate void BaseEvent(Base b);

	[RequireComponent(typeof(SimpleKillable))]
	[RequireComponent(typeof(Rigidbody))]
	public class Base : MonoBehaviour {
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;

		public eKillableTeam Team { get => _team; set => _team = value; }
		public Rigidbody Body { get; private set; }
		public Transform Transform { get => this.transform; }
		public Vector3 CenterPosition { get => this.transform.position + Vector3.up * 0.5f; }
		public BaseEvent OnBaseKilled { get; set; }

		private void Start() {
			Container.AutoInject(this);
			Body = GetComponent<Rigidbody>();
		}

		private void OnKill(IKillable us) {
			if (OnBaseKilled != null) { OnBaseKilled(this); }
		}
	}
}