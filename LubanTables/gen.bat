set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\LubanTables\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=..\UnityApp\Assets\App.Model\Base.Model\config.gen ^
    -x outputDataDir=..\UnityApp\Assets\GameResources\LubanConfigs\GenerateDatas\json

pause