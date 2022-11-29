using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public abstract class Equipment:BaseItem {

        public Equipment(ref ProfPair[]profPairs,Sprite sprite,GameDataBase.ItemRarity rarity=GameDataBase.ItemRarity.RANDOM) {
            foreach (var item in profPairs) {
                if (item.available) {
                    this.permittedProf.Add(item.prof);
                }
            }
            this.type = GameDataBase.ItemType.EQUIPMENT;
            if (rarity == GameDataBase.ItemRarity.RANDOM) {
                RandomRarity = true;
            }
            else {
                this.rarity = rarity;
            }
            this.sprite= sprite;
        }

        public override GameDataBase.ItemType ItemType {
            get { return this.type; }
            set { this.type = GameDataBase.ItemType.EQUIPMENT; }
        }

        public void AddAbility(GameDataBase.AbilityType type,int value) {
            if (!this.abilities.ContainsKey(type)) {
                this.abilities.Add(type,value);
            }
            else {
                this.abilities[type] = value;
            }
        }

        public void RemoveAbility(GameDataBase.AbilityType type) {
            if (this.abilities.ContainsKey(type)) {
                this.abilities.Remove(type);
            }
        }

        public int CalculateAbility(GameDataBase.AbilityType type) {
            int baseAbility;
            abilities.TryGetValue(type, out baseAbility);
            List<rarityPair> pairs;
            if(rarityPairs.TryGetValue(type, out pairs)) {
                foreach (var item in pairs) {
                    baseAbility += item.value;
                }
            }
            return baseAbility;
        }

        public void AddEntry(GameDataBase.AbilityType type,ref rarityPair pair) {
            if (!this.rarityPairs.ContainsKey(type)) {
                List<rarityPair> pairs = new List<rarityPair>();
                pairs.Add(pair);
                rarityPairs.Add(type, pairs);
            }
            else {
                rarityPairs[type].Add(pair);
            }
            Rarity_Total += pair.rarity;
        }

        public void RemoveEntry(GameDataBase.AbilityType type,rarityPair pair) {
            if (!this.rarityPairs.ContainsKey(type)) {
                return;
            }
            if (rarityPairs[type].Remove(pair)) {
                this.Rarity_Total -= pair.rarity;
            }
        }

        public bool ValidateProf(ref GameDataBase.CharacterProfession profession) {
            return permittedProf.Contains(profession);
        }

        private HashSet<GameDataBase.CharacterProfession> permittedProf = new HashSet<GameDataBase.CharacterProfession>();

        public GameDataBase.EquipmentType EquipmentType=>equipmentType;

        protected GameDataBase.EquipmentType equipmentType;

        private Dictionary<GameDataBase.AbilityType,int> abilities=new Dictionary<GameDataBase.AbilityType, int>();

        public ushort Rarity_Total {
            get { return this.rarity_Total; }
            set {
                if (RandomRarity) {
                    this.rarity_Total = value;
                    if (this.rarity_Total <= 3) {
                        this.rarity = GameDataBase.ItemRarity.COMMON;
                    }
                    else if (this.rarity_Total <= 8) {
                        this.rarity = GameDataBase.ItemRarity.GREAT;
                    }
                    else if (this.rarity_Total <= 20) {
                        this.rarity = GameDataBase.ItemRarity.PRECIOUS;
                    }
                    else if (this.rarity_Total <= 50) {
                        this.rarity = GameDataBase.ItemRarity.EPIC;
                    }
                    else {
                        this.rarity = GameDataBase.ItemRarity.LEGENDARY;
                    }
                }
            }
        }

        public Dictionary<GameDataBase.AbilityType, List<rarityPair>> rarityPairs = new Dictionary<GameDataBase.AbilityType, List<rarityPair>>();

        private bool RandomRarity = false;

        /// <summary>
        /// 装备总的稀有度，会因为词条的附加而提升
        /// </summary>
        public ushort rarity_Total;

        public Sprite sprite;
    }
    public class rarityPair {
        public rarityPair(byte rarity,int value) {
            this.rarity = rarity;
            this.value = value;
        }
        public byte rarity;

        public int value;
    }
}
