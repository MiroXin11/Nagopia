using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public class CharacterGenerator  {
        
        /// <summary>
        /// 该方法只会创建出一个角色
        /// </summary>
        /// <param name="required">可能出现的角色的职业</param>
        /// <param name="level">角色的初始等级，默认为根据当前关卡配置角色等级</param>
        /// <returns></returns>
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

        public static CharacterData GenerateCharacter(CharaProfTemplate template,int level = -1,List<RelationData>relation=null,string name="") {
            CharacterData data = new CharacterData(ref template, ref level, relation, name);
            foreach (var item in template.AvalibleHead) {//添加头部装备
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var HeadEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetHead(HeadEquipment as HeadEquipment, out var old);
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
        
        public static CharacterData GenerateCharacter(string name,int level=-1) {
            if (level == -1) {
                level = GameDataBase.GameStage;
            }
            var template=GameDataBase.GetCharaTemplate(name);
            if (ReferenceEquals(template, null)) {
                return null;
            }
            CharacterData data = new CharacterData(ref template, ref level);
            foreach (var item in template.AvalibleHead) {//添加头部装备
                if (RandomNumberGenerator.Happened(item.probability)) {
                    var eqTemplate = item.Equipment as EquipmentTemplate;
                    var HeadEquipment = EquipmentGenerator.GenerateEquipment(ref eqTemplate);
                    data.SetHead(HeadEquipment as HeadEquipment, out var old);
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
