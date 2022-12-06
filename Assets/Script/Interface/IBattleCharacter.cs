using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public interface IBattleCharacter : IBattleElement {

        int HP { get; set; }

        uint MaxHP { get; }

        uint ATK { get; }

        uint DEF { get;}

        uint SPE { get;}

        float ATB { get; set; }

        int POSITION{ get;}

        bool Attacker { get; }

        bool Curer { get; }

        Sprite HeadImage { get; }

        GameObject avatar { get; }

        CharacterAnimatorController animatorController { get; }

        void ThinkMove(BattleInfo battleInfo,System.Action thinkFinishedCallback);

        void UnderAttack(AttackEventData attackEvent,out EscapeEvent escape);

        bool TeammateUnderAttack(AttackEventData attackEvent);

        uint CalculateAbility();
    }
}