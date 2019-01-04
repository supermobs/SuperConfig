package config

type Damage_buffFormulaSheet struct {
	formulaSheet
}

var damage_buffFormaulaTemplate *formulaSheetTemplate

func loadFormulaDamage_buff() {
	damage_buffFormaulaTemplate = new(formulaSheetTemplate)
	damage_buffFormaulaTemplate.datas = make(map[int32]float64)
	damage_buffFormaulaTemplate.relation = make(map[int32][]int32)
	damage_buffFormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float64)
	damage_buffFormaulaTemplate.datas[2003] = 1
	damage_buffFormaulaTemplate.funcs[2006] = func(ins *formulaSheet) float64 {
		return ins.get(7*1000 + 8)
	}
	damage_buffFormaulaTemplate.datas[3003] = 0.0015
	damage_buffFormaulaTemplate.datas[4003] = 0.03
	damage_buffFormaulaTemplate.datas[5003] = 12.54
	damage_buffFormaulaTemplate.funcs[5006] = func(ins *formulaSheet) float64 {
		return ins.get(8*1000 + 8)
	}
	damage_buffFormaulaTemplate.datas[6003] = 3
	damage_buffFormaulaTemplate.datas[7003] = 1
	damage_buffFormaulaTemplate.funcs[7008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[8003] = 0
	damage_buffFormaulaTemplate.funcs[8006] = func(ins *formulaSheet) float64 {
		return ins.get(9*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[8008] = func(ins *formulaSheet) float64 {
		return (float64(0) - float64((float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[9003] = 0
	damage_buffFormaulaTemplate.funcs[9008] = func(ins *formulaSheet) float64 {
		return (float64(0) - float64((float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[10003] = 0
	damage_buffFormaulaTemplate.funcs[10008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[11003] = 0
	damage_buffFormaulaTemplate.funcs[11006] = func(ins *formulaSheet) float64 {
		return ins.get(10*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[11008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))
	}
	damage_buffFormaulaTemplate.funcs[12008] = func(ins *formulaSheet) float64 {
		return (float64(0) - float64((float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))))
	}
	damage_buffFormaulaTemplate.funcs[13008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64(ins.get(4*1000+3)) + float64((float64(ins.get(5*1000+3)) * float64((float64(ins.get(25*1000+3)) - float64(1))))))) * float64(ins.get(20*1000+3)))) * float64(ins.get(21*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[14003] = 600
	damage_buffFormaulaTemplate.funcs[14006] = func(ins *formulaSheet) float64 {
		return ins.get(11*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[14008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(24*1000+3)) * float64(ins.get(3*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[15003] = 580
	damage_buffFormaulaTemplate.funcs[15008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(23*1000+3)) * float64(ins.get(3*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[16003] = 0
	damage_buffFormaulaTemplate.funcs[16008] = func(ins *formulaSheet) float64 {
		return (float64(0) - float64((float64(ins.get(24*1000+3)) * float64(ins.get(3*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[17003] = 0.1
	damage_buffFormaulaTemplate.funcs[17006] = func(ins *formulaSheet) float64 {
		return ins.get(12*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[17008] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[18003] = 0.4
	damage_buffFormaulaTemplate.funcs[18008] = func(ins *formulaSheet) float64 {
		return (float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))
	}
	damage_buffFormaulaTemplate.datas[19003] = 20
	damage_buffFormaulaTemplate.funcs[19008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))) * float64(0.3))
	}
	damage_buffFormaulaTemplate.datas[20003] = 1
	damage_buffFormaulaTemplate.funcs[20006] = func(ins *formulaSheet) float64 {
		return ins.get(11*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[20008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64(ins.get(21*1000+3)) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damage_buffFormaulaTemplate.datas[21003] = 1
	damage_buffFormaulaTemplate.funcs[21008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64(ins.get(21*1000+3)) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damage_buffFormaulaTemplate.datas[22003] = 0.6
	damage_buffFormaulaTemplate.funcs[22008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(25*1000+3)) * float64(ins.get(3*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[23003] = 50000
	damage_buffFormaulaTemplate.funcs[23006] = func(ins *formulaSheet) float64 {
		return ins.get(14*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[23008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(3*1000+3)))) + float64(0.05))) * float64(ins.get(23*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[24003] = 100
	damage_buffFormaulaTemplate.funcs[24008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(3*1000+3)) + float64((float64(ins.get(4*1000+3)) * float64(ins.get(25*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[25003] = 20
	damage_buffFormaulaTemplate.funcs[25008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(3*1000+3)) * float64(ins.get(25*1000+3)))
	}
	damage_buffFormaulaTemplate.funcs[26006] = func(ins *formulaSheet) float64 {
		return ins.get(15*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[26008] = func(ins *formulaSheet) float64 {
		return ins.get(3*1000 + 3)
	}
	damage_buffFormaulaTemplate.funcs[27008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(3*1000+3)) * float64(ins.get(23*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[28003] = 900
	damage_buffFormaulaTemplate.funcs[28008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64(ins.get(25*1000+3)) * float64(ins.get(4*1000+3)))))) * float64(0.3))) + float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64(ins.get(4*1000+3)) * float64(ins.get(25*1000+3)))))) * float64(0.3))))) / float64(2))
	}
	damage_buffFormaulaTemplate.datas[29003] = 1000
	damage_buffFormaulaTemplate.funcs[29006] = func(ins *formulaSheet) float64 {
		return ins.get(16*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[29008] = func(ins *formulaSheet) float64 {
		return (float64((float64(0.05) + float64((float64(ins.get(3*1000+3)) * float64(ins.get(25*1000+3)))))) * float64(ins.get(23*1000+3)))
	}
	damage_buffFormaulaTemplate.datas[30003] = 0
	damage_buffFormaulaTemplate.funcs[30008] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(4*1000+3)) + float64((float64(ins.get(3*1000+3)) * float64(ins.get(25*1000+3)))))
	}
	damage_buffFormaulaTemplate.datas[31003] = 0
	damage_buffFormaulaTemplate.datas[32003] = 0.05
	damage_buffFormaulaTemplate.funcs[32006] = func(ins *formulaSheet) float64 {
		return ins.get(17*1000 + 8)
	}
	damage_buffFormaulaTemplate.datas[33003] = 0.15
	damage_buffFormaulaTemplate.datas[34003] = 50
	damage_buffFormaulaTemplate.datas[35003] = 0.6
	damage_buffFormaulaTemplate.funcs[35006] = func(ins *formulaSheet) float64 {
		return ins.get(18*1000 + 8)
	}
	damage_buffFormaulaTemplate.datas[36003] = 1000
	damage_buffFormaulaTemplate.datas[37003] = 100
	damage_buffFormaulaTemplate.funcs[38006] = func(ins *formulaSheet) float64 {
		return ins.get(19*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[41006] = func(ins *formulaSheet) float64 {
		return ins.get(20*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[44006] = func(ins *formulaSheet) float64 {
		return ins.get(21*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[47006] = func(ins *formulaSheet) float64 {
		return ins.get(22*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[50006] = func(ins *formulaSheet) float64 {
		return ins.get(23*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[53006] = func(ins *formulaSheet) float64 {
		return ins.get(24*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[56006] = func(ins *formulaSheet) float64 {
		return ins.get(25*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[59006] = func(ins *formulaSheet) float64 {
		return ins.get(26*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[62006] = func(ins *formulaSheet) float64 {
		return ins.get(27*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[65006] = func(ins *formulaSheet) float64 {
		return ins.get(28*1000 + 8)
	}
	damage_buffFormaulaTemplate.funcs[68006] = func(ins *formulaSheet) float64 {
		return ins.get(29*1000 + 8)
	}
	damage_buffFormaulaTemplate.relation[7008] = []int32{2006}
	damage_buffFormaulaTemplate.relation[8008] = []int32{5006}
	damage_buffFormaulaTemplate.relation[4003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 17008, 18008, 19008, 20008, 21008, 24008, 28008, 30008, 2006, 5006, 8006, 11006, 14006, 20006, 17006, 32006, 35006, 38006, 41006, 44006, 53006, 65006}
	damage_buffFormaulaTemplate.relation[5003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 18008, 2006, 5006, 8006, 11006, 14006, 20006, 17006, 35006}
	damage_buffFormaulaTemplate.relation[25003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 17008, 18008, 19008, 22008, 23008, 24008, 25008, 28008, 29008, 30008, 2006, 5006, 8006, 11006, 14006, 20006, 17006, 32006, 35006, 38006, 47006, 50006, 53006, 56006, 65006, 68006}
	damage_buffFormaulaTemplate.relation[20003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 2006, 5006, 8006, 11006, 14006, 20006, 17006}
	damage_buffFormaulaTemplate.relation[21003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 17008, 18008, 19008, 20008, 21008, 2006, 5006, 8006, 11006, 14006, 20006, 17006, 32006, 35006, 38006, 41006, 44006}
	damage_buffFormaulaTemplate.relation[9008] = []int32{8006}
	damage_buffFormaulaTemplate.relation[10008] = []int32{11006}
	damage_buffFormaulaTemplate.relation[11008] = []int32{14006, 20006}
	damage_buffFormaulaTemplate.relation[24003] = []int32{14008, 16008, 23006, 29006}
	damage_buffFormaulaTemplate.relation[3003] = []int32{14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 26008, 27008, 28008, 29008, 30008, 23006, 26006, 29006, 32006, 35006, 38006, 41006, 44006, 47006, 50006, 53006, 56006, 59006, 62006, 65006, 68006}
	damage_buffFormaulaTemplate.relation[23003] = []int32{15008, 23008, 27008, 29008, 26006, 50006, 62006, 68006}
	damage_buffFormaulaTemplate.relation[12008] = []int32{17006}
	damage_buffFormaulaTemplate.relation[15003] = []int32{17008, 18008, 19008, 21008, 28008, 32006, 35006, 38006, 44006, 65006}
	damage_buffFormaulaTemplate.relation[29003] = []int32{19008, 21008, 38006, 44006}
	damage_buffFormaulaTemplate.relation[14003] = []int32{20008, 28008, 41006, 65006}
	damage_buffFormaulaTemplate.relation[28003] = []int32{20008, 41006}
	damage_buffFormaulaTemplate.relation[14008] = []int32{23006}
	damage_buffFormaulaTemplate.relation[15008] = []int32{26006}
	damage_buffFormaulaTemplate.relation[16008] = []int32{29006}
	damage_buffFormaulaTemplate.relation[17008] = []int32{32006}
	damage_buffFormaulaTemplate.relation[18008] = []int32{35006}
	damage_buffFormaulaTemplate.relation[19008] = []int32{38006}
	damage_buffFormaulaTemplate.relation[20008] = []int32{41006}
	damage_buffFormaulaTemplate.relation[21008] = []int32{44006}
	damage_buffFormaulaTemplate.relation[22008] = []int32{47006}
	damage_buffFormaulaTemplate.relation[23008] = []int32{50006}
	damage_buffFormaulaTemplate.relation[24008] = []int32{53006}
	damage_buffFormaulaTemplate.relation[25008] = []int32{56006}
	damage_buffFormaulaTemplate.relation[26008] = []int32{59006}
	damage_buffFormaulaTemplate.relation[27008] = []int32{62006}
	damage_buffFormaulaTemplate.relation[28008] = []int32{65006}
	damage_buffFormaulaTemplate.relation[29008] = []int32{68006}
}
func NewDamage_buffFormula() *Damage_buffFormulaSheet {
	formula := new(Damage_buffFormulaSheet)
	formula.template = damage_buffFormaulaTemplate
	formula.datas = make(map[int32]float64)
	return formula
}
func (ins *Damage_buffFormulaSheet) GetParam1() float64 { //额外参数1（公式序号）
	return ins.get(2003)
}
func (ins *Damage_buffFormulaSheet) SetParam1(v float64) { //额外参数1（公式序号）
	ins.set(2003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam2() float64 { //额外参数2（攻击力系数）
	return ins.get(3003)
}
func (ins *Damage_buffFormulaSheet) SetParam2(v float64) { //额外参数2（攻击力系数）
	ins.set(3003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam3() float64 { //额外参数3（技能初始值）
	return ins.get(4003)
}
func (ins *Damage_buffFormulaSheet) SetParam3(v float64) { //额外参数3（技能初始值）
	ins.set(4003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam4() float64 { //额外参数4（技能等级系数）
	return ins.get(5003)
}
func (ins *Damage_buffFormulaSheet) SetParam4(v float64) { //额外参数4（技能等级系数）
	ins.set(5003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam5() float64 { //额外参数5（群秒衰减系数）
	return ins.get(6003)
}
func (ins *Damage_buffFormulaSheet) SetParam5(v float64) { //额外参数5（群秒衰减系数）
	ins.set(6003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam6() float64 { //额外参数6（目标个数）
	return ins.get(7003)
}
func (ins *Damage_buffFormulaSheet) SetParam6(v float64) { //额外参数6（目标个数）
	ins.set(7003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam7() float64 { //额外参数7（BUFF添加概率）
	return ins.get(8003)
}
func (ins *Damage_buffFormulaSheet) SetParam7(v float64) { //额外参数7（BUFF添加概率）
	ins.set(8003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam8() float64 { //额外参数8（1-BUFF添加概率）
	return ins.get(9003)
}
func (ins *Damage_buffFormulaSheet) SetParam8(v float64) { //额外参数8（1-BUFF添加概率）
	ins.set(9003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam9() float64 { //额外参数9（BUFF-ID1）
	return ins.get(10003)
}
func (ins *Damage_buffFormulaSheet) SetParam9(v float64) { //额外参数9（BUFF-ID1）
	ins.set(10003, v)
}
func (ins *Damage_buffFormulaSheet) GetParam10() float64 { //额外参数10（BUFF-ID2）
	return ins.get(11003)
}
func (ins *Damage_buffFormulaSheet) SetParam10(v float64) { //额外参数10（BUFF-ID2）
	ins.set(11003, v)
}
func (ins *Damage_buffFormulaSheet) GetPattack() float64 { //物攻
	return ins.get(14003)
}
func (ins *Damage_buffFormulaSheet) SetPattack(v float64) { //物攻
	ins.set(14003, v)
}
func (ins *Damage_buffFormulaSheet) GetMattack() float64 { //特攻
	return ins.get(15003)
}
func (ins *Damage_buffFormulaSheet) SetMattack(v float64) { //特攻
	ins.set(15003, v)
}
func (ins *Damage_buffFormulaSheet) GetAplevel() float64 { //攻修
	return ins.get(16003)
}
func (ins *Damage_buffFormulaSheet) SetAplevel(v float64) { //攻修
	ins.set(16003, v)
}
func (ins *Damage_buffFormulaSheet) GetHit() float64 { //命中率
	return ins.get(17003)
}
func (ins *Damage_buffFormulaSheet) SetHit(v float64) { //命中率
	ins.set(17003, v)
}
func (ins *Damage_buffFormulaSheet) GetCrit() float64 { //暴击率
	return ins.get(18003)
}
func (ins *Damage_buffFormulaSheet) SetCrit(v float64) { //暴击率
	ins.set(18003, v)
}
func (ins *Damage_buffFormulaSheet) GetAlevel() float64 { //攻击者等级
	return ins.get(19003)
}
func (ins *Damage_buffFormulaSheet) SetAlevel(v float64) { //攻击者等级
	ins.set(19003, v)
}
func (ins *Damage_buffFormulaSheet) GetEvolveRadio() float64 { //进化技能系数
	return ins.get(20003)
}
func (ins *Damage_buffFormulaSheet) SetEvolveRadio(v float64) { //进化技能系数
	ins.set(20003, v)
}
func (ins *Damage_buffFormulaSheet) GetGradeRadio() float64 { //评级技能系数
	return ins.get(21003)
}
func (ins *Damage_buffFormulaSheet) SetGradeRadio(v float64) { //评级技能系数
	ins.set(21003, v)
}
func (ins *Damage_buffFormulaSheet) GetAhpPrecent() float64 { //剩余血量比例
	return ins.get(22003)
}
func (ins *Damage_buffFormulaSheet) SetAhpPrecent(v float64) { //剩余血量比例
	ins.set(22003, v)
}
func (ins *Damage_buffFormulaSheet) GetAmaxHP() float64 { //最大血量
	return ins.get(23003)
}
func (ins *Damage_buffFormulaSheet) SetAmaxHP(v float64) { //最大血量
	ins.set(23003, v)
}
func (ins *Damage_buffFormulaSheet) GetAspeed() float64 { //速度
	return ins.get(24003)
}
func (ins *Damage_buffFormulaSheet) SetAspeed(v float64) { //速度
	ins.set(24003, v)
}
func (ins *Damage_buffFormulaSheet) GetAslevel() float64 { //Buff等级
	return ins.get(25003)
}
func (ins *Damage_buffFormulaSheet) SetAslevel(v float64) { //Buff等级
	ins.set(25003, v)
}
func (ins *Damage_buffFormulaSheet) GetPdefence() float64 { //物防
	return ins.get(28003)
}
func (ins *Damage_buffFormulaSheet) SetPdefence(v float64) { //物防
	ins.set(28003, v)
}
func (ins *Damage_buffFormulaSheet) GetMdefence() float64 { //特防
	return ins.get(29003)
}
func (ins *Damage_buffFormulaSheet) SetMdefence(v float64) { //特防
	ins.set(29003, v)
}
func (ins *Damage_buffFormulaSheet) GetDpplevel() float64 { //物防修
	return ins.get(30003)
}
func (ins *Damage_buffFormulaSheet) SetDpplevel(v float64) { //物防修
	ins.set(30003, v)
}
func (ins *Damage_buffFormulaSheet) GetDmplevel() float64 { //特防修
	return ins.get(31003)
}
func (ins *Damage_buffFormulaSheet) SetDmplevel(v float64) { //特防修
	ins.set(31003, v)
}
func (ins *Damage_buffFormulaSheet) GetDodge() float64 { //闪避率
	return ins.get(32003)
}
func (ins *Damage_buffFormulaSheet) SetDodge(v float64) { //闪避率
	ins.set(32003, v)
}
func (ins *Damage_buffFormulaSheet) GetTough() float64 { //韧性率
	return ins.get(33003)
}
func (ins *Damage_buffFormulaSheet) SetTough(v float64) { //韧性率
	ins.set(33003, v)
}
func (ins *Damage_buffFormulaSheet) GetDlevel() float64 { //防御者等级
	return ins.get(34003)
}
func (ins *Damage_buffFormulaSheet) SetDlevel(v float64) { //防御者等级
	ins.set(34003, v)
}
func (ins *Damage_buffFormulaSheet) GetDhpPrecent() float64 { //剩余血量比例
	return ins.get(35003)
}
func (ins *Damage_buffFormulaSheet) SetDhpPrecent(v float64) { //剩余血量比例
	ins.set(35003, v)
}
func (ins *Damage_buffFormulaSheet) GetDmaxHP() float64 { //最大血量
	return ins.get(36003)
}
func (ins *Damage_buffFormulaSheet) SetDmaxHP(v float64) { //最大血量
	ins.set(36003, v)
}
func (ins *Damage_buffFormulaSheet) GetDspeed() float64 { //速度
	return ins.get(37003)
}
func (ins *Damage_buffFormulaSheet) SetDspeed(v float64) { //速度
	ins.set(37003, v)
}
func (ins *Damage_buffFormulaSheet) GetDamage1() float64 { //计算结果
	return ins.get(2006)
}
func (ins *Damage_buffFormulaSheet) GetDamage2() float64 { //计算结果
	return ins.get(5006)
}
func (ins *Damage_buffFormulaSheet) GetDamage3() float64 { //计算结果
	return ins.get(8006)
}
func (ins *Damage_buffFormulaSheet) GetDamage4() float64 { //计算结果
	return ins.get(11006)
}
func (ins *Damage_buffFormulaSheet) GetDamage5() float64 { //计算结果
	return ins.get(14006)
}
func (ins *Damage_buffFormulaSheet) GetDamage6() float64 { //计算结果
	return ins.get(17006)
}
func (ins *Damage_buffFormulaSheet) GetDamage7() float64 { //计算结果
	return ins.get(20006)
}
func (ins *Damage_buffFormulaSheet) GetDamage100() float64 { //计算结果
	return ins.get(23006)
}
func (ins *Damage_buffFormulaSheet) GetDamage101() float64 { //计算结果
	return ins.get(26006)
}
func (ins *Damage_buffFormulaSheet) GetDamage102() float64 { //计算结果
	return ins.get(29006)
}
func (ins *Damage_buffFormulaSheet) GetDamage50() float64 { //计算结果
	return ins.get(32006)
}
func (ins *Damage_buffFormulaSheet) GetDamage60() float64 { //计算结果
	return ins.get(35006)
}
func (ins *Damage_buffFormulaSheet) GetDamage70() float64 { //计算结果
	return ins.get(38006)
}
func (ins *Damage_buffFormulaSheet) GetDamage80() float64 { //计算结果
	return ins.get(41006)
}
func (ins *Damage_buffFormulaSheet) GetDamage90() float64 { //计算结果
	return ins.get(44006)
}
func (ins *Damage_buffFormulaSheet) GetDamage1001() float64 { //计算结果
	return ins.get(47006)
}
func (ins *Damage_buffFormulaSheet) GetDamage1002() float64 { //计算结果
	return ins.get(50006)
}
func (ins *Damage_buffFormulaSheet) GetDamage1003() float64 { //计算结果
	return ins.get(53006)
}
func (ins *Damage_buffFormulaSheet) GetDamage2001() float64 { //计算结果
	return ins.get(56006)
}
func (ins *Damage_buffFormulaSheet) GetDamage2002() float64 { //计算结果
	return ins.get(59006)
}
func (ins *Damage_buffFormulaSheet) GetDamage3003() float64 { //计算结果
	return ins.get(62006)
}
func (ins *Damage_buffFormulaSheet) GetDamage4001() float64 { //计算结果
	return ins.get(65006)
}
func (ins *Damage_buffFormulaSheet) GetDamage4002() float64 { //计算结果
	return ins.get(68006)
}
