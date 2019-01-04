package config

import (
	"encoding/json"
)

// 伤害公式.xlsx
type DamageparmTable struct {
	Name  string
	Datas map[int32]*DamageparmConfig
	Group *DamageparmTableGroup
}
type DamageparmTableGroup struct {
}

type DamageparmConfig struct {
	Id       int32   // 等级
	Roleparm float32 // 角色技能参数
	Petparm  float32 // 精灵技能参数
}

var _DamageparmIns *DamageparmTable

func loadSheetDamageparm(data []byte) error {
	tmp := new(DamageparmTable)
	err := json.Unmarshal(data, tmp)
	if err != nil {
		return err
	}
	_DamageparmIns = tmp
	return nil
}

func GetDamageparmTable() *DamageparmTable {
	return _DamageparmIns
}

func (ins *DamageparmTable) Get(id int32) *DamageparmConfig {
	data, ok := ins.Datas[id]
	if ok {
		return data
	}
	return nil
}
func data_damageparm_vlookup_1(id float64) float64 {
	return float64(GetDamageparmTable().Datas[int32(id)].Id)
}
func data_damageparm_vlookup_2(id float64) float64 {
	return float64(GetDamageparmTable().Datas[int32(id)].Roleparm)
}
func data_damageparm_vlookup_3(id float64) float64 {
	return float64(GetDamageparmTable().Datas[int32(id)].Petparm)
}
