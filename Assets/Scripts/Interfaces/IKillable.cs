
namespace Sojourn.ARDefense.Interfaces {
	public enum eKillableTeam {
		Player1,
		BadGuys,
		Switzerland,
	}

	//each killable is resonsible for taking the damage the other guy gives
	public interface IKillable {
		eKillableTeam Team { get; set; }
		int CurrentHealth { get; }
		int MaxHealth { get; }
		int CollisionDamageGiven { get; }

		// void OnDamage(IKillable killable);
	}
}