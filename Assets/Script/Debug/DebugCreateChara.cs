using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nagopia;
using Sirenix.OdinInspector;

public class DebugCreateChara:MonoBehaviour
{
    [Button]
    public void TestCreate(GameDataBase.CharacterProfession[]profs,int level=1) {
        datas.Add(CharacterGenerator.GenerateCharacter(profs, level));
    }

    [Button]
    public void ShowCharacterData() {
        foreach (var item in datas) {
            Debug.Log(item.ToString());
        }
    }

    [Button]
    public void Test() {
        StartCoroutine(BattleStartCoroutine());
    }

    IEnumerator BattleStartCoroutine() {
        var chara1 = CharacterGenerator.GenerateCharacter("PriestTemplate", 1);
        var chara2 = CharacterGenerator.GenerateCharacter("KnightTemplate", 1);
        var chara3 = CharacterGenerator.GenerateCharacter("RangerTemplate", 1);
        datas.Add(chara1);
        datas.Add(chara2);
        datas.Add(chara3);

        var enemy1 = EnemyGenerator.GenerateEnemy("Test", 1);
        var enemy2 = EnemyGenerator.GenerateEnemy("Test", 1);
        var enemy3 = EnemyGenerator.GenerateEnemy("Test", 1);

        List<IBattleCharacter> player = new List<IBattleCharacter>();
        List<IBattleCharacter>enemy= new List<IBattleCharacter>();
        BattleCharacter bc1 = new BattleCharacter(chara1,player,enemy);
        BattleCharacter bc2=new BattleCharacter(chara2,player,enemy);
        BattleCharacter bc3 = new BattleCharacter(chara3,player, enemy);
        player.Add(bc1);player.Add(bc2);player.Add(bc3);

        EnemyBattleCharacter ebc1 = new EnemyBattleCharacter(enemy1, enemy, player);
        EnemyBattleCharacter ebc2=new EnemyBattleCharacter(enemy2, enemy, player);
        EnemyBattleCharacter ebc3=new EnemyBattleCharacter(enemy3, enemy, player);
        enemy.Add(ebc1);enemy.Add(ebc2);enemy.Add(ebc3);
        BattleStartEvent battleStart=new BattleStartEvent(player,enemy);
        SingletonMonobehaviour<EventHandler>.Instance.StartBattle(battleStart, () => { Debug.Log("fight"); });
        yield break;
    }

    [Sirenix.OdinInspector.ShowInInspector]
    public List<Nagopia.CharacterData> datas = new List<Nagopia.CharacterData>();
}
