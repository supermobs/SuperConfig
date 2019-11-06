/*
 * @Author: chiuan wei 
 * @Date: 2017-11-06 01:32:19 
 * @Last Modified by: chiuan wei
 * @Last Modified time: 2017-11-06 23:35:56
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 所有生成配置表的实例化方法都在这个Config里面
/// </summary>
public partial class Config
{
    // 游戏用到的配置表json存放目录
    const string PATH_ASSETS_FOLDER = "Assets/config_data";

    // 审核版本配置表对应存放目录
    const string PATH_REVIEW_ASSETS_FOLDER = "Assets/config_data/review";

    // 委托的加载接口方便不同项目设置
    public delegate byte[] DelegateLoadConfigBytes(string filename);

    public delegate string DelegateLoadConfigJson(string filepath);

    public static DelegateLoadConfigBytes delegateLoadConfigBytes;
    public static DelegateLoadConfigJson delegateLoadConfigJson;
}

public struct SheetCell
{
    public int row;
    public int col;
}

public class FormulaSheetTemplate
{
    public Dictionary<int, float> datas = new Dictionary<int, float>();
    public Dictionary<int, int[]> relation = new Dictionary<int, int[]>();
    public Dictionary<int, Func<FormulaSheet, float>> funcs = new Dictionary<int, Func<FormulaSheet, float>>();

    void TestXXX()
    {
        if (datas.ContainsKey(1))
        {
            //datas.Count;
        }
    }
}

/// <summary>
/// 配置表生成的基础类,结合公式化的计算可以使用
/// 具体的生成表格的sheet继承这个对象
/// </summary>
public class FormulaSheet : FormulaSheetTemplate
{
    public string name;
    public Dictionary<int, float> newdatas = new Dictionary<int, float>();

    public float get(int key)
    {
        if (newdatas.ContainsKey(key))
            return newdatas[key];

        if (datas.ContainsKey(key))
            return datas[key];

        if (funcs.ContainsKey(key))
        {
            var v = funcs[key](this);
            newdatas[key] = v;
            return v;
        }

        Debug.LogError("no value in sheet " + name + " with key = " + key +",请检查是否用Config.NewXXX来构造算法对象");
        return 0;
    }

    public void set(int key, float val)
    {
        if (newdatas.ContainsKey(key))
        {
            if (newdatas[key] == val)
                return;
        }

        newdatas[key] = val;

        if (relation.ContainsKey(key))
        {
            foreach (var k in relation[key])
            {
                newdatas.Remove(k);
            }
        }
    }

    public float excelAnd(params float[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] <= 0f)
            {
                return 0f;
            }
        }

        return 1f;
    }

    public float excelOr(params float[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] > 0f)
            {
                return 1f;
            }
        }

        return 0f;
    }

    public float excelIf(float c, float a, float b)
    {
        if (c > 0)
            return a;
        return b;
    }

    public float excelCompare(bool a)
    {
        if (a)
            return 1;
        return -1;
    }

    public float excelPow(float a, float b)
    {
        return (float) Math.Pow((double) a, (double) b);
    }

    public float excelMod(float a, float b)
    {
        return a % b;
    }

    static Dictionary<int, int> _factcache;
    static Dictionary<int, int> factcache
    {
        get
        {
            if (_factcache == null)
            {
                _factcache = new Dictionary<int, int>();
                _factcache[1] = 1;
            }
            return _factcache;
        }
        set { }
    }
    static int factmax = 1;

    public float excelFact(float a)
    {
        var n = (int) a;
        if (n < 0)
            return 0;

        if (n <= factmax)
        {
            return (float) factcache[n];
        }

        for (int i = factmax + 1; i <= n; i++)
        {
            factcache[i] = factcache[i - 1] * i;
        }

        factmax = n;
        return (float) factcache[n];
    }

    public float excelRound(float a, float b)
    {
        var d = Math.Pow(10, (double) b);
        return (float) Math.Floor(((double) a * d + 0.5) / d);
    }

    public float excelRoundDown(float a, float b)
    {
        var d = Math.Pow(10, (double) b);
        return (float) Math.Floor(((double) a * d) / d);
    }

    public float excelRoundUp(float a, float b)
    {
        var d = (float) Math.Pow(10, (double) b);
        return (float) Mathf.Ceil(a * d) / d;
    }

    public float excelMax(params float[] args)
    {
        if (args.Length == 0)
            return 0;

        var ret = args[0];
        foreach (var v in args)
        {
            ret = Mathf.Max(ret, v);
        }
        return ret;
    }

    public float excelMin(params float[] args)
    {
        if (args.Length == 0)
            return 0;

        var ret = args[0];
        foreach (var v in args)
        {
            ret = Mathf.Min(ret, v);
        }
        return ret;
    }

}

#region 配置表流加载功能

public interface IConfigSerializer
{
    void ToStream(BinaryWriter bw);
    void FromStream(BinaryReader br);
}

public abstract class StreamConfig : IConfigSerializer
{
    public abstract void FromStream(BinaryReader br);
    public abstract void ToStream(BinaryWriter bw);

    public byte[] ToBytes()
    {
        using (var ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms,Encoding.UTF8))
            {
                ToStream(bw);

                return ms.ToArray();

//                bw.BaseStream.Position = 0;
//                byte[] bytes = new byte[bw.BaseStream.Length];
//                bw.BaseStream.Read(bytes, 0, bytes.Length);
//                return bytes;
            }
        }
    }

    public void FromBytes(byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            using (var br = new BinaryReader(ms,Encoding.UTF8))
            {
                FromStream(br);
            }
        }
    }

    protected void ReadArray(BinaryReader br,ref int[] arr)
    {
        int _count = br.ReadInt32();
        arr = new int[_count];
        for (int i = 0; i < _count; i++)
        {
            arr[i] = br.ReadInt32();
        }
    }
    protected void ReadArray(BinaryReader br,ref string[] arr)
    {
        int _count = br.ReadInt32();
        arr = new string[_count];
        for (int i = 0; i < _count; i++)
        {
            arr[i] = br.ReadString();
        }
    }

    protected void ReadArray(BinaryReader br,ref double[] arr)
    {
        int _count = br.ReadInt32();
        arr = new double[_count];
        for (int i = 0; i < _count; i++)
        {
            arr[i] = br.ReadDouble();
        }
    }
    protected void ReadArray(BinaryReader br,ref long[] arr)
    {
        int _count = br.ReadInt32();
        arr = new long[_count];
        for (int i = 0; i < _count; i++)
        {
            arr[i] = br.ReadInt64();
        }
    }
    protected void ReadArray(BinaryReader br,ref float[] arr)
    {
        int _count = br.ReadInt32();
        arr = new float[_count];
        for (int i = 0; i < _count; i++)
        {
            arr[i] = br.ReadSingle();
        }
    }

    public void WriteArray(BinaryWriter bw,ref int[] arr)
    {
        if(arr == null)
        {
            bw.Write(0);
            return;
        }

        // 先写长度
        bw.Write(arr.Length);

        // 一个个写入
        for (int i = 0; i < arr.Length; i++)
        {
            bw.Write(arr[i]);
        }
    }

    public void WriteArray(BinaryWriter bw,ref long[] arr)
    {
        if(arr == null)
        {
            bw.Write(0);
            return;
        }
        bw.Write(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            bw.Write(arr[i]);
        }
    }

    public void WriteArray(BinaryWriter bw,ref float[] arr)
    {
        if(arr == null)
        {
            bw.Write(0);
            return;
        }
        bw.Write(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            bw.Write(arr[i]);
        }
    }

    public void WriteArray(BinaryWriter bw,ref string[] arr)
    {
        if(arr == null)
        {
            bw.Write(0);
            return;
        }
        bw.Write(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            bw.Write(arr[i]);
        }
    }

    public void WriteArray(BinaryWriter bw,ref double[] arr)
    {
        if(arr == null)
        {
            bw.Write(0);
            return;
        }
        bw.Write(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            bw.Write(arr[i]);
        }
    }

}

#endregion