using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class SoundOnDamage : SoundOnEvent {
		private void Awake() {
			DestroyAfter = true;
			Tag = "Damage";
		}

		public void OnDamage(IKillable us) {
			PlaySound();
		}
	}
}
