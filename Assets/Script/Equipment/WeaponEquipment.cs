using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class WeaponEquipment : Equipment {
        public WeaponEquipment(ref ProfPair[] profPairs, Sprite sprite, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM, string name = "") : base(ref profPairs, sprite, rarity, name) {
            this.equipmentType = GameDataBase.EquipmentType.WEAPON;
        }
    }
}

