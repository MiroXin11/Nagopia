using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public static class EnemyGenerator {

        /// <summary>
        /// 生成一个敌人数据
        /// </summary>
        /// <param name="name">模板的名字</param>
        /// <param name="level">生成的角色的等级</param>
        /// <returns></returns>
        public static EnemyData GenerateEnemy(string name,int level=-1) {
            if (level < 0) {
                level = GameDataBase.GameStage;
            }
            EnemyData enemyData;
            EnemyTemplate template;
            if(!cache.TryGetValue(name, out template)) {
                template=GameDataBase.GetEnemyTemplate(name);
                cache.Add(name, template);
            }
            enemyData=new EnemyData(template, level);
            return enemyData;
        }
        private static Dictionary<string,EnemyTemplate>cache=new Dictionary<string,EnemyTemplate>();
    }
}