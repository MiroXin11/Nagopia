using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Nagopia {
    public class TestBattleSequence:MonoBehaviour {

        [Button]
        public void GetTemplate(string enemyName="Test") {
            template = GameDataBase.GetEnemyTemplate(enemyName);
        }

        public EnemyTemplate template;

        [Button]
        public void GenerateBattleCharacter() {
            List<IBattleCharacter>player= new List<IBattleCharacter>();
            List<IBattleCharacter>enemy=new List<IBattleCharacter>();
            for(int i = 0; i < 3; ++i) {
                enemyBattleCharacters.Add(new EnemyBattleCharacter(new EnemyData(template,1), enemy, player));//生成敌人
            }
            for(int i = 0; i < 3; ++i) {
                playerBattleCharacter.Add(new EnemyBattleCharacter(new EnemyData(template,1), player, enemy));//生成友军
            }
        }

        [Button]
        public void StartBattle() {
            SingletonMonobehaviour<BattleManager>.Instance.StartBattle(playerBattleCharacter, enemyBattleCharacters);
        }

        public List<IBattleCharacter>enemyBattleCharacters= new List<IBattleCharacter>();

        public List<IBattleCharacter>playerBattleCharacter=new List<IBattleCharacter>();

        [Button]
        public void TestBattleManagerExistence() {
            Debug.Log($"{SingletonMonobehaviour<BattleManager>.Instance==null}");
        }

        [Button]
        public void TestEventHandleExistence() {
            Debug.Log($"{SingletonMonobehaviour<EventHandler>.Instance==null}");
        }
    }
}