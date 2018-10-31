package config

type HeroFormulaSheet struct {
	formulaSheet
}

var heroFormaulaTemplate *formulaSheetTemplate

func loadFormulaHero() {
	heroFormaulaTemplate = new(formulaSheetTemplate)
	heroFormaulaTemplate.datas = make(map[int32]float64)
	heroFormaulaTemplate.relation = make(map[int32][]int32)
	heroFormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float64)
	heroFormaulaTemplate.datas[1003] = 20
	heroFormaulaTemplate.funcs[1006] = func(ins *formulaSheet) float64 {
		return (float64(data_level_vlookup_3(ins.get(1*1000+3))) + float64(ins.get(2*1000+8)))
	}
	heroFormaulaTemplate.datas[2003] = 5
	heroFormaulaTemplate.funcs[2008] = func(ins *formulaSheet) float64 {
		return ins.excelFact(ins.get(2*1000 + 3))
	}
	heroFormaulaTemplate.relation[1003] = []int32{1006}
	heroFormaulaTemplate.relation[2008] = []int32{1006}
	heroFormaulaTemplate.relation[2003] = []int32{2008, 1006}
}
func NewHeroFormula() *HeroFormulaSheet {
	formula := new(HeroFormulaSheet)
	formula.template = heroFormaulaTemplate
	formula.datas = make(map[int32]float64)
	return formula
}
func (ins *HeroFormulaSheet) GetLevel() float64 { //等级
	return ins.get(1003)
}
func (ins *HeroFormulaSheet) SetLevel(v float64) { //等级
	ins.set(1003, v)
}
func (ins *HeroFormulaSheet) GetRatio() float64 { //成长
	return ins.get(2003)
}
func (ins *HeroFormulaSheet) SetRatio(v float64) { //成长
	ins.set(2003, v)
}
func (ins *HeroFormulaSheet) GetMaxhp() float64 { //最大血量
	return ins.get(1006)
}
