
namespace Sojourn.ARDefense.Interfaces {
	public enum eKillableTeam {
		Player1,
		BadGuys,
		// Switzerland,
	}

	//any object that can give or take damage needs to have a killable
	//each killable is resonsible for taking the damage the other guy gives
	public interface IKillable {
		eKillableTeam Team { get; set; }
		bool TeamDamage { get; }
		bool KillOnHitGround { get; }
		int CurrentHealth { get; }
		int MaxHealth { get; }
		int CollisionDamageGiven { get; }

		// void OnDamage(IKillable killable);
	}
}