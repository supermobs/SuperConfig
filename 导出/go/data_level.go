package config

import (
	"encoding/json"
)

// 等级.xlsx
type LevelTable struct {
	Name  string
	Datas map[int32]*LevelConfig
	Group *LevelTableGroup
}
type LevelTableGroup struct {
	Maxexp       map[string][]int32
	Maxexp_maxhp map[string]map[string][]int32
}

type LevelConfig struct {
	Level  int32   // 等级
	Maxexp float32 // 升级经验
	Maxhp  float32 // 最大血量
	Txt    string  // 测试文本
	Double float32 // 测试double
	Aaa    int64   // 测试
}

var _LevelIns *LevelTable

func loadSheetLevel(data []byte) error {
	tmp := new(LevelTable)
	err := json.Unmarshal(data, tmp)
	if err != nil {
		return err
	}
	_LevelIns = tmp
	return nil
}

func GetLevelTable() *LevelTable {
	return _LevelIns
}

func (ins *LevelTable) Get(id int32) *LevelConfig {
	data, ok := ins.Datas[id]
	if ok {
		return data
	}
	return nil
}

func (ins *LevelTable) Get_maxexp(Maxexp string) []*LevelConfig {
	if tmp0, ok := ins.Group.Maxexp[Maxexp]; ok {
		ids := tmp0
		configs := make([]*LevelConfig, len(ids))
		for i, id := range ids {
			configs[i] = ins.Get(id)
		}
		return configs
	}
	return make([]*LevelConfig, 0)
}

func (ins *LevelTable) Get_maxexp_maxhp(Maxexp string, Maxhp string) []*LevelConfig {
	if tmp0, ok := ins.Group.Maxexp_maxhp[Maxexp]; ok {
		if tmp1, ok := tmp0[Maxhp]; ok {
			ids := tmp1
			configs := make([]*LevelConfig, len(ids))
			for i, id := range ids {
				configs[i] = ins.Get(id)
			}
			return configs
		}
	}
	return make([]*LevelConfig, 0)
}
func data_level_vlookup_1(id float64) float64 {
	return float64(GetLevelTable().Datas[int32(id)].Level)
}
func data_level_vlookup_2(id float64) float64 {
	return float64(GetLevelTable().Datas[int32(id)].Maxexp)
}
func data_level_vlookup_3(id float64) float64 {
	return float64(GetLevelTable().Datas[int32(id)].Maxhp)
}
func data_level_vlookup_5(id float64) float64 {
	return float64(GetLevelTable().Datas[int32(id)].Double)
}
func data_level_vlookup_6(id float64) float64 {
	return float64(GetLevelTable().Datas[int32(id)].Aaa)
}
