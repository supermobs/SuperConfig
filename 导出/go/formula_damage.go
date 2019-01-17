package config

type DamageFormulaSheet struct {
	formulaSheet
}

var damageFormaulaTemplate *formulaSheetTemplate

func loadFormulaDamage() {
	damageFormaulaTemplate = new(formulaSheetTemplate)
	damageFormaulaTemplate.datas = make(map[int32]float64)
	damageFormaulaTemplate.relation = make(map[int32][]int32)
	damageFormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float64)
	damageFormaulaTemplate.datas[2003] = 90
	damageFormaulaTemplate.funcs[2006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.datas[3003] = 0.84
	damageFormaulaTemplate.funcs[3006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.datas[4003] = 0.36
	damageFormaulaTemplate.funcs[4006] = func(ins *formulaSheet) float64 {
		return ins.get(7*1000 + 8)
	}
	damageFormaulaTemplate.datas[5003] = 2.05
	damageFormaulaTemplate.datas[6003] = 0
	damageFormaulaTemplate.datas[7003] = 0
	damageFormaulaTemplate.funcs[7006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[7008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.datas[8003] = 0
	damageFormaulaTemplate.funcs[8006] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))) + float64(0.5))
	}
	damageFormaulaTemplate.funcs[8008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.datas[9003] = 0
	damageFormaulaTemplate.funcs[9006] = func(ins *formulaSheet) float64 {
		return ins.get(8*1000 + 8)
	}
	damageFormaulaTemplate.funcs[9008] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))
	}
	damageFormaulaTemplate.datas[10003] = 0
	damageFormaulaTemplate.funcs[10008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.datas[11003] = 0
	damageFormaulaTemplate.funcs[11008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.datas[12006] = 1
	damageFormaulaTemplate.funcs[12008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.funcs[13006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[13008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))
	}
	damageFormaulaTemplate.datas[14003] = 270
	damageFormaulaTemplate.funcs[14006] = func(ins *formulaSheet) float64 {
		return ins.get(9*1000 + 8)
	}
	damageFormaulaTemplate.funcs[14008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[15003] = 270
	damageFormaulaTemplate.funcs[15008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[16003] = 0
	damageFormaulaTemplate.funcs[16008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[17003] = 1
	damageFormaulaTemplate.datas[17006] = 1
	damageFormaulaTemplate.funcs[17008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[18003] = 0.05
	damageFormaulaTemplate.funcs[18006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[18008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[19003] = 4
	damageFormaulaTemplate.funcs[19006] = func(ins *formulaSheet) float64 {
		return ins.get(10*1000 + 8)
	}
	damageFormaulaTemplate.funcs[19008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[20003] = 1
	damageFormaulaTemplate.funcs[20008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))
	}
	damageFormaulaTemplate.datas[21003] = 1
	damageFormaulaTemplate.funcs[21008] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))
	}
	damageFormaulaTemplate.datas[22003] = 1
	damageFormaulaTemplate.datas[22006] = 1
	damageFormaulaTemplate.funcs[22008] = func(ins *formulaSheet) float64 {
		return (float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))) * float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))))
	}
	damageFormaulaTemplate.datas[23003] = 795
	damageFormaulaTemplate.funcs[23006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[23008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))) * float64(0.6))
	}
	damageFormaulaTemplate.datas[24003] = 100
	damageFormaulaTemplate.funcs[24006] = func(ins *formulaSheet) float64 {
		return ins.get(11*1000 + 8)
	}
	damageFormaulaTemplate.funcs[24008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))) + float64((float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))))) * float64(0.5))
	}
	damageFormaulaTemplate.datas[25003] = 19
	damageFormaulaTemplate.funcs[25008] = func(ins *formulaSheet) float64 {
		return (float64((float64((float64((float64((float64((float64((float64(ins.get(14*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(14*1000+3)))) / float64((float64(ins.get(14*1000+3)) + float64((float64(2) * float64(ins.get(28*1000+3)))))))) + float64((float64((float64((float64((float64(ins.get(15*1000+3)) * float64(ins.get(3*1000+3)))) + float64((float64((float64(ins.get(25*1000+3)) * float64(ins.get(21*1000+3)))) * float64(ins.get(4*1000+3)))))) * float64(ins.get(15*1000+3)))) / float64((float64(ins.get(15*1000+3)) + float64((float64(2) * float64(ins.get(29*1000+3)))))))))) * float64(0.5))) * float64((float64(0.3) + float64((float64(0.6) / float64(ins.get(5*1000+3)))))))
	}
	damageFormaulaTemplate.funcs[27006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.datas[28003] = 3
	damageFormaulaTemplate.funcs[28006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.datas[29003] = 3
	damageFormaulaTemplate.funcs[29006] = func(ins *formulaSheet) float64 {
		return ins.get(12*1000 + 8)
	}
	damageFormaulaTemplate.datas[30003] = 0
	damageFormaulaTemplate.datas[31003] = 0
	damageFormaulaTemplate.datas[32003] = 0
	damageFormaulaTemplate.funcs[32006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.datas[33003] = 0
	damageFormaulaTemplate.funcs[33006] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))) + float64(0.5))
	}
	damageFormaulaTemplate.datas[34003] = 1
	damageFormaulaTemplate.funcs[34006] = func(ins *formulaSheet) float64 {
		return ins.get(13*1000 + 8)
	}
	damageFormaulaTemplate.datas[35003] = 1
	damageFormaulaTemplate.datas[36003] = 278103
	damageFormaulaTemplate.datas[37003] = 100
	damageFormaulaTemplate.funcs[37006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[38006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[39006] = func(ins *formulaSheet) float64 {
		return ins.get(14*1000 + 8)
	}
	damageFormaulaTemplate.datas[42006] = 1
	damageFormaulaTemplate.funcs[43006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[44006] = func(ins *formulaSheet) float64 {
		return ins.get(15*1000 + 8)
	}
	damageFormaulaTemplate.funcs[47006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[48006] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))) + float64(0.5))
	}
	damageFormaulaTemplate.funcs[49006] = func(ins *formulaSheet) float64 {
		return ins.get(14*1000 + 8)
	}
	damageFormaulaTemplate.funcs[52006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[53006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[54006] = func(ins *formulaSheet) float64 {
		return ins.get(17*1000 + 8)
	}
	damageFormaulaTemplate.funcs[57006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[58006] = func(ins *formulaSheet) float64 {
		return (float64((float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))) + float64(0.5))
	}
	damageFormaulaTemplate.funcs[59006] = func(ins *formulaSheet) float64 {
		return ins.get(18*1000 + 8)
	}
	damageFormaulaTemplate.funcs[62006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[63006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[64006] = func(ins *formulaSheet) float64 {
		return ins.get(19*1000 + 8)
	}
	damageFormaulaTemplate.datas[67006] = 1
	damageFormaulaTemplate.funcs[68006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[69006] = func(ins *formulaSheet) float64 {
		return ins.get(20*1000 + 8)
	}
	damageFormaulaTemplate.datas[72006] = 1
	damageFormaulaTemplate.datas[73006] = 0
	damageFormaulaTemplate.funcs[74006] = func(ins *formulaSheet) float64 {
		return ins.get(21*1000 + 8)
	}
	damageFormaulaTemplate.datas[77006] = 1
	damageFormaulaTemplate.datas[78006] = 0
	damageFormaulaTemplate.funcs[79006] = func(ins *formulaSheet) float64 {
		return ins.get(22*1000 + 8)
	}
	damageFormaulaTemplate.datas[82006] = 1
	damageFormaulaTemplate.datas[83006] = 0
	damageFormaulaTemplate.funcs[84006] = func(ins *formulaSheet) float64 {
		return ins.get(22*1000 + 8)
	}
	damageFormaulaTemplate.funcs[87006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[88006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[89006] = func(ins *formulaSheet) float64 {
		return ins.get(24*1000 + 8)
	}
	damageFormaulaTemplate.funcs[92006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(17*1000+3)) - float64(ins.get(32*1000+3)))
	}
	damageFormaulaTemplate.funcs[93006] = func(ins *formulaSheet) float64 {
		return (float64(ins.get(18*1000+3)) - float64(ins.get(33*1000+3)))
	}
	damageFormaulaTemplate.funcs[94006] = func(ins *formulaSheet) float64 {
		return ins.get(25*1000 + 8)
	}
	damageFormaulaTemplate.relation[17003] = []int32{2006, 7006, 27006, 32006, 37006, 47006, 52006, 57006, 62006, 87006, 92006}
	damageFormaulaTemplate.relation[32003] = []int32{2006, 7006, 27006, 32006, 37006, 47006, 52006, 57006, 62006, 87006, 92006}
	damageFormaulaTemplate.relation[18003] = []int32{3006, 8006, 13006, 18006, 23006, 28006, 33006, 38006, 43006, 48006, 53006, 58006, 63006, 68006, 88006, 93006}
	damageFormaulaTemplate.relation[33003] = []int32{3006, 8006, 13006, 18006, 23006, 28006, 33006, 38006, 43006, 48006, 53006, 58006, 63006, 68006, 88006, 93006}
	damageFormaulaTemplate.relation[7008] = []int32{4006}
	damageFormaulaTemplate.relation[14003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 24008, 25008, 4006, 9006, 14006, 19006, 24006, 29006, 34006, 89006, 94006}
	damageFormaulaTemplate.relation[3003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 4006, 9006, 14006, 19006, 24006, 29006, 34006, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 74006, 79006, 84006, 89006, 94006}
	damageFormaulaTemplate.relation[25003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 4006, 9006, 14006, 19006, 24006, 29006, 34006, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 74006, 79006, 84006, 89006, 94006}
	damageFormaulaTemplate.relation[21003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 4006, 9006, 14006, 19006, 24006, 29006, 34006, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 74006, 79006, 84006, 89006, 94006}
	damageFormaulaTemplate.relation[4003] = []int32{7008, 8008, 9008, 10008, 11008, 12008, 13008, 14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 4006, 9006, 14006, 19006, 24006, 29006, 34006, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 74006, 79006, 84006, 89006, 94006}
	damageFormaulaTemplate.relation[28003] = []int32{7008, 8008, 10008, 11008, 12008, 13008, 24008, 25008, 4006, 9006, 19006, 24006, 29006, 34006, 89006, 94006}
	damageFormaulaTemplate.relation[8008] = []int32{9006}
	damageFormaulaTemplate.relation[5003] = []int32{12008, 13008, 17008, 18008, 19008, 20008, 22008, 25008, 29006, 34006, 54006, 59006, 64006, 69006, 79006, 84006, 94006}
	damageFormaulaTemplate.relation[9008] = []int32{14006}
	damageFormaulaTemplate.relation[15003] = []int32{14008, 15008, 16008, 17008, 18008, 19008, 20008, 21008, 22008, 23008, 24008, 25008, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 74006, 79006, 84006, 89006, 94006}
	damageFormaulaTemplate.relation[29003] = []int32{14008, 15008, 16008, 17008, 18008, 19008, 20008, 23008, 24008, 25008, 39006, 49006, 44006, 54006, 59006, 64006, 69006, 89006, 94006}
	damageFormaulaTemplate.relation[10008] = []int32{19006}
	damageFormaulaTemplate.relation[11008] = []int32{24006}
	damageFormaulaTemplate.relation[12008] = []int32{29006}
	damageFormaulaTemplate.relation[13008] = []int32{34006}
	damageFormaulaTemplate.relation[14008] = []int32{39006, 49006}
	damageFormaulaTemplate.relation[15008] = []int32{44006}
	damageFormaulaTemplate.relation[17008] = []int32{54006}
	damageFormaulaTemplate.relation[18008] = []int32{59006}
	damageFormaulaTemplate.relation[19008] = []int32{64006}
	damageFormaulaTemplate.relation[20008] = []int32{69006}
	damageFormaulaTemplate.relation[21008] = []int32{74006}
	damageFormaulaTemplate.relation[22008] = []int32{79006, 84006}
	damageFormaulaTemplate.relation[24008] = []int32{89006}
	damageFormaulaTemplate.relation[25008] = []int32{94006}
}
func NewDamageFormula() *DamageFormulaSheet {
	formula := new(DamageFormulaSheet)
	formula.template = damageFormaulaTemplate
	formula.datas = make(map[int32]float64)
	return formula
}
func (ins *DamageFormulaSheet) GetParam1() float64 { //额外参数1（公式序号）
	return ins.get(2003)
}
func (ins *DamageFormulaSheet) SetParam1(v float64) { //额外参数1（公式序号）
	ins.set(2003, v)
}
func (ins *DamageFormulaSheet) GetParam2() float64 { //额外参数2（攻击力系数）
	return ins.get(3003)
}
func (ins *DamageFormulaSheet) SetParam2(v float64) { //额外参数2（攻击力系数）
	ins.set(3003, v)
}
func (ins *DamageFormulaSheet) GetParam3() float64 { //额外参数3（绝对衰减系数）
	return ins.get(4003)
}
func (ins *DamageFormulaSheet) SetParam3(v float64) { //额外参数3（绝对衰减系数）
	ins.set(4003, v)
}
func (ins *DamageFormulaSheet) GetParam4() float64 { //额外参数4（目标个数）
	return ins.get(5003)
}
func (ins *DamageFormulaSheet) SetParam4(v float64) { //额外参数4（目标个数）
	ins.set(5003, v)
}
func (ins *DamageFormulaSheet) GetParam5() float64 { //额外参数5（BUFF添加概率）
	return ins.get(6003)
}
func (ins *DamageFormulaSheet) SetParam5(v float64) { //额外参数5（BUFF添加概率）
	ins.set(6003, v)
}
func (ins *DamageFormulaSheet) GetParam6() float64 { //额外参数6（BUFFid）
	return ins.get(7003)
}
func (ins *DamageFormulaSheet) SetParam6(v float64) { //额外参数6（BUFFid）
	ins.set(7003, v)
}
func (ins *DamageFormulaSheet) GetParam7() float64 { //额外参数7
	return ins.get(8003)
}
func (ins *DamageFormulaSheet) SetParam7(v float64) { //额外参数7
	ins.set(8003, v)
}
func (ins *DamageFormulaSheet) GetParam8() float64 { //额外参数8
	return ins.get(9003)
}
func (ins *DamageFormulaSheet) SetParam8(v float64) { //额外参数8
	ins.set(9003, v)
}
func (ins *DamageFormulaSheet) GetParam9() float64 { //额外参数9
	return ins.get(10003)
}
func (ins *DamageFormulaSheet) SetParam9(v float64) { //额外参数9
	ins.set(10003, v)
}
func (ins *DamageFormulaSheet) GetParam10() float64 { //额外参数10
	return ins.get(11003)
}
func (ins *DamageFormulaSheet) SetParam10(v float64) { //额外参数10
	ins.set(11003, v)
}
func (ins *DamageFormulaSheet) GetPattack() float64 { //物攻
	return ins.get(14003)
}
func (ins *DamageFormulaSheet) SetPattack(v float64) { //物攻
	ins.set(14003, v)
}
func (ins *DamageFormulaSheet) GetMattack() float64 { //特攻
	return ins.get(15003)
}
func (ins *DamageFormulaSheet) SetMattack(v float64) { //特攻
	ins.set(15003, v)
}
func (ins *DamageFormulaSheet) GetAplevel() float64 { //攻修
	return ins.get(16003)
}
func (ins *DamageFormulaSheet) SetAplevel(v float64) { //攻修
	ins.set(16003, v)
}
func (ins *DamageFormulaSheet) GetHit() float64 { //命中率
	return ins.get(17003)
}
func (ins *DamageFormulaSheet) SetHit(v float64) { //命中率
	ins.set(17003, v)
}
func (ins *DamageFormulaSheet) GetCrit() float64 { //暴击率
	return ins.get(18003)
}
func (ins *DamageFormulaSheet) SetCrit(v float64) { //暴击率
	ins.set(18003, v)
}
func (ins *DamageFormulaSheet) GetAlevel() float64 { //攻击者等级
	return ins.get(19003)
}
func (ins *DamageFormulaSheet) SetAlevel(v float64) { //攻击者等级
	ins.set(19003, v)
}
func (ins *DamageFormulaSheet) GetEvolveRadio() float64 { //进化技能系数
	return ins.get(20003)
}
func (ins *DamageFormulaSheet) SetEvolveRadio(v float64) { //进化技能系数
	ins.set(20003, v)
}
func (ins *DamageFormulaSheet) GetGradeRadio() float64 { //评级技能系数
	return ins.get(21003)
}
func (ins *DamageFormulaSheet) SetGradeRadio(v float64) { //评级技能系数
	ins.set(21003, v)
}
func (ins *DamageFormulaSheet) GetAhpPrecent() float64 { //剩余血量比例
	return ins.get(22003)
}
func (ins *DamageFormulaSheet) SetAhpPrecent(v float64) { //剩余血量比例
	ins.set(22003, v)
}
func (ins *DamageFormulaSheet) GetAmaxHP() float64 { //最大血量
	return ins.get(23003)
}
func (ins *DamageFormulaSheet) SetAmaxHP(v float64) { //最大血量
	ins.set(23003, v)
}
func (ins *DamageFormulaSheet) GetAspeed() float64 { //速度
	return ins.get(24003)
}
func (ins *DamageFormulaSheet) SetAspeed(v float64) { //速度
	ins.set(24003, v)
}
func (ins *DamageFormulaSheet) GetAslevel() float64 { //攻击技能等级系数
	return ins.get(25003)
}
func (ins *DamageFormulaSheet) SetAslevel(v float64) { //攻击技能等级系数
	ins.set(25003, v)
}
func (ins *DamageFormulaSheet) GetPdefence() float64 { //物防
	return ins.get(28003)
}
func (ins *DamageFormulaSheet) SetPdefence(v float64) { //物防
	ins.set(28003, v)
}
func (ins *DamageFormulaSheet) GetMdefence() float64 { //特防
	return ins.get(29003)
}
func (ins *DamageFormulaSheet) SetMdefence(v float64) { //特防
	ins.set(29003, v)
}
func (ins *DamageFormulaSheet) GetDpplevel() float64 { //物防修
	return ins.get(30003)
}
func (ins *DamageFormulaSheet) SetDpplevel(v float64) { //物防修
	ins.set(30003, v)
}
func (ins *DamageFormulaSheet) GetDmplevel() float64 { //特防修
	return ins.get(31003)
}
func (ins *DamageFormulaSheet) SetDmplevel(v float64) { //特防修
	ins.set(31003, v)
}
func (ins *DamageFormulaSheet) GetDodge() float64 { //闪避率
	return ins.get(32003)
}
func (ins *DamageFormulaSheet) SetDodge(v float64) { //闪避率
	ins.set(32003, v)
}
func (ins *DamageFormulaSheet) GetTough() float64 { //韧性率
	return ins.get(33003)
}
func (ins *DamageFormulaSheet) SetTough(v float64) { //韧性率
	ins.set(33003, v)
}
func (ins *DamageFormulaSheet) GetDlevel() float64 { //防御者等级
	return ins.get(34003)
}
func (ins *DamageFormulaSheet) SetDlevel(v float64) { //防御者等级
	ins.set(34003, v)
}
func (ins *DamageFormulaSheet) GetDhpPrecent() float64 { //剩余血量比例
	return ins.get(35003)
}
func (ins *DamageFormulaSheet) SetDhpPrecent(v float64) { //剩余血量比例
	ins.set(35003, v)
}
func (ins *DamageFormulaSheet) GetDmaxHP() float64 { //最大血量
	return ins.get(36003)
}
func (ins *DamageFormulaSheet) SetDmaxHP(v float64) { //最大血量
	ins.set(36003, v)
}
func (ins *DamageFormulaSheet) GetDspeed() float64 { //速度
	return ins.get(37003)
}
func (ins *DamageFormulaSheet) SetDspeed(v float64) { //速度
	ins.set(37003, v)
}
func (ins *DamageFormulaSheet) GetHitratio10() float64 { //命中概率
	return ins.get(2006)
}
func (ins *DamageFormulaSheet) GetCritratio10() float64 { //暴击概率
	return ins.get(3006)
}
func (ins *DamageFormulaSheet) GetDamage10() float64 { //正常伤害结果
	return ins.get(4006)
}
func (ins *DamageFormulaSheet) GetHitratio11() float64 { //命中概率
	return ins.get(7006)
}
func (ins *DamageFormulaSheet) GetCritratio11() float64 { //暴击概率
	return ins.get(8006)
}
func (ins *DamageFormulaSheet) GetDamage11() float64 { //正常伤害结果
	return ins.get(9006)
}
func (ins *DamageFormulaSheet) GetHitratio12() float64 { //命中概率
	return ins.get(12006)
}
func (ins *DamageFormulaSheet) GetCritratio12() float64 { //暴击概率
	return ins.get(13006)
}
func (ins *DamageFormulaSheet) GetDamage12() float64 { //正常伤害结果
	return ins.get(14006)
}
func (ins *DamageFormulaSheet) GetHitratio13() float64 { //命中概率
	return ins.get(17006)
}
func (ins *DamageFormulaSheet) GetCritratio13() float64 { //暴击概率
	return ins.get(18006)
}
func (ins *DamageFormulaSheet) GetDamage13() float64 { //正常伤害结果
	return ins.get(19006)
}
func (ins *DamageFormulaSheet) GetHitratio14() float64 { //命中概率
	return ins.get(22006)
}
func (ins *DamageFormulaSheet) GetCritratio14() float64 { //暴击概率
	return ins.get(23006)
}
func (ins *DamageFormulaSheet) GetDamage14() float64 { //正常伤害结果
	return ins.get(24006)
}
func (ins *DamageFormulaSheet) GetHitratio20() float64 { //命中概率
	return ins.get(27006)
}
func (ins *DamageFormulaSheet) GetCritratio20() float64 { //暴击概率
	return ins.get(28006)
}
func (ins *DamageFormulaSheet) GetDamage20() float64 { //正常伤害结果
	return ins.get(29006)
}
func (ins *DamageFormulaSheet) GetHitratio21() float64 { //命中概率
	return ins.get(32006)
}
func (ins *DamageFormulaSheet) GetCritratio21() float64 { //暴击概率
	return ins.get(33006)
}
func (ins *DamageFormulaSheet) GetDamage21() float64 { //正常伤害结果
	return ins.get(34006)
}
func (ins *DamageFormulaSheet) GetHitratio30() float64 { //命中概率
	return ins.get(37006)
}
func (ins *DamageFormulaSheet) GetCritratio30() float64 { //暴击概率
	return ins.get(38006)
}
func (ins *DamageFormulaSheet) GetDamage30() float64 { //正常伤害结果
	return ins.get(39006)
}
func (ins *DamageFormulaSheet) GetHitratio31() float64 { //命中概率
	return ins.get(42006)
}
func (ins *DamageFormulaSheet) GetCritratio31() float64 { //暴击概率
	return ins.get(43006)
}
func (ins *DamageFormulaSheet) GetDamage31() float64 { //正常伤害结果
	return ins.get(44006)
}
func (ins *DamageFormulaSheet) GetHitratio32() float64 { //命中概率
	return ins.get(47006)
}
func (ins *DamageFormulaSheet) GetCritratio32() float64 { //暴击概率
	return ins.get(48006)
}
func (ins *DamageFormulaSheet) GetDamage32() float64 { //正常伤害结果
	return ins.get(49006)
}
func (ins *DamageFormulaSheet) GetHitratio40() float64 { //命中概率
	return ins.get(52006)
}
func (ins *DamageFormulaSheet) GetCritratio40() float64 { //暴击概率
	return ins.get(53006)
}
func (ins *DamageFormulaSheet) GetDamage40() float64 { //正常伤害结果
	return ins.get(54006)
}
func (ins *DamageFormulaSheet) GetHitratio41() float64 { //命中概率
	return ins.get(57006)
}
func (ins *DamageFormulaSheet) GetCritratio41() float64 { //暴击概率
	return ins.get(58006)
}
func (ins *DamageFormulaSheet) GetDamage41() float64 { //正常伤害结果
	return ins.get(59006)
}
func (ins *DamageFormulaSheet) GetHitratio42() float64 { //命中概率
	return ins.get(62006)
}
func (ins *DamageFormulaSheet) GetCritratio42() float64 { //暴击概率
	return ins.get(63006)
}
func (ins *DamageFormulaSheet) GetDamage42() float64 { //正常伤害结果
	return ins.get(64006)
}
func (ins *DamageFormulaSheet) GetHitratio43() float64 { //命中概率
	return ins.get(67006)
}
func (ins *DamageFormulaSheet) GetCritratio43() float64 { //暴击概率
	return ins.get(68006)
}
func (ins *DamageFormulaSheet) GetDamage43() float64 { //正常伤害结果
	return ins.get(69006)
}
func (ins *DamageFormulaSheet) GetHitratio1010() float64 { //命中概率
	return ins.get(72006)
}
func (ins *DamageFormulaSheet) GetCritratio1010() float64 { //暴击概率
	return ins.get(73006)
}
func (ins *DamageFormulaSheet) GetDamage1010() float64 { //正常伤害结果
	return ins.get(74006)
}
func (ins *DamageFormulaSheet) GetHitratio1020() float64 { //命中概率
	return ins.get(77006)
}
func (ins *DamageFormulaSheet) GetCritratio1020() float64 { //暴击概率
	return ins.get(78006)
}
func (ins *DamageFormulaSheet) GetDamage1020() float64 { //正常伤害结果
	return ins.get(79006)
}
func (ins *DamageFormulaSheet) GetHitratio70() float64 { //命中概率
	return ins.get(82006)
}
func (ins *DamageFormulaSheet) GetCritratio70() float64 { //暴击概率
	return ins.get(83006)
}
func (ins *DamageFormulaSheet) GetDamage70() float64 { //正常伤害结果
	return ins.get(84006)
}
func (ins *DamageFormulaSheet) GetHitratio80() float64 { //命中概率
	return ins.get(87006)
}
func (ins *DamageFormulaSheet) GetCritratio80() float64 { //暴击概率
	return ins.get(88006)
}
func (ins *DamageFormulaSheet) GetDamage80() float64 { //正常伤害结果
	return ins.get(89006)
}
func (ins *DamageFormulaSheet) GetHitratio90() float64 { //命中概率
	return ins.get(92006)
}
func (ins *DamageFormulaSheet) GetCritratio90() float64 { //暴击概率
	return ins.get(93006)
}
func (ins *DamageFormulaSheet) GetDamage90() float64 { //正常伤害结果
	return ins.get(94006)
}
