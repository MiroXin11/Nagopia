using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
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
        //Debug.Log($"rng={rng}");
        return rng < Comparer;
    }

    private static double[] statistic=>Nagopia.GameDataBase.Config.Equipment_RPair_Probability;

    /// <summary>
    /// 获得一个int类型的随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="includeRight">是否包含最大值，默认包含</param>
    /// <returns></returns>
    public static int Average_GetRandomNumber(int min,int max,bool includeRight=true) {
        if (min > max) {
            SwapData<int>(ref min, ref max);
        }
        if (max == min&&max!=int.MaxValue) {
            max += 1;
        }
        AddUsedTimes();
        if (includeRight&&max!=int.MaxValue)
            return Random.Range(min, max+1);
        else
            return Random.Range(min, max);
    }

    /// <summary>
    /// 获得一个byte类型的随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="includeRight">生成的随机数是否包含右边界，默认包含</param>
    /// <returns></returns>
    public static byte Average_GetRandomNumber(byte min,byte max,bool includeRight=true) {
        if (min > max) {
            SwapData<byte>(ref min, ref max);
        }
        if (max == min&&max!=byte.MaxValue) {
            max += 1;
        }
        int val = 0;
        AddUsedTimes();
        if (includeRight&&max!=byte.MaxValue)
            val = Random.Range(min, max+1);
        else
            val = Random.Range(min, max);
        return (byte)val;
    }

    /// <summary>
    /// 获得一个uint类型的随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="includeRight">是否包含最大值</param>
    /// <returns></returns>
    public static uint Average_GetRandomNumber(uint min,uint max,bool includeRight=true) {
        int mi = System.Convert.ToInt32(min);
        int ma = System.Convert.ToInt32(max);
        if (mi > ma) {
            SwapData<int>(ref mi, ref ma);
        }
        if (mi == ma && ma != int.MaxValue) {
            ma += 1;
        }
        AddUsedTimes();
        if (includeRight && max != int.MaxValue)
            return System.Convert.ToUInt32(Random.Range(mi, ma + 1));
        else
            return System.Convert.ToUInt32(Random.Range(mi, ma));
    }

    public static float Average_GetRandomNumber(float min,float max) {
        if (min > max) {
            SwapData<float>(ref min,ref max);
        }
        AddUsedTimes();
        return Random.Range(min, max);
    }

    public static double Average_GetRandomNumber(double min,double max) {
        if (min > max) {
            SwapData<double>(ref min, ref max);
        }
        AddUsedTimes();
        return Random.Range((float)min, (float)max);
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
            AddUsedTimes();
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

    private static int usedTimes = 0;

    private static void AddUsedTimes() {
        ++usedTimes;
        if (usedTimes >= 10) {
            usedTimes = 0;
            InitialRandom();
        }
    }

    public static void InitialRandom() {
        byte[] bytes = new byte[4];
        RNGCryptoServiceProvider rNG = new RNGCryptoServiceProvider();
        rNG.GetBytes(bytes);
        Random.InitState(System.BitConverter.ToInt32(bytes, 0));
    }
}
