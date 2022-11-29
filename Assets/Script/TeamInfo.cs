using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class TeamInfo{
        public static List<CharacterData> Members = new List<CharacterData>();

        public static void RemoveCharacter(CharacterData character) {
            Members.Remove(character);
        }
    }
}
