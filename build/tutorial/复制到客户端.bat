::pushd %~dp0
::exporter.exe nowindow

set GO_CONFIG_DATA=D:\pokemon_unity\pokemon_unity\Bridge\Config\Generated
set GO_CONFIG_JSON=D:\pokemon_unity\pokemon_unity\Assets\config_data

set GO_ORG_DATA=D:\pokemon_design\output\cs\config
set GO_ORG_JSON=D:\pokemon_design\output\cs\config_data

XCOPY %GO_ORG_DATA%  %GO_CONFIG_DATA% /Y 
XCOPY %GO_ORG_JSON%  %GO_CONFIG_JSON% /Y 