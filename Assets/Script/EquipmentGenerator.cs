using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.AddressableAssets;
namespace Nagopia {
    public class EquipmentGenerator:MonoBehaviour {

        private void Awake() {
            GameDataBase.Initialize();
        }

        public static Equipment GenerateEquipment(ref EquipmentTemplate template) {
            GenerateEquipment(ref template, out Equipment equip);
            var abilities = template.Abilities;
            foreach (var item in abilities) {
                equip.AddAbility(item.type, System.Convert.ToInt32(item.Value*GameDataBase.GameStage+0.5f));
            }
            var additional = template.AdditionalPair;
            foreach (var item in additional) {
                if (RandomNumberGenerator.Happened(item.rarity)) {
                    rarityPair pair = new rarityPair(item.rarity, System.Convert.ToInt32(item.value * GameDataBase.GameStage+0.5));
                    equip.AddEntry(item.type, ref pair);
                }
            }
            return equip;
        }

        public static Equipment GenerateEquipment(GameDataBase.ItemRarity rarity,GameDataBase.CharacterProfession[]usable,GameDataBase.EquipmentType equipmentType) {
            var template=GameDataBase.GetEquipmentTemplate(rarity, usable, equipmentType);
            GenerateEquipment(ref template, out Equipment equip);
            
            var abilities = template.Abilities;
            foreach (var item in abilities) {
                equip.AddAbility(item.type, System.Convert.ToInt32(item.Value * GameDataBase.GameStage+0.5f));
            }
            var additional = template.AdditionalPair;
            foreach (var item in additional) {
                if (RandomNumberGenerator.Happened(item.rarity)) {
                    rarityPair pair = new rarityPair(item.rarity, System.Convert.ToInt32(item.value*GameDataBase.GameStage+0.5));
                    equip.AddEntry(item.type, ref pair);
                }
            }
            return equip;
        }

        private static void GenerateEquipment(ref EquipmentTemplate template,out Equipment equipment) {
            switch (template.EquipmentType) {
                case GameDataBase.EquipmentType.INVALID:
                    equipment = null;
                    break;
                case GameDataBase.EquipmentType.HEAD:
                    equipment = new HeadEquipment(ref template.requirements, template.ItemRarity);
                    break;
                case GameDataBase.EquipmentType.CLOTH:
                    equipment = new ClothEquipment(ref template.requirements, template.ItemRarity);
                    break;
                case GameDataBase.EquipmentType.SHOES:
                    equipment = new ShoesEquipment(ref template.requirements, template.ItemRarity);
                    break;
                case GameDataBase.EquipmentType.WEAPON:
                    equipment = new WeaponEquipment(ref template.requirements, template.ItemRarity);
                    break;
                default:
                    equipment = null;
                    break;
            }
        }

        [Sirenix.OdinInspector.Button]
        public void CreateEquipment() {
            List<GameDataBase.CharacterProfession> profs = new List<GameDataBase.CharacterProfession>();
            foreach (var item in template.requirements) {
                if (item.available)
                    profs.Add(item.prof);
            }
            var equipment = GenerateEquipment(template.ItemRarity, profs.ToArray(), template.EquipmentType);
            equipments.Add(equipment);
        }

        [Sirenix.OdinInspector.Button]
        public void GetAllTemplate() {
            Addressables.LoadAssetsAsync<EquipmentTemplate>("EquipmentTemplate",null).Completed+=
                (item)=> { this.Templates = item.Result as List<EquipmentTemplate>; };
        }

        [Sirenix.OdinInspector.Button]
        public void ShowAllName() {
            foreach (var item in Templates) {
                Debug.Log(item.name);
            }
        }

        [Sirenix.OdinInspector.ShowInInspector]
        public List<Equipment> equipments = new List<Equipment>();

        [Sirenix.OdinInspector.ShowInInspector,Sirenix.OdinInspector.AssetsOnly]
        public EquipmentTemplate template;

        [Sirenix.OdinInspector.ShowInInspector]
        List<EquipmentTemplate> Templates=new List<EquipmentTemplate>();
    }
}
