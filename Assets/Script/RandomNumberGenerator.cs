using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator
{
    public static bool Happened(int level) {
        int Comparer = (int)(statistic[level] * int.MaxValue);
        int rng = Average_GetRandomNumber(0, int.MaxValue);
        return rng < Comparer;
    }

    public static bool Happened(double probability) {
        probability = Clamp01(probability);
        int rng = Average_GetRandomNumber(0, int.MaxValue);
        int Comparer = System.Convert.ToInt32(probability * int.MaxValue);
        return rng < Comparer;
    }

    private static double[] statistic=>Nagopia.GameDataBase.Config.Equipment_RPair_Probability;

    public static int Average_GetRandomNumber(int min,int max) {
        if (min > max) {
            SwapData<int>(ref min, ref max);
        }
        if (max == min) {
            max += 1;
        }
        return Random.Range(min, max);
    }

    public static byte Average_GetRandomNumber(byte min,byte max) {
        if (min > max) {
            SwapData<byte>(ref min, ref max);
        }
        if (max == min) {
            max += 1;
        }
        int val = Random.Range(min, max);
        return (byte)val;
    }

    public static uint Average_GetRandomNumber(uint min,uint max) {
        int mi = System.Convert.ToInt32(min);
        int ma = System.Convert.ToInt32(max);
        if (mi > ma) {
            SwapData<int>(ref mi, ref ma);
        }
        if (mi == ma) {
            ma += 1;
        }
        return System.Convert.ToUInt32(Random.Range(mi, ma));
    }

    public static float Average_GetRandomNumber(float min,float max) {
        if (min > max) {
            SwapData<float>(ref min,ref max);
        }
        return Random.Range(min, max);
    }

    /// <summary>
    /// 正态分布的随机生成器
    /// </summary>
    /// <param name="miu">正态分布的平均值，越在miu附近的值出现的概率越大</param>
    /// <param name="sig">正态分布的方差，sig越大，miu出现的概率最小，函数图像越平缓</param>
    /// <param name="min">正态分布能生成的最小值</param>
    /// <param name="max">正态分布能生成的最大值</param>
    /// <returns></returns>
    public static int Gauss_GetRandomNumber(int miu,float sig,int min,int max) {
        int res=0;
        float y,dScope;
        ++max;
        do {
            res = Random.Range(min, max);
            y = Normal(ref res, ref miu, ref sig);
            dScope = Normal(ref miu, ref miu, ref sig);
        } while (dScope > y);
        return res;
        static float Normal(ref int x, ref int miu, ref float sigma) {//本地函数，用来求正态分布的概率密度
            return (1.0f / (GAUSS_NORMAL * sigma)) * Mathf.Exp(-((x - miu) * (x - miu) * 1.0f) / (2.0f * sigma * sigma));
        }
    }
    
    /// <summary>
    /// 正态分布概率密度中的一个常量，此处用来避免重复计算
    /// </summary>
    private static readonly float GAUSS_NORMAL = Mathf.Sqrt(2.0f * Mathf.PI);

    public static double Clamp01(double val) {
        if (val > 1.00) {
            val = 1.00;
        }
        else if (val < 0.00) {
            val = 0.0;
        }
        return val;
    }

    public static void SwapData<T>(ref T x,ref T y) {
        T temp = x;
        x = y;
        y = temp;
    }
}
