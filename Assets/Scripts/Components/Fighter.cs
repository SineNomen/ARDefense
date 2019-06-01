namespace Sojourn.ARDefense.Components {
	//curently no dfferent from Enemy, but some patterns are meant for a specific type of enemy
	public class Fighter : SimpleEnemy {
		private static int _createCount = 0;

		protected override void Awake() {
			base.Awake();
			gameObject.name = string.Format("Fighter #{0}", _createCount);
			_createCount++;
		}
	}
}