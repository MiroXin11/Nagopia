using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class BattleInfo{
        public BattleInfo(List<IBattleCharacter> teamPos=null,List<IBattleCharacter> teamSpe = null,List<IBattleCharacter>enePos=null,List<IBattleCharacter>eneSpe=null) {
            teammate_sortByPos = teamPos;
            teammate_sortBySPE = teamSpe;
            enemy_sortByPos = enePos;
            enemy_sortBySPE = eneSpe;
        }

        public List<IBattleCharacter> teammate_sortByPos;

        public List<IBattleCharacter> teammate_sortBySPE;

        public List<IBattleCharacter> enemy_sortByPos;

        public List<IBattleCharacter> enemy_sortBySPE;
    }
}

