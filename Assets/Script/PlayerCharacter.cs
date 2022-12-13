using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {

    /// <summary>
    /// 主角
    /// </summary>
    public class PlayerCharacter : CharacterData {
        public PlayerCharacter(ref CharaProfTemplate template,ref int level,List<RelationData>initialRelation=null,string name=""):base(ref template,ref level,initialRelation,name) {
            foreach (var item in template.AvalibleHead) {//添加头部装备
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var HeadEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    this.SetHead(HeadEquipment as HeadEquipment, out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleCloth) {//添加衣服
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var ClothEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    this.SetCloth(ClothEquipment as ClothEquipment, out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleShoes) {//添加鞋子
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var ShoesEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    this.SetShoes(ShoesEquipment as ShoesEquipment, out var old);
                    break;
                }
            }
            foreach (var item in template.AvalibleWeapon) {//添加武器
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var WeaponTemplate = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    this.SetWeapon(WeaponTemplate as WeaponEquipment, out var old);
                    break;
                }
            }
        }
    }
}