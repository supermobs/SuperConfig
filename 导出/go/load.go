package config

import (
	"comm"
	"fmt"
	"github.com/name5566/leaf/log"
	"time"
)

func init() {
	defer cfg_init_success()
	cfg_regisiter("level", loadSheetLevel)
}

func Load() {
	start := time.Now()
	loadFormulaHero()
	loadFormulaPlayer()

	ret, err := LoadConfig(false)
	if err != nil {
		log.Fatal("%v", err)
	}
	comm.RecountUseTime(start, fmt.Sprintf("load config(% v) success!!!", len(ret)))
}
