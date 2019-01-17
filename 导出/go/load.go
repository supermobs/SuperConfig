package config

import (
	"comm"
	"fmt"
	"github.com/name5566/leaf/log"
	"time"
)

func init() {
	defer cfg_init_success()
	cfg_regisiter("damageparm", loadSheetDamageparm)
	cfg_regisiter("level", loadSheetLevel)
}

func Load() {
	start := time.Now()
	loadFormulaDamage()
	loadFormulaDamage_buff()
	loadFormulaHero()
	loadFormulaPlayer()

	ret, err := LoadConfig(false)
	if err != nil {
		log.Fatal("%v", err)
	}
	comm.RecountUseTime(start, fmt.Sprintf("load config(% v) success!!!", len(ret)))
}
