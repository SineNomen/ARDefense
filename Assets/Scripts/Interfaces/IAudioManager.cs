using UnityEngine;
using UnityEngine.Audio;

namespace Sojourn.ARDefense.Interfaces {
	public interface IAudioManager {
		AudioMixerGroup SfxGroup { get; }
		AudioMixerGroup MusicGroup { get; }
		Transform SoundParent { get; }
		// IPromise PlayUISound();
	}
}