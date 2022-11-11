using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class AscentCurve {
        /// <summary>
        /// 成长曲线的构造函数
        /// </summary>
        /// <param name="minRange">成长曲线下限的范围</param>
        /// <param name="maxRange">成长曲线上限的范围</param>
        /// <param name="inflectRange">成长曲线拐点的范围，拐点越靠前，满级能达到的值越大</param>
        /// <param name="sigmaRange">成长曲线陡峭度的范围，值越大曲线越趋于s形</param>
        public AscentCurve((int min, int max) minRange, (int min, int max) maxRange, (int min, int max) inflectRange, (float min, float max) sigmaRange) {
            this.min = (uint)RandomNumberGenerator.Gauss_GetRandomNumber((minRange.min + minRange.max) / 2, 1.0f, minRange.min, minRange.max);
            this.max = (uint)RandomNumberGenerator.Gauss_GetRandomNumber((maxRange.min + maxRange.max) / 2, 1.0f, maxRange.min, maxRange.max);
            this.InflectPoint = (uint)RandomNumberGenerator.Gauss_GetRandomNumber((inflectRange.min + inflectRange.max) / 2, 1.0f, inflectRange.min, inflectRange.max);
            this.sigma = Random.Range(sigmaRange.min, sigmaRange.max);
        }

        /// <summary>
        /// 成长曲线的构造曲线
        /// </summary>
        /// <param name="curveTemple">曲线的随机数值范围模板</param>
        public AscentCurve(((int min, int max) min_range, (int min, int max) max_range, (int min, int max) inflect, (float min, float max) sigma) curveTemple) {
            this.min = (uint)RandomNumberGenerator.Gauss_GetRandomNumber((curveTemple.min_range.min + curveTemple.min_range.max) / 2, 1.0f, curveTemple.min_range.min, curveTemple.min_range.max);
            this.max = (uint)RandomNumberGenerator.Gauss_GetRandomNumber((curveTemple.max_range.min + curveTemple.max_range.max) / 2, 1.0f, curveTemple.max_range.min, curveTemple.max_range.max);
            this.InflectPoint = (uint)Random.Range(curveTemple.inflect.min, curveTemple.inflect.max);
            this.sigma = Random.Range(curveTemple.sigma.min, curveTemple.inflect.max);
        }

        public AscentCurve(RandomRangeCurve rangeCurve,ref int level) {
            min = RandomNumberGenerator.Average_GetRandomNumber(rangeCurve.MinRange.min, rangeCurve.MinRange.max);
            max = RandomNumberGenerator.Average_GetRandomNumber(rangeCurve.MaxRange.min, rangeCurve.MaxRange.max);
            this.InflectPoint = RandomNumberGenerator.Average_GetRandomNumber(rangeCurve.InflectRange.min, rangeCurve.InflectRange.max);
            this.sigma = RandomNumberGenerator.Average_GetRandomNumber(rangeCurve.SigmaRange.min, rangeCurve.SigmaRange.max);
            GetValue(ref level);
        }

        public readonly uint min;
        public readonly uint max;
        private uint value;
        int lastlevel=0;
        public uint GetValue(ref int level) {
            if (level == lastlevel) {
                return value;
            }
            float xPsigma = Mathf.Pow(level, sigma);
            this.value = this.min + System.Convert.ToUInt32(((this.max * xPsigma) / (Mathf.Pow(InflectPoint, sigma) + xPsigma)));
            lastlevel = level;
            return value;
        }
        public readonly uint InflectPoint;
        public float sigma;
    }
}
