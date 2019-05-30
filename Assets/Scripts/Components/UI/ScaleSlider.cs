using Sojourn.PicnicIOC;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;

namespace Sojourn.ARDefense.Components {
	public class ScaleSlider : MonoBehaviour {
		[SerializeField]
		private Slider _slider = null;
		[SerializeField]
		private TMP_Text _percentText = null;
		[SerializeField]
		private ARSessionOrigin _origin = null;


		private void Start() {
			Container.AutoInject(this);
			_slider.onValueChanged.AddListener(OnValueChanged);
			_slider.value = _origin.transform.localScale.x;
		}

		private void OnValueChanged(float val) {
			_origin.transform.localScale = Vector3.one * val;
			_percentText.SetText(string.Format("{0:0.00}", val));
		}
	}
}