namespace Sojourn.ARDefense.Components {
	public class SoundOnFire : SoundOnEvent {
		private void Awake() {
			DestroyAfter = true;
			Tag = "Fire";
		}

		private void OnFire() {
			PlaySound();
		}
	}
}
