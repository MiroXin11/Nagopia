using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class AttackEventData : BaseEvent
    {
        public AttackEventData(ref IBattleCharacter attacker,ref IBattleCharacter target,int damage,ref BattleInfo battleInfo,GameDataBase.AttackAnimationType animationType=GameDataBase.AttackAnimationType.CLOSE)
        {
            this.attacker = attacker;
            this.target = target;
            this.Damage= damage;
            this.battleInfo= battleInfo;
            this.animationType= animationType;
            this.eventType = GameDataBase.EventType.CHARACTER_ATTACK;
        }

        public IBattleCharacter attacker;

        public GameDataBase.AttackAnimationType animationType;

        public IBattleCharacter target;

        public int Damage;

        public BattleInfo battleInfo;

        public override string ToString()
        {
            return $"{attacker.Name}尝试攻击{target.Name}";
            //return $"{attacker.Name} 攻击 {target.Name},造成伤害 {Damage}点,{target.Name} 剩余 {target.HP}点hp";
        }

    }
}

