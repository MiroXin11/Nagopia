using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EnemyBattleCharacter:IBattleCharacter
    {
        public int HP { get => data.currentHP; set { data.currentHP = Mathf.Clamp(data.currentHP, 0, data.MaxHP); } }

        public uint ATK => data.ATK;

        public uint DEF => data.DEF;

        public uint SPE => data.SPE;

        public float ATB { get => this.atb; 
            set {
                this.atb = Mathf.Clamp(this.atb, 0, GameDataBase.Config.MovedRequireATB);
            } }

        public int POSITION { get => data.Position; }

        public string Name { get => data.name; }

        void IBattleCharacter.ThinkMove(BattleInfo battleInfo)
        {
            switch (this.data.duty)
            {
                case GameDataBase.EnemyDuty.ATTACKER:
                    NormalAttackerAttack(ref battleInfo);
                    break;
                case GameDataBase.EnemyDuty.CURE:
                    CureAttack(ref battleInfo);
                    break;
                default:
                    break;
            }
        }

        private void NormalAttackerAttack(ref BattleInfo battleInfo)
        {
            var target = battleInfo.enemy_sortByPos[0];
        }

        private void CureAttack(ref BattleInfo battleInfo)
        {
            var teammates = battleInfo.teammate_sortBySPE;
            teammates.Sort((x, y) => x.HP.CompareTo(y.HP));
            var target = teammates[0];//选择血量最少的友方进行治疗，虽然默认都是最前排
        }

        public float atb;

        public EnemyData data;
    }
}

