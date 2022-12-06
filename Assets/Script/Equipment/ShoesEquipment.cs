using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class ShoesEquipment : Equipment {
        public ShoesEquipment(ref ProfPair[] profPairs, Sprite sprite, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM, string name = "") : base(ref profPairs, sprite, rarity, name) {
            this.equipmentType = GameDataBase.EquipmentType.SHOES;
        }
    }
}

