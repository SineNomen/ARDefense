using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using UnityEngine.UI;
using TMPro;

//may need a pause menu
namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(CanvasGroup))]
	public class PlayerHUD : MonoBehaviour, IPlayerHUD {
		[SerializeField]
		private TMP_Text _scoreText = null;
		[SerializeField]
		private Toggle[] _weaponToggles = null;

		private CanvasGroup _canvasGroup;

		[AutoInject]
		private ILevelManager _levelManager = null;
		[AutoInject]
		private IPlayer _player = null;

		private void Awake() {
			Container.Register<IPlayerHUD>(this).AsSingleton();
			_canvasGroup = GetComponent<CanvasGroup>();
			HideInstant();
		}

		private void Start() {
			Container.AutoInject(this);
			SetScore(_levelManager.CurrentScore);
			_levelManager.OnLevelStarted += OnLevelStarted;
			_levelManager.OnEnemyKilled += OnEnemyKilled;

			for (int i = 0; i < _weaponToggles.Length; i++) {
				int index = i;
				_weaponToggles[index].onValueChanged.AddListener((isOn) => {
					if (isOn) {
						OnWeaponToggle(index);
					}
				});
			}
		}

		private void OnWeaponToggle(int i) {
			_player.SetCurrentWeapon(i);
		}

		private void OnLevelStarted() {
			//setup the weapon toggles
			for (int i = 0; i < _weaponToggles.Length; i++) {
				if (i < _player.WeaponList.Capacity) {
					_weaponToggles[i].gameObject.SetActive(true);
					Transform parent = _weaponToggles[i].transform;
					GameObject prefab = _player.WeaponList[i].UIIcon;
					GameObject obj = Instantiate(prefab, parent);
					RectTransform rect = obj.transform as RectTransform;
					rect.anchoredPosition = Vector3.zero;
				} else {
					_weaponToggles[i].gameObject.SetActive(false);
				}
			}
			OnWeaponToggle(0);
		}

		private void OnEnemyKilled(GameObject go) {
			SetScore(_levelManager.CurrentScore);
		}

		public void ShowInstant() { _canvasGroup.ShowInstant(); }
		public void HideInstant() { _canvasGroup.HideInstant(); }

		public IPromise Show() { return _canvasGroup.Show(0.1f); }
		public IPromise Hide() { return _canvasGroup.Hide(0.1f); }

		private void SetScore(int score) {
			_scoreText.text = score.ToString("N0");
		}
	}
}