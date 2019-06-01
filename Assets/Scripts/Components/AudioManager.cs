using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using UnityEngine.Audio;

namespace Sojourn.ARDefense.Components {
	[DisallowMultipleComponent]
	public class AudioManager : MonoBehaviour, IAudioManager {
		[SerializeField]
		private AudioMixerGroup _sfxGroup = null;
		[SerializeField]
		private AudioMixerGroup _musicGroup = null;
		[SerializeField]
		private Transform _soundParent = null;

		public AudioMixerGroup SfxGroup { get => _sfxGroup; }
		public AudioMixerGroup MusicGroup { get => _musicGroup; }
		public Transform SoundParent { get => _soundParent; }

		private void Awake() {
			Container.Register<IAudioManager>(this).AsSingleton();
		}
	}
}