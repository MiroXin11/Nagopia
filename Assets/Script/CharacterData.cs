using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Nagopia {

    [System.Serializable]
    public class CharacterData {
        public CharacterData(ref CharaProfTemplate template,ref int level,List<RelationData>initialRelation=null) {
            this.profession = template.AdaptProf;
            this.Position = RandomNumberGenerator.Average_GetRandomNumber(template.PossiblePosition.min, template.PossiblePosition.max);
            this.Level = level;
            GenerateAbility(ref template,ref level);
            GenerateMental(ref template);
            obj = GameObject.Instantiate(template.prefab);
            animatorController = obj.GetComponent<CharacterAnimatorController>();
            HeadImage = template.HeadImage;
            if (!ReferenceEquals(initialRelation, null)) {
                foreach (var item in initialRelation) {
                    item.source = this;
                }
                this.relationData.AddRange(initialRelation);
            }
        }

        public string name="";

        public GameObject obj;

        public Sprite HeadImage;

        public CharacterAnimatorController animatorController;

        /// <summary>
        /// 角色的血量成长曲线
        /// </summary>
        private AscentCurve HPCurve { get; set; }

        /// <summary>
        /// 角色的最大血量值
        /// </summary>
        public uint HPMaxValue { get { return HPCurve.GetValue(ref level); } }

        public int CurrentHP { get { return currentHP; } set { currentHP = Mathf.Clamp(value, 0, (int)HPMaxValue); } }

        private int currentHP;

        /// <summary>
        /// 角色的攻击成长曲线
        /// </summary>
        private AscentCurve ATKCurve { get; set; }

        /// <summary>
        /// 角色的攻击数值
        /// </summary>
        public uint ATK { get { return ATKCurve.GetValue(ref level); } }

        /// <summary>
        /// 角色的防御成长曲线
        /// </summary>
        private AscentCurve DEFCurve { get; set; }

        /// <summary>
        /// 角色的防御数值
        /// </summary>
        public uint DEF { get { return DEFCurve.GetValue(ref level); } }

        /// <summary>
        /// 角色的敏捷成长曲线
        /// </summary>
        private AscentCurve SPECurve { get; set; }

        /// <summary>
        /// 角色的速度值
        /// </summary>
        public uint SPE { get { return SPECurve.GetValue(ref level); } }


        public int Position { get { return position; } set { position = Mathf.Clamp(value, GameDataBase.Config.MinPosition, GameDataBase.Config.MaxPosition); } }

        private int position;

        public int Level { get { return level; } set {
                this.level = Mathf.Clamp(value, GameDataBase.Config.MinLevel, GameDataBase.Config.MaxLevel);
            } }

        private int level;

        public GameDataBase.CharacterProfession Profession { get { return profession; } set {
                this.profession = value;
            } }

        private GameDataBase.CharacterProfession profession;

        /// <summary>
        /// 角色的领导能力，默认范围为1到20
        /// </summary>
        public byte LEA { get { return lea; } 
            set {
                byte b = (byte)Mathf.Clamp(value, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
                lea = b;
            } }

        private byte lea;

        /// <summary>
        /// 角色的合作能力，默认范围为1到20
        /// </summary>
        public byte COO { get { return coo; } 
            set {
                byte b = (byte)Mathf.Clamp(value, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
                coo = b;
            } }

        private byte coo;

        /// <summary>
        /// 角色的冷静能力，默认范围为1到20
        /// </summary>
        public byte CAL { get { return cal; }
            set {
                byte b = (byte)Mathf.Clamp(value, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
                cal = b;
            } }

        private byte cal;

        /// <summary>
        /// 角色的道德水平，范围为1到20
        /// </summary>
        public byte MOR { get { return mor; }
            set {
                byte b = (byte)Mathf.Clamp(value, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
                mor = b;
            }  }

        private byte mor;

        public bool SetHead(HeadEquipment equipment,out Equipment oldEquipment) {
            if(ReferenceEquals(equipment,null)||equipment.ValidateProf(ref profession)) {
                oldEquipment = head;
                head = equipment;
                return true;
            }
            oldEquipment = null;
            return false;
        }

        private HeadEquipment head;

        public HeadEquipment Head => this.head;

        public bool SetWeapon(WeaponEquipment equipment,out Equipment oldEquipment) {
            if (ReferenceEquals(equipment, null) || equipment.ValidateProf(ref profession)) {
                oldEquipment = this.weapon;
                this.weapon = equipment;
                return true;
            }
            oldEquipment = null;
            return false;
        }

        private WeaponEquipment weapon;

        public WeaponEquipment Weapon => this.weapon;

        public bool SetCloth(ClothEquipment equipment,out Equipment oldEquipment) {
            if (ReferenceEquals(equipment, null) || equipment.ValidateProf(ref profession)) {
                oldEquipment = this.cloth;
                this.cloth = equipment;
                return true;
            }
            oldEquipment = null;
            return false;
        }

        private ClothEquipment cloth;

        public ClothEquipment Cloth=>this.cloth;

        public bool SetShoes(ShoesEquipment equipment,out Equipment oldEquipment) {
            if (ReferenceEquals(equipment, null) || equipment.ValidateProf(ref profession)) {
                oldEquipment = this.shoes;
                this.shoes = equipment;
                return true;
            }
            oldEquipment = null;
            return false;
        }

        private ShoesEquipment shoes;

        public ShoesEquipment Shoes => this.shoes;

        private static System.Array MentalType = System.Enum.GetValues(typeof(GameDataBase.MentalType));

        private void GenerateAbility(ref CharaProfTemplate template,ref int level) {
            this.HPCurve = new AscentCurve(template.HP,ref level);
            this.CurrentHP = (int)HPMaxValue;
            this.ATKCurve = new AscentCurve(template.ATK,ref level);
            this.DEFCurve = new AscentCurve(template.DEF,ref level);
            this.SPECurve = new AscentCurve(template.SPE,ref level);
        }

        private void GenerateMental(ref CharaProfTemplate template) {
            var baseMental = template.MentalRange;
            MinMaxPair<byte> defaultPair = new MinMaxPair<byte>(GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
            foreach (GameDataBase.MentalType item in MentalType) {
                if (baseMental.TryGetValue(item, out var pair)) {
                    SetMentalValue(item, ref pair);
                }
                else {
                    SetMentalValue(item, ref defaultPair);
                }
            }
            foreach (var item in template.Revises) {
                if (RandomNumberGenerator.Happened(item.probability)) {
                    ReviseMentalValue(item.mental, item.revise);
                }
            }
        }

        private void SetMentalValue(GameDataBase.MentalType type, ref MinMaxPair<byte> pair) {
            switch (type) {
                case GameDataBase.MentalType.CAL: SetMentalValue(ref cal, ref pair); break;
                case GameDataBase.MentalType.COO: SetMentalValue(ref coo, ref pair); break;
                case GameDataBase.MentalType.LEA: SetMentalValue(ref lea, ref pair); break;
                case GameDataBase.MentalType.MOR: SetMentalValue(ref mor, ref pair); break;
            }
        }

        private void ReviseMentalValue(GameDataBase.MentalType type,float revise) {
            switch (type) {
                case GameDataBase.MentalType.LEA:SetMentalValue(ref lea, ref revise);
                    break;
                case GameDataBase.MentalType.COO:SetMentalValue(ref coo, ref revise);
                    break;
                case GameDataBase.MentalType.CAL:SetMentalValue(ref cal, ref revise);
                    break;
                case GameDataBase.MentalType.MOR:SetMentalValue(ref mor, ref revise);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 生成角色时，初始化角色的精神属性的函数，address相当于个指针
        /// </summary>
        /// <param name="address"></param>
        /// <param name="pair"></param>
        private static void SetMentalValue(ref byte address, ref MinMaxPair<byte> pair) {
            byte rng = RandomNumberGenerator.Average_GetRandomNumber(pair.min, pair.max);
            rng = (byte)Mathf.Clamp(rng, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
            address = rng;
        }

        private static void SetMentalValue(ref byte address,ref float revise) {
            float newData = address * revise;
            newData = Mathf.Clamp(newData, GameDataBase.Config.MinMental, GameDataBase.Config.MaxMental);
            address = System.Convert.ToByte(newData);
        }

        public int CalculateEquipmentAbility(GameDataBase.AbilityType type) {
            int equipAbi = 0;
            equipAbi =equipAbi+ (!ReferenceEquals(weapon,null) ? weapon.CalculateAbility(type) : 0);
            equipAbi = equipAbi + (!ReferenceEquals(head, null) ? head.CalculateAbility(type) : 0);
            equipAbi = equipAbi + (!ReferenceEquals(cloth, null) ? cloth.CalculateAbility(type) : 0);
            equipAbi = equipAbi + (!ReferenceEquals(shoes, null) ? shoes.CalculateAbility(type) : 0);
            return equipAbi;
        }

        public int GetRelationData(CharacterData data) {
            var relation = this.relationData.Find((x) => x.target == data);
            return relation != null ? relation.relation : 0;
        }

        private List<RelationData> relationData = new List<RelationData>();

        public override string ToString() {
            string property = $"Name:{name},Prof:{Profession}\n";
            property += $"HP:{CurrentHP}/{HPMaxValue},ATK:{ATK},DEF:{DEF},SPE:{SPE}\n";
            property += $"MOR:{mor},COO:{coo},CAL:{cal}\n";
            string HeadString = head != null ? head.ToString()+"\n" : string.Empty;
            string ClothString=cloth!=null?cloth.ToString()+"\n" : string.Empty;
            string ShoesString=shoes!=null?shoes.ToString()+"\n" : string.Empty;
            string WeaponString=weapon!=null?weapon.ToString()+"\n" : string.Empty;
            string equipment = $"{HeadString}{ClothString}{ShoesString}{WeaponString}";
            return property+equipment;
        }
    }

    /// <summary>
    /// 角色的关系数据
    /// </summary>
    public class RelationData {

        public RelationData(CharacterData source=null,CharacterData target=null,sbyte relation = 0) {
            this.relation = relation;
            this.source = source;
            this.target = target;
        }

        /// <summary>
        /// 起点
        /// </summary>
        public CharacterData source;

        /// <summary>
        /// 终点
        /// </summary>
        public CharacterData target;

        /// <summary>
        /// 关系值，范围由GameConfig中的具体参数决定，数值越大关系越好
        /// 为正数时，二者关系偏正面；为负数时，二者关系偏负面
        /// </summary>
        public sbyte Relation { get => relation; set { this.relation = (sbyte)Mathf.Clamp(value, sbyte.MinValue, sbyte.MaxValue); } }

        public sbyte relation;
    }
}
