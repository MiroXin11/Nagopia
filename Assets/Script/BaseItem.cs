using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public abstract class BaseItem {
        public virtual GameDataBase.ItemRarity ItemRarity {
            get { return this.rarity; }
            protected set {
                if (value != GameDataBase.ItemRarity.RANDOM)
                    this.rarity = value;
                }
        }

        protected GameDataBase.ItemRarity rarity;

        public virtual GameDataBase.ItemType ItemType {
            get { return this.type; }
            set { this.type = value; }
        }

        protected GameDataBase.ItemType type;
    }
}
