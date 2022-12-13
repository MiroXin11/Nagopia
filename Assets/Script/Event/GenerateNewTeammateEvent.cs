using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Nagopia {
    public class GenerateNewTeammateEvent : BaseEvent {

        /// <summary>
        /// 默认生成一个随机职业
        /// </summary>
        public GenerateNewTeammateEvent() {
            this.eventType = GameDataBase.EventType.NEW_TEAMMATE_GENERATE;
            var templates = GameDataBase.GetAllCharaTemplate();
            int stage = GameDataBase.GameStage;
            var filter = templates.Where((x) => x.CanAppear(stage)).ToList();
            int count=filter.Count();
            if (count > 0) {
                for(int i = 0; i < count; ++i) {
                    var tmp = filter[i];
                    if (RandomNumberGenerator.Happened(tmp.Probability)) {
                        //this.data=new CharacterData(ref tmp,ref stage);
                        this.data=CharacterGenerator.GenerateCharacter(tmp, stage);
                        break;
                    }
                }
                if (ReferenceEquals(this.data, null)) {
                    var template = filter[RandomNumberGenerator.Average_GetRandomNumber(0, count,false)];
                    this.data = new CharacterData(ref template, ref stage);
                }
            }
            else {
                count = templates.Count;
                for(int i = 0; i < count; ++i) {
                    var tmp = templates[i];
                    if (RandomNumberGenerator.Happened(tmp.Probability)) {
                        this.data=new CharacterData(ref tmp,ref stage);
                        break;
                    }
                    if (ReferenceEquals(this.data, null)) {
                        var template = templates[RandomNumberGenerator.Average_GetRandomNumber(0, count,false)];
                        this.data=new CharacterData(ref template,ref stage);
                    }
                }
            }
        }

        public GenerateNewTeammateEvent(CharacterData data) {
            this.data = data;
            this.eventType = GameDataBase.EventType.NEW_TEAMMATE_GENERATE;
        }

        public CharacterData data;

        private static System.Array CharaProfs=System.Enum.GetValues(typeof(GameDataBase.CharacterProfession));
    }
}