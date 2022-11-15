using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    /// <summary>
    /// ��ҷ���ɫ
    /// </summary>
    public class BattleCharacter : IBattleCharacter {
        public int HP { get => data.CurrentHP; set => data.CurrentHP = value; }
        public uint ATK { get => data.ATK; set { } }
        public uint DEF { get => data.DEF; set { } }
        public uint SPE { get => data.SPE; set { } }
        public float ATB { get => atb; 
            set {
                this.atb = Mathf.Clamp(value, 0f, GameDataBase.Config.MovedRequireATB);
            } }
        public int POSITION { get => data.Position; set => data.Position = value; }

        public string Name { get => data.name; }

        public float atb;

        [Sirenix.OdinInspector.ShowInInspector]
        private CharacterData data;

        public void ThinkMove(BattleInfo battleInfo) {
            //var teammate = battleInfo.teammate_sortByPos;
            
        }

        /// <summary>
        /// �����¼���˼��
        /// </summary>
        /// <param name="battleInfo"></param>
        private void ConsiderEscape(ref BattleInfo battleInfo) {
            var myIndex = battleInfo.teammate_sortByPos.IndexOf(this);
            if (myIndex == 0) {//����������һ���ܻ��Ķ���
                GameDataBase g = new GameDataBase();
            }
            else {

            }
        }

        private void ConsiderAttack(ref BattleInfo battleInfo) {
            IBattleCharacter target;
            switch (this.data.Profession) {
                case GameDataBase.CharacterProfession.KNIGHT:
                    target = battleInfo.enemy_sortByPos[0];//��ʿ�Ĺ���ѡ��з������Ŀ��
                    break;
                case GameDataBase.CharacterProfession.RANGER:
                    target = battleInfo.enemy_sortByPos[0];//�����Ĺ���ѡ��з������Ŀ��
                    break;
                case GameDataBase.CharacterProfession.PRIEST:
                    var possibleTargets = new List<IBattleCharacter>(battleInfo.teammate_sortByPos);
                    possibleTargets.Sort((x, y) => x.HP.CompareTo(y.HP));
                    
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// �����жϵ�ǰ�Ƿ�����״̬����Ҫ����ǰ�����Ѫ�����Լ������CAL�й�
        /// ������Ѫ������20%���߶�����Ѫ������40%ʱ
        /// �������ǿ��ʱ
        /// </summary>
        /// <param name="battleInfo"></param>
        /// <returns></returns>
        private bool StayCalm(ref BattleInfo battleInfo) {
            return false;
        }
        
    }
}
