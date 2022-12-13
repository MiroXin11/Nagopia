using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nagopia {
    public static class TeamInfo{
        public static void Initialize(List<CharacterData>members=null,List<Equipment>equipments=null) {
            if(ReferenceEquals(null,members)) Members = new List<CharacterData>(GameDataBase.Config.MaxTeamMember);
            else Members = members;
            if (ReferenceEquals(null, equipments)) Equipments = new List<Equipment>(GameDataBase.Config.CarriedMaximumEquipment);
            else Equipments = equipments;
            //parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        private static List<CharacterData> Members = new List<CharacterData>();

        private static List<Equipment>Equipments= new List<Equipment>();

        public static bool AddCharacter(CharacterData character) {
            if (Members.Contains(character) || Members.Count>=GameDataBase.Config.MaxTeamMember) {
                return false;
            }
            if (ReferenceEquals(CharacterParent, null)) {
                CharacterParent = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
            Members.Add(character);
            character.onAbilityChange.AddListener(ActAbilityAddEvent);
            character.obj.transform.SetParent(CharacterParent);
            ResetPosition();
            AddCharacterEvent.Invoke(character);
            return true;
        }

        public static bool RemoveCharacter(CharacterData character) {
            if(character is PlayerCharacter) {
                return false;
            }
            if (Members.Remove(character)) {
                character.onAbilityChange.RemoveListener(ActAbilityAddEvent);
                ResetPosition();
                RemoveCharacterEvent.Invoke(character);
                return true;
            }
            return false;
        }

        public static void ResetPosition() {
            Members.Sort((x, y) => { return -x.Position.CompareTo(y.Position); });
            int count = Members.Count;
            for (int i = 0; i < Members.Count; ++i) {
                var data = Members[i];
                data.obj.transform.localPosition = InitialPosition[i];
            }
        }

        public static bool AddEquipment(Equipment equip) {
            if (Equipments.Contains(equip)||Equipments.Count>=GameDataBase.Config.CarriedMaximumEquipment) {
                return false;
            }
            Equipments.Add(equip);
            AddEquipmentEvent.Invoke(equip);
            return true;
        }

        public static bool RemoveEquipment(Equipment equip) {
            if (Equipments.Count <= 0) {
                return false;
            }
            if(Equipments.Remove(equip) ) {
                RemoveEquipmentEvent.Invoke(equip);
                return true;
            }
            return false;
        }

        private static void ActAbilityAddEvent(CharacterData data) {
            CharacterAbilityChangeEvent.Invoke(data);
        }

        public static CharacterData[] CharacterDatas { get { return Members.ToArray(); } }

        public static Equipment[] EquipmentDatas { get { return Equipments.ToArray();} }

        public static UnityEvent<CharacterData> RemoveCharacterEvent=new UnityEvent<CharacterData>();

        public static UnityEvent<CharacterData> AddCharacterEvent=new UnityEvent<CharacterData>();

        public static UnityEvent<Equipment> RemoveEquipmentEvent=new UnityEvent<Equipment>();

        public static UnityEvent<Equipment> AddEquipmentEvent = new UnityEvent<Equipment>();

        public static UnityEvent<CharacterData> CharacterAbilityChangeEvent = new UnityEvent<CharacterData>();

        public static Vector3[] InitialPosition = { new Vector3(-2, -3, 2), new Vector3(-4, -3, 2), new Vector3(-6, -3, 2), new Vector3(-8, -3, 2), new Vector3(-10, -3, 2) };

        public static Transform CharacterParent;
    }
}
