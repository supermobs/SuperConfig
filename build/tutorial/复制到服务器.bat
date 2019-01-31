set GO_CONFIG_DATA=E:\pokemon_server\src\config
set GO_CONFIG_JSON=E:\pokemon_server\bin\config

set GO_ORG_DATA=E:\pokemon_design\output\go\config
set GO_ORG_JSON=E:\pokemon_design\output\go\config_data

del /q /s %GO_CONFIG_DATA%\*.*
del /q /s %GO_CONFIG_JSON%\*.*

XCOPY %GO_ORG_DATA%  %GO_CONFIG_DATA% /Y 
XCOPY %GO_ORG_JSON%  %GO_CONFIG_JSON% /Y 
pause