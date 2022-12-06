using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public static class TeamInfo{
        public static void Initialize(List<CharacterData>members=null,List<Equipment>equipments=null) {
            if(ReferenceEquals(null,members)) Members = new List<CharacterData>(GameDataBase.Config.MaxTeamMember);
            else Members = members;
            if (ReferenceEquals(null, equipments)) Equipments = new List<Equipment>(GameDataBase.Config.CarriedMaximumEquipment);
            else Equipments = equipments;
        }

        private static List<CharacterData> Members = new List<CharacterData>();

        private static List<Equipment>Equipments= new List<Equipment>();

        public static bool AddCharacter(CharacterData character) {
            if (Members.Contains(character)||Members.Count>=GameDataBase.Config.MaxTeamMember) {
                return false;
            }
            Members.Add(character);
            return true;
        }

        public static bool RemoveCharacter(CharacterData character) {
            if(character is PlayerCharacter) {
                return false;
            }
            return Members.Remove(character);
        }

        public static bool AddEquipment(Equipment equip) {
            if (Equipments.Contains(equip)||Equipments.Count>=GameDataBase.Config.CarriedMaximumEquipment) {
                return false;
            }
            Equipments.Add(equip);
            return true;
        }

        public static bool RemoveEquipment(Equipment equip) {
            if (Equipments.Count <= 0) {
                return false;
            }
            return Equipments.Remove(equip);
        }

        public static CharacterData[] CharacterDatas { get { return Members.ToArray(); } }

        public static Equipment[] EquipmentDatas { get { return Equipments.ToArray();} }
    }
}
