using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class SoundOnKill : SoundOnEvent {
		private void Awake() {
			DestroyAfter = true;
			Tag = "Kill";
		}

		public void OnKill(IKillable us) {
			PlaySound();
		}
	}
}
