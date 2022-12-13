using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class InGameUIManager : SingletonMonobehaviour<InGameManager> {
        public void Awake() {
            TeamInfo.AddCharacterEvent.AddListener(CharacterAdd);
            TeamInfo.RemoveCharacterEvent.AddListener(CharacterRemove);
            TeamInfo.CharacterAbilityChangeEvent.AddListener(CharacterAbilityChange);
        }

        public void CharacterAdd(CharacterData character) {
            var members = TeamInfo.CharacterDatas;
            int count = members.Length;
            int i;
            for(i=0; i<count; i++) {
                CharaInfos[i].Data= members[i];
                CharaInfos[i].gameObject.SetActive(true);
            }
            for (; i < CharaInfos.Count; ++i) {
                CharaInfos[i].Data= null;
                CharaInfos[i].gameObject.SetActive(false);
            }
        }

        public void CharacterRemove(CharacterData character) {
            var members = TeamInfo.CharacterDatas;
            int count = members.Length;
            int i;
            for(i=0;i<count; ++i) {
                CharaInfos[i].Data= members[i];
                CharaInfos[i].gameObject.SetActive(true);
            }
            for(;i<CharaInfos.Count; ++i) {
                CharaInfos[i].Data = null;
                CharaInfos[i].gameObject.SetActive(false);
            }
        }

        public void CharacterAbilityChange(CharacterData character) {
            foreach (var item in CharaInfos) {
                if (ReferenceEquals(item.Data, character)) {
                    item.SetText();
                    break;
                }
            }
        }

        public void OutputString(string text) {
            infoShower.AddNewInfo(text);
        }

        public OutputInfoManager infoShower;

        public List<CharaInfoShower> CharaInfos=new List<CharaInfoShower>();
    }
}