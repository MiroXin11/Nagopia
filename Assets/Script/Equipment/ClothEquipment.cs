using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class ClothEquipment : Equipment {
        public ClothEquipment(ref ProfPair[] profPairs, Sprite sprite, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM, string name = "")
            : base(ref profPairs, sprite, rarity,name) {
            this.equipmentType = GameDataBase.EquipmentType.CLOTH;
        }
    }
}

