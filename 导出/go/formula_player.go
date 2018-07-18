package config

type PlayerFormulaSheet struct {
	formulaSheet
}

var playerFormaulaTemplate *formulaSheetTemplate

func loadFormulaPlayer() {
	playerFormaulaTemplate = new(formulaSheetTemplate)
	playerFormaulaTemplate.datas = make(map[int32]float32)
	playerFormaulaTemplate.relation = make(map[int32][]int32)
	playerFormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float32)
	playerFormaulaTemplate.datas[1003] = 10
	playerFormaulaTemplate.funcs[1006] = func(ins *formulaSheet) float32 {
		return (float32(ins.get(1*1000+3)) * float32(999))
	}
	playerFormaulaTemplate.relation[1003] = []int32{1006}
}
func NewPlayerFormula() *PlayerFormulaSheet {
	formula := new(PlayerFormulaSheet)
	formula.template = playerFormaulaTemplate
	formula.datas = make(map[int32]float32)
	return formula
}
func (ins *PlayerFormulaSheet) GetLevel() float32 { //等级
	return ins.get(1003)
}
func (ins *PlayerFormulaSheet) SetLevel(v float32) { //等级
	ins.set(1003, v)
}
func (ins *PlayerFormulaSheet) GetLevelupexp() float32 { //升级经验
	return ins.get(1006)
}
