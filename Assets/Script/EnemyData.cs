using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EnemyData
    {
        public EnemyData(EnemyTemplate template) {
            this.Position = template.Position;
            this.rank = template.rank;
            this.duty = template.duty;
            GenerateAbility(ref template);
            GeneratetMind();
        }
        public int Position;

        public int currentHP;

        public int MaxHP;

        public uint ATK;

        public uint DEF;

        public uint SPE;

        public int level;

        public byte LEA;

        public byte COO;

        public byte CAL;

        public byte MOR;

        public GameDataBase.EnemyRarity rank;

        public GameDataBase.EnemyDuty duty;

        public string name;

        private void GenerateAbility(ref EnemyTemplate template)
        {
            this.MaxHP = System.Convert.ToInt32(template.HP_Rate * template.BasisHP * level + template.BasisHP);
            this.ATK = System.Convert.ToUInt32(template.ATK_Rate * template.BasisATK * level + template.BasisATK);
            this.DEF = System.Convert.ToUInt32(template.DEF_Rate * template.BasisDEF * level + template.BasisDEF);
            this.SPE = System.Convert.ToUInt32(template.SPE_Rate * template.BasisSPE * level + template.BasisSPE);
            this.currentHP = this.MaxHP;
        }

        private void GeneratetMind()
        {
            byte minMental = GameDataBase.Config.MinMental, maxMental = GameDataBase.Config.MaxMental;
            this.LEA = RandomNumberGenerator.Average_GetRandomNumber(minMental,maxMental);
            this.CAL = RandomNumberGenerator.Average_GetRandomNumber(minMental,maxMental);
            this.COO = RandomNumberGenerator.Average_GetRandomNumber(minMental, maxMental);
            this.MOR = RandomNumberGenerator.Average_GetRandomNumber(minMental, maxMental);
        }
    }
}