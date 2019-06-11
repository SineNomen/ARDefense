using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using UnityEngine.UI;
using Sojourn.ARDefense.Data;
using TMPro;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(CanvasGroup))]
	public class MainMenu : MonoBehaviour, IMainMenu {
		[SerializeField]
		private Button _newGameButton = null;
		[SerializeField]
		private TMP_Text _highScoreText = null;

		private CanvasGroup _canvasGroup;

		[AutoInject]
		private ILevelManager _levelManager = null;
		[AutoInject]
		private IPersistentDataManager _saveDataManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;

		private void Awake() {
			Container.Register<IMainMenu>(this).AsSingleton();
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start() {
			Container.AutoInject(this);
			SetScore(0);
			_newGameButton.onClick.AddListener(OnNewGame);
		}

		private void OnNewGame() {
			Debug.Log("Starting new game");
			string desc = "Defend your base against the enemies.\n\nDestroy the dropships before they launch their attacks!";
			Utilities.PromiseGroup(
				Hide(),
				_levelManager.SetupLevel()
			)
			.Chain<string, string, string>(_displayManager.ShowOKModal, "New Game", desc, "GO")
			.Then(_levelManager.StartLevel);
		}

		public void ShowInstant() {
			OnShow();
			_canvasGroup.ShowInstant();
		}

		public void HideInstant() { _canvasGroup.HideInstant(); }

		public IPromise Show() {
			OnShow();
			return _canvasGroup.Show(0.1f);
		}

		private void OnShow() {
			LevelSaveData data = _saveDataManager.LoadData<LevelSaveData>("TestLevelData");
			SetScore(data.HighScore);
		}

		public IPromise Hide() { return _canvasGroup.Hide(0.1f); }

		private void SetScore(int score) {
			_highScoreText.text = score.ToString("N0");
		}
	}
}