using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EnemyBattleCharacter:IBattleCharacter
    {
        public EnemyBattleCharacter(EnemyData data,List<IBattleCharacter>teammates,List<IBattleCharacter>opponent) {
            this.data = data;
            this.teammates = teammates;
            this.opponents = opponent;
        }

        public int HP { get => data.currentHP; set { data.currentHP = Mathf.Clamp(data.currentHP, 0, data.MaxHP); } }

        public uint ATK => data.ATK;

        public uint DEF => data.DEF;

        public uint SPE => data.SPE;

        public float ATB { get => this.atb; 
            set {
                this.atb = Mathf.Clamp(this.atb, 0, GameDataBase.Config.MovedRequireATB);
            } }

        public int POSITION { get => data.Position; }

        public string Name { get => data.name; }

        void IBattleCharacter.ThinkMove(BattleInfo battleInfo){
            switch (this.data.duty)
            {
                case GameDataBase.EnemyDuty.ATTACKER:
                    NormalAttackerAttack(ref battleInfo);
                    break;
                case GameDataBase.EnemyDuty.CURE:
                    CureAttack(ref battleInfo);
                    break;
                default:
                    break;
            }
        }

        private void NormalAttackerAttack(ref BattleInfo battleInfo){
            var target = battleInfo.enemy_sortByPos[0];
            IBattleCharacter temp = this;
            int damage = (int)(this.ATK - target.DEF);
            AttackEventData attackEventData = new AttackEventData(ref temp, ref target,damage ,ref battleInfo);
            SingletonMonobehaviour<EventHandler>.Instance.AttackEvent(attackEventData);
        }

        private void CureAttack(ref BattleInfo battleInfo){
            var teammates = battleInfo.teammate_sortBySPE;
            teammates.Sort((x, y) => x.HP.CompareTo(y.HP));
            var target = teammates[0];//选择血量最少的友方进行治疗，虽然默认都是最前排
        }

        public void TeammateUnderAttack(AttackEventData eventData){

        }

        public void UnderAttack(AttackEventData attackEvent,out EscapeEvent escape) {
            var damage = attackEvent.Damage;
            escape= null;
            if(this.HP-damage <= 0) {//表示自己受到下一次攻击就会死去
                bool underPressure = UnderPressure();
                double[] param = GameDataBase.mentalBuffs;
                double baseProb = GameDataBase.Config.EscapeEventBaseProbability;
                //计算逃跑事件的发生概率，处于高压力状态下概率会增大。
                double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);//道德越高，越不容易发生
                double p2 = CalculateMentalBuffParam(data.COO, ref param, false);//合作能力越高，越不容易发生独自逃跑
                double finalProb = baseProb * p1 * p2*1.05;
                //接下来要考虑镇静度对参数的影响，原公式中谈到要计算预期收益。
                //我们用这个概率代表期望收益，镇静度越高越会倾向于选择期望收益高的选项。
                finalProb = CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                if (RandomNumberGenerator.Happened(finalProb) && data.rank < GameDataBase.EnemyRarity.BOSS) {
                    escape = new EscapeEvent(this);
                }
            }
            else {//受到攻击并不会死去，但是我们要检查理智程度来判断ai会不会产生错误行为
                if (this.HP - damage * 2 <= 0) {
                    bool underPressure = UnderPressure();
                    double[] param = GameDataBase.mentalBuffs;
                    double baseProb = GameDataBase.Config.EscapeEventBaseProbability;
                    //计算逃跑事件的发生概率，处于高压力状态下概率会增大。
                    double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);//道德越高，越不容易发生
                    double p2 = CalculateMentalBuffParam(data.COO, ref param, false);//合作能力越高，越不容易发生独自逃跑
                    double finalProb = baseProb * p1 * p2;
                    //接下来要考虑镇静度对参数的影响，原公式中谈到要计算预期收益。
                    //我们用这个概率代表期望收益，镇静度越高越会倾向于选择期望收益高的选项。
                    finalProb=CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                    if (RandomNumberGenerator.Happened(finalProb)&&data.rank<GameDataBase.EnemyRarity.BOSS) {
                        escape=new EscapeEvent(this);
                    }
                }
            }
        }

        public uint CalculateAbility() {
            return System.Convert.ToUInt32(HP + ATK * 5 + DEF * 5 + SPE * 10);
        }

        /// <summary>
        /// 计算精神属性的概率参数
        /// </summary>
        /// <param name="buff">精神数值</param>
        /// <param name="param">从GameDataBase中获取的概率数组</param>
        /// <param name="Positive">是否是正向修正</param>
        /// <returns></returns>
        private double CalculateMentalBuffParam(byte buff,ref double[]param,bool Positive) {
            return Positive ? param[buff] : 2.0 - param[buff];
        }

        /// <summary>
        /// 计算镇静度对于概率的影响
        /// </summary>
        /// <param name="probability">该事件发生的概率，也代表期望收益</param>
        /// <param name="underPressure">是否处于压力下</param>
        /// <param name="CalculateProb">镇静度的补正率，主要是对随机范围进行收束</param>
        /// <returns>修正后的概率</returns>
        private double CalculateCalmnessInfluence(ref double probability,ref bool underPressure,ref double[]CalculateProb) {
            byte Clamness = this.data.CAL;
            if (probability >= 0.5) {//此时代表期望收益为正,镇静度越高，越倾向于选择该事件
                double min = 0.7, max = 1.2;
                if (underPressure) {
                    min = 0.5;max = 1.5;
                }
                min =min*CalculateMentalBuffParam(data.CAL,ref CalculateProb,true);//如果镇静度高，那么min将会乘大于一的数，使得范围被收缩;镇静度低，范围会扩大
                double finalParam=RandomNumberGenerator.Averaeg_GetRandomNumber(min, max);
                return probability* finalParam;
            }
            else {//此时代表期望收益为负，镇静度越高，越不倾向于选择该事件。高压环境下，镇静度越低，越倾向于选择该事件
                double min = 0.8, max = 1.3;
                if(underPressure) {
                    min = 0.7;max = 1.5;
                }
                max = max * CalculateMentalBuffParam(data.CAL, ref CalculateProb, false);
                double finalParam = RandomNumberGenerator.Averaeg_GetRandomNumber(min, max);
                return probability* finalParam;
            }
        }

        private bool UnderPressure() {//判断目前是否处于高压环境
            double opponentsAbility = 0;
            foreach (var item in opponents) {
                opponentsAbility += item.CalculateAbility();
            }
            double teammatesAbility = 0;
            foreach (var item in teammates) {
                teammatesAbility += item.CalculateAbility();
            }
            return teammatesAbility*1.2 < opponentsAbility;
        }

        public float atb;

        public EnemyData data;

        private List<IBattleCharacter> teammates;

        private List<IBattleCharacter> opponents;
    }
}

