
namespace Sojourn.ARDefense.Interfaces {
	public enum eKillableTeam {
		Player1,
		BadGuys
	}

	interface IKillable {
		eKillableTeam Team { get; }
		int CurrentHealth { get; }
		int MaxHealth { get; }
	}
}