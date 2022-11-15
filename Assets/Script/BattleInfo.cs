using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class BattleInfo{
        public BattleInfo(List<IBattleCharacter> teamPos=null,List<IBattleCharacter> teamSpe = null,List<IBattleCharacter>enePos=null,List<IBattleCharacter>eneSpe=null) {
            teammate_sortByPos = new List<IBattleCharacter>(teamPos);
            teammate_sortBySPE = new List<IBattleCharacter>(teamSpe);
            enemy_sortByPos = new List<IBattleCharacter>(enePos);
            enemy_sortBySPE = new List<IBattleCharacter>(eneSpe);
        }

        public List<IBattleCharacter> teammate_sortByPos;

        public List<IBattleCharacter> teammate_sortBySPE;

        public List<IBattleCharacter> enemy_sortByPos;

        public List<IBattleCharacter> enemy_sortBySPE;
    }
}

