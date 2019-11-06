package config

import (
	"math"
)

type cell struct {
	row int32
	col int32
}

type formulaSheetTemplate struct {
	datas    map[int32]float64
	relation map[int32][]int32
	funcs    map[int32]func(*formulaSheet) float64
}

type formulaSheet struct {
	template *formulaSheetTemplate
	datas    map[int32]float64
}

func (ins *formulaSheet) get(key int32) float64 {
	v, ok := ins.datas[key]
	if ok {
		return v
	}

	v, ok = ins.template.datas[key]
	if ok {
		ins.datas[key] = v
		return v
	}

	f, ok := ins.template.funcs[key]
	if ok {
		v = f(ins)
		ins.datas[key] = v
		return v
	}

	log.Error("no value in sheet with key %d", key)
	return 0
}

func (ins *formulaSheet) set(key int32, value float64) {
	v, ok := ins.datas[key]
	if ok && v == value {
		return
	}

	ins.datas[key] = value
	for _, k := range ins.template.relation[key] {
		delete(ins.datas, k)
	}
}

func (ins *formulaSheet) excelIf(c, a, b float64) float64 {
	if c > 0 {
		return a
	}
	return b
}

func (ins *formulaSheet) excelCompare(a bool) float64 {
	if a {
		return 1
	}
	return -1
}

func (ins *formulaSheet) excelPow(a, b float64) float64 {
	return float64(math.Pow(float64(a), float64(b)))
}

func (ins *formulaSheet) excelMod(a, b float64) float64 {
	return float64(math.Mod(float64(a), float64(b)))
}

var factcache map[int32]int32
var factmax int32 = 1

func (ins *formulaSheet) excelFact(a float64) float64 {
	var n int32 = int32(a)
	if n < 0 {
		return 0
	}

	if n <= factmax {
		return float64(factcache[n])
	}

	for i := factmax + 1; i <= n; i++ {
		factcache[i] = factcache[i-1] * i
	}

	factmax = n
	return float64(factcache[n])
}

func (ins *formulaSheet) excelRound(a, b float64) float64 {
	d := math.Pow(10, float64(b))
	return float64(math.Floor(float64(a)*d+0.5) / d)
}

func (ins *formulaSheet) excelRoundDown(a, b float64) float64 {
	d := math.Pow(10, float64(b))
	return float64(math.Floor(float64(a)*d) / d)
}

func (ins *formulaSheet) excelRoundUp(a, b float64) float64 {
	d := math.Pow(10, float64(b))
	return float64(math.Ceil(float64(a)*d) / d)
}

func (ins *formulaSheet) excelMax(args ...float64) float64 {
	if len(args) == 0 {
		return 0
	}

	ret := float64(args[0])
	for _, v := range args {
		ret = math.Max(ret, float64(v))
	}
	return float64(ret)
}

func (ins *formulaSheet) excelMin(args ...float64) float64 {
	if len(args) == 0 {
		return 0
	}

	ret := float64(args[0])
	for _, v := range args {
		ret = math.Min(ret, float64(v))
	}
	return float64(ret)
}

func (ins *formulaSheet) excelOr(args ...float64) float64 {
	for _, v := range args {
		if v > 0 {
			return 1
		}
	}
	return 0
}

func (ins *formulaSheet) excelAnd(args ...float64) float64 {
	for _, v := range args {
		if v <= 0 {
			return 0
		}
	}	
	return 1
}

func init() {
	factcache = make(map[int32]int32)
	factcache[1] = 1
}
