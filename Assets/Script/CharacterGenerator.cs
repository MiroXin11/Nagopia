using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public class CharacterGenerator : MonoBehaviour {
        private void Awake() {
            GameDataBase.Initialize();
        }
        
        
        public static CharacterData GenerateCharacter(GameDataBase.CharacterProfession[] required,int level=-1) {
            if (level <0) {
                level = GameDataBase.GameStage;
            }
            var template = GameDataBase.GetCharaTemplate(required);
            CharacterData data = new CharacterData(ref template,ref level);
            foreach (var item in template.AvalibleHead) {//添加头部装备
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var HeadEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetHead(HeadEquipment as HeadEquipment,out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleCloth) {//添加衣服
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var ClothEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetCloth(ClothEquipment as ClothEquipment, out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleShoes) {//添加鞋子
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var ShoesEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetShoes(ShoesEquipment as ShoesEquipment, out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleWeapon) {//添加武器
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var WeaponTemplate = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetWeapon(WeaponTemplate as WeaponEquipment, out var old);
                    break;
                }
            }
            return data;
        } 
    }
}
