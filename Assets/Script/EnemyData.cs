using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EnemyData
    {
        public EnemyData(EnemyTemplate template,int level) {
            this.Position = template.Position;
            this.rank = template.rank;
            this.duty = template.duty;
            this.avatar = GameObject.Instantiate(template.prefab,parent);
            this.avatar.transform.localPosition = initialPos;
            this.animatorController = avatar.GetComponent<CharacterAnimatorController>();
            this.level = level;
            this.HeadImage = template.HeadImage;
            this.attackAnimationType = template.attackAnimationType;
            this.expRate = template.exp_rate;
            this.name = template.Name;
            GenerateAbility(ref template);
            GeneratetMind(ref template);
        }
        public int Position;

        public int currentHP;

        public int MaxHP;

        public uint ATK;

        public uint DEF;

        public uint SPE;

        public int level;

        public float expRate;

        /// <summary>
        /// 领导力
        /// </summary>
        public byte LEA;

        /// <summary>
        /// 合作能力
        /// </summary>
        public byte COO;

        /// <summary>
        /// 镇静度
        /// </summary>
        public byte CAL;

        /// <summary>
        /// 道德值
        /// </summary>
        public byte MOR;

        public GameDataBase.EnemyRarity rank;

        public GameDataBase.EnemyDuty duty;

        public GameDataBase.AttackAnimationType attackAnimationType;

        public string name;

        public GameObject avatar;

        public Sprite HeadImage;

        public CharacterAnimatorController animatorController;

        private void GenerateAbility(ref EnemyTemplate template)
        {
            this.MaxHP = System.Convert.ToInt32(template.HP_Rate * template.BasisHP * level + template.BasisHP);
            this.ATK = System.Convert.ToUInt32(template.ATK_Rate * template.BasisATK * level + template.BasisATK);
            this.DEF = System.Convert.ToUInt32(template.DEF_Rate * template.BasisDEF * level + template.BasisDEF);
            this.SPE = System.Convert.ToUInt32(template.SPE_Rate * template.BasisSPE * level + template.BasisSPE);
            this.currentHP = this.MaxHP;
        }

        private void GeneratetMind(ref EnemyTemplate template)
        {
            var dict = template.MentalRange;
            byte minMental = GameDataBase.Config.MinMental;
            byte maxMental = GameDataBase.Config.MaxMental;
            foreach (GameDataBase.MentalType item in mentalArray) {
                if(dict.TryGetValue(item,out var value)) {
                    SetMental(item, RandomNumberGenerator.Average_GetRandomNumber(value.min, value.max));
                }
                else {
                    SetMental(item,RandomNumberGenerator.Average_GetRandomNumber(minMental, maxMental));
                }
            }
        }

        private void SetMental(GameDataBase.MentalType type,byte value) {
            switch (type) {
                case GameDataBase.MentalType.LEA:
                    this.LEA = value;
                    break;
                case GameDataBase.MentalType.COO:
                    this.COO= value;
                    break;
                case GameDataBase.MentalType.CAL:
                    this.CAL = value;
                    break;
                case GameDataBase.MentalType.MOR:
                    this.MOR= value;
                    break;
                default:
                    break;
            }
        }

        private static Array mentalArray = Enum.GetValues(typeof(GameDataBase.MentalType));

        private Transform parent=Camera.main.transform;

        private static Vector3 initialPos = new Vector3(500, 0, 2); 
    }
}