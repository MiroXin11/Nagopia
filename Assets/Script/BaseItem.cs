using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public abstract class BaseItem {
        public BaseItem(string name) {
            this.name = name;
        }

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

        protected string name;

        public string Name { get { return this.name; } }

        /// <summary>
        /// 获得Item的基本信息
        /// </summary>
        /// <returns>Name:{name},Type:{type},rarity:{rarity}</returns>
        public override string ToString() {
            return $"Name:{name},Type:{type},rarity:{rarity}";
        }
    }
}
