using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {

    /// <summary>
    /// Ö÷½Ç
    /// </summary>
    public class PlayerCharacter : CharacterData {
        public PlayerCharacter(ref CharaProfTemplate template,ref int level,List<RelationData>initialRelation=null):base(ref template,ref level,initialRelation) {

        }
    }
}