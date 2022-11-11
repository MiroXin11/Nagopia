using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
namespace Nagopia {

    public abstract class EquipmentTemplate : BaseItemTemplate {

        public EquipmentTemplate() {
            this.itemType = GameDataBase.ItemType.EQUIPMENT;
            var profs = System.Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            int length = profs.Length;
            requirements = new ProfPair[length];
            for (int i = 0; i < length; ++i) {
                requirements[i].prof = (GameDataBase.CharacterProfession)profs.GetValue(i);
                requirements[i].available = false;
            }
        }

        public override bool ValidateProf(GameDataBase.CharacterProfession type) {
            return requirements[(int)type].available;
        }

        [HideInInspector]
        public GameDataBase.EquipmentType EquipmentType { get { return this.equipmentType; } }

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [DisplayAsString]
        [BoxGroup("Equipment",showLabel:false)]
        [LabelText("װ������")]
        [Space()]
        protected GameDataBase.EquipmentType equipmentType = GameDataBase.EquipmentType.INVALID;

        [HideInInspector]
        public EquipmentAbility[] Abilities { get { return this.abilities.ToArray(); } }

        [System.NonSerialized,OdinSerialize,ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("��������ֵ")]
        [Space()]
        private List<EquipmentAbility> abilities=new List<EquipmentAbility>();

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("���õ�ְҵ")]
        [Space()]
        [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        public ProfPair[] requirements;

        [HideInInspector]
        public RarityPair_Template[] AdditionalPair { get { return this.additional_abilities.ToArray(); } }

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("���ܳ��ֵ����Լ���������ϡ��")]
        [Tooltip("��ϡ��Խ�ߣ����ֵĸ���ҲԽ�ͣ�����ĸ��ʿ��Բμ�RandomNumberGenerator�е�Happened(byte)")]
        [Space()]
        [ListDrawerSettings(DraggableItems =true)]
        private List<RarityPair_Template> additional_abilities = new List<RarityPair_Template>();

#if UNITY_EDITOR
        public void OnValidate() {

            //���ְҵ�����Ƿ����ı�
            var profs = System.Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            int length = profs.Length;
            int oldLength = this.requirements.Length;
            if (length != oldLength) {
                ProfPair[] newrequirement = new ProfPair[length];
                for(int i = 0; i < length; ++i) {
                    newrequirement[i].prof = (GameDataBase.CharacterProfession)profs.GetValue(i);
                    newrequirement[i].available = i < oldLength ? this.requirements[i].available : false;
                }
                this.requirements = newrequirement;
            }
        }
#endif
    }
    public struct EquipmentAbility {
        public EquipmentAbility(GameDataBase.AbilityType type, short values) {
            this.type = type;
            value = values;
        }

        [LabelText("��ֵ����")]
        [HorizontalGroup("Ability")]
        public readonly GameDataBase.AbilityType type;

        [OdinSerialize,ShowInInspector]
        [LabelText("��ֵ")]
        [HorizontalGroup("Ability")]
        [PropertyTooltip("��׼��ֵ���������Ϊ������Ϸ���ȷ�չ������ֵ�ı仯�����ͬһ��Ϸ�����¸�ֵԽ��������������ֵԽ��")]
        public float Value { get { return this.value; } private set { this.value = value; } }

        [System.NonSerialized,OdinSerialize,HideInInspector]
        private float value;
    }

    public struct RarityPair_Template {

        [LabelText("��������")]
        [HorizontalGroup("Pair")]
        [LabelWidth(50)]
        public GameDataBase.AbilityType type;
        
        [LabelText("ϡ�ж�"),HorizontalGroup("Pair")]
        [Range(0,10)]
        [LabelWidth(50)]
        [Tooltip("����ֵΪ0ʱ���ô����ض������")]
        public byte rarity;

        [LabelText("��ֵ"),HorizontalGroup("Pair")]
        [Indent()]
        [LabelWidth(50)]
        [Tooltip("��׼��ֵ���������Ϊ������Ϸ���ȷ�չ������ֵ�ı仯�����ͬһ��Ϸ�����¸�ֵԽ��������������ֵԽ��")]
        public float value;
    }

    public struct ProfPair {
        [LabelText("ְҵ")]
        [HorizontalGroup("ProfPair")]
        [ReadOnly]
        public GameDataBase.CharacterProfession prof;

        [LabelText("����")]
        [HorizontalGroup("ProfPair")]
        public bool available;
    }
}
