using Nagopia;
namespace BehaviorDesigner.Runtime {
    [System.Serializable]
    public class SharedBattleCharacter : SharedVariable<BattleCharacter> {
        public static implicit operator SharedBattleCharacter(BattleCharacter character) {
            return new SharedBattleCharacter { mValue = character };
        }
    }
}

