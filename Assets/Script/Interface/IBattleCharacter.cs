using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public interface IBattleCharacter : IBattleElement {

        int HP { get; set; }

        uint ATK { get; }

        uint DEF { get;}

        uint SPE { get;}

        float ATB { get; set; }

        int POSITION{ get;}

        void ThinkMove(BattleInfo battleInfo);

        void UnderAttack(AttackEventData attackEvent,out EscapeEvent escape);

        void TeammateUnderAttack(AttackEventData attackEvent);

        uint CalculateAbility();
    }
}