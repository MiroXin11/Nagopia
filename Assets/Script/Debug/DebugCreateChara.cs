using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nagopia;
public class DebugCreateChara:MonoBehaviour
{
    [Sirenix.OdinInspector.Button]
    public void TestCreate(GameDataBase.CharacterProfession[]profs,int level=1) {
        datas.Add(CharacterGenerator.GenerateCharacter(profs, level));
    }

    [Sirenix.OdinInspector.ShowInInspector]
    public List<Nagopia.CharacterData> datas = new List<Nagopia.CharacterData>();
}
