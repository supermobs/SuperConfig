/*
 * @Author: chiuan wei 
 * @Date: 2017-11-06 01:32:19 
 * @Last Modified by: chiuan wei
 * @Last Modified time: 2017-11-06 23:35:56
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有生成配置表的实例化方法都在这个Config里面
/// </summary>
public partial class Config
{
    // public static string GetClientTip(string label)
    // {
    //     var ls = Config.GetClientTipTable().Get_name(label);
    //     if(ls != null && ls.Length > 0)
    //     {
    //         return ls[0].Tip;
    //     }
    //     else
    //     {
    //         return "";
    //     }
    // }
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

    public float excelIf(float a, float b, float c)
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