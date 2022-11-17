using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    /// <summary>
    /// 玩家方角色
    /// </summary>
    public class BattleCharacter : IBattleCharacter {
        public int HP { get => data.CurrentHP; set => data.CurrentHP = value; }
        public uint ATK { get => data.ATK; set { } }
        public uint DEF { get => data.DEF; set { } }
        public uint SPE { get => data.SPE; set { } }
        public float ATB { get => atb; 
            set {
                this.atb = Mathf.Clamp(value, 0f, GameDataBase.Config.MovedRequireATB);
            } }
        public int POSITION { get => data.Position; set => data.Position = value; }

        public string Name { get => data.name; }

        public float atb;

        [Sirenix.OdinInspector.ShowInInspector]
        private CharacterData data;

        public void ThinkMove(BattleInfo battleInfo) {
            //var teammate = battleInfo.teammate_sortByPos;
            
        }

        /// <summary>
        /// 逃跑事件的思考
        /// </summary>
        /// <param name="battleInfo"></param>
        private void ConsiderEscape(ref BattleInfo battleInfo) {
            var myIndex = battleInfo.teammate_sortByPos.IndexOf(this);
            if (myIndex == 0) {//代表我是下一次受击的对象
                //GameDataBase g = new GameDataBase();
            }
            else {

            }
        }

        public void UnderAttack(AttackEventData data,out EscapeEvent escape) {
            escape= null;
        }

        public void TeammateUnderAttack(AttackEventData data) {

        }

        private void ConsiderAttack(ref BattleInfo battleInfo) {
            IBattleCharacter target;
            switch (this.data.Profession) {
                case GameDataBase.CharacterProfession.KNIGHT:
                    target = battleInfo.enemy_sortByPos[0];//骑士的攻击选择敌方最近的目标
                    break;
                case GameDataBase.CharacterProfession.RANGER:
                    target = battleInfo.enemy_sortByPos[0];//游侠的攻击选择敌方最近的目标
                    break;
                case GameDataBase.CharacterProfession.PRIEST:
                    var possibleTargets = new List<IBattleCharacter>(battleInfo.teammate_sortByPos);
                    possibleTargets.Sort((x, y) => x.HP.CompareTo(y.HP));
                    
                    break;
                default:
                    break;
            }
        }

        public uint CalculateAbility() {
            return System.Convert.ToUInt32(this.HP + this.ATK * 5 + this.DEF * 5 + this.SPE * 10);
        }

        /// <summary>
        /// 用于判断当前是否处于镇静状态，主要跟当前队伍的血量，以及自身的CAL有关
        /// 当自身血量低于20%或者队伍总血量低于40%时
        /// 或者面对强敌时
        /// </summary>
        /// <param name="battleInfo"></param>
        /// <returns></returns>
        private bool StayCalm(ref BattleInfo battleInfo) {
            return false;
        }
        
    }
}
