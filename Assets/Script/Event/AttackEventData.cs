using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class AttackEventData : BaseEvent
    {
        public AttackEventData(ref IBattleCharacter attacker,ref IBattleCharacter target,int damage,ref BattleInfo battleInfo)
        {
            this.attacker = attacker;
            this.target = target;
            this.Damage= damage;
            this.battleInfo= battleInfo;
        }

        public IBattleCharacter attacker;

        public IBattleCharacter target;

        public int Damage;

        public BattleInfo battleInfo;

        public override string ToString()
        {
            return $"{attacker.Name} π•ª˜ {target.Name},‘Ï≥……À∫¶ {Damage}µ„";
        }

    }
}

