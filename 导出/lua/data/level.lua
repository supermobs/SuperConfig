local data = {}
data.name = [[level]]
-- 字段名
data.keys = {
"level"--[[等级]],
"maxexp"--[[升级经验]],
"maxhp"--[[最大血量]],
"txt"--[[测试文本]],
"double"--[[测试double]]
}
for i,k in ipairs(data.keys) do
	data.keys[k] = i
end
-- 原始列对应导出列
data.cols = {}
data.cols[1] = 1
data.cols[2] = 2
data.cols[3] = 3
data.cols[4] = 4
data.cols[5] = 5
-- 完整id列表
data.ids = {99009593,99027772,99031129,99041567,99044170,99088771,99107298,99115839,99117258,99161966,99167398,99187173,99190417,99190809,99220246,99221157,99244668,99275142,99335182,99355017,99442926,99451028,99534502,99552053,99660948,99662153,99673328,99696431,99723489,99776958,99834239}
-- 分组数据
data.groups = {}
data.groups["maxexp"] = {}
data.groups["maxexp"][80] = {}
data.groups["maxexp|maxhp"] = {}
data.groups["maxexp|maxhp"][80] = {}
data.groups["maxexp|maxhp"][80][6400] = {}
data.groups["maxexp"][20] = {}
data.groups["maxexp|maxhp"][20] = {}
data.groups["maxexp|maxhp"][20][200] = {}
data.groups["maxexp"][30] = {}
data.groups["maxexp|maxhp"][30] = {}
data.groups["maxexp|maxhp"][30][200] = {}
data.groups["maxexp"][10] = {}
data.groups["maxexp|maxhp"][10] = {}
data.groups["maxexp|maxhp"][10][100] = {}
data.groups["maxexp"][50] = {}
data.groups["maxexp|maxhp"][50] = {}
data.groups["maxexp|maxhp"][50][800] = {}
data.groups["maxexp"][70] = {}
data.groups["maxexp|maxhp"][70] = {}
data.groups["maxexp|maxhp"][70][1600] = {}
data.groups["maxexp|maxhp"][30][400] = {}
data.groups["maxexp"][60] = {}
data.groups["maxexp|maxhp"][60] = {}
data.groups["maxexp|maxhp"][60][1600] = {}
data.groups["maxexp|maxhp"][20][100] = {}
data.groups["maxexp"][40] = {}
data.groups["maxexp|maxhp"][40] = {}
data.groups["maxexp|maxhp"][40][400] = {}
data.groups["maxexp|maxhp"][80][3200] = {}
data.groups["maxexp|maxhp"][70][3200] = {}
data.groups["maxexp|maxhp"][40][800] = {}
data.groups["maxexp"][80] = {99009593,99221157,99662153}
data.groups["maxexp|maxhp"][80][6400] = {99009593}
data.groups["maxexp"][20] = {99027772,99167398,99696431,99776958}
data.groups["maxexp|maxhp"][20][200] = {99027772,99696431,99776958}
data.groups["maxexp"][30] = {99031129,99107298,99117258,99673328}
data.groups["maxexp|maxhp"][30][200] = {99031129,99107298}
data.groups["maxexp"][10] = {99041567,99190417,99442926,99660948}
data.groups["maxexp|maxhp"][10][100] = {99041567,99190417,99442926,99660948}
data.groups["maxexp"][50] = {99044170,99088771,99220246,99355017}
data.groups["maxexp|maxhp"][50][800] = {99044170,99088771,99220246,99355017}
data.groups["maxexp"][70] = {99115839,99244668,99552053,99834239}
data.groups["maxexp|maxhp"][70][1600] = {99115839}
data.groups["maxexp|maxhp"][30][400] = {99117258,99673328}
data.groups["maxexp"][60] = {99161966,99275142,99335182,99534502}
data.groups["maxexp|maxhp"][60][1600] = {99161966,99275142,99335182,99534502}
data.groups["maxexp|maxhp"][20][100] = {99167398}
data.groups["maxexp"][40] = {99187173,99190809,99451028,99723489}
data.groups["maxexp|maxhp"][40][400] = {99187173,99190809,99451028}
data.groups["maxexp|maxhp"][80][3200] = {99221157,99662153}
data.groups["maxexp|maxhp"][70][3200] = {99244668,99552053,99834239}
data.groups["maxexp|maxhp"][40][800] = {99723489}
-- 分组数据数量
data.groupscount = {}
data.groupscount["maxexp"] = {}
data.groupscount["maxexp"].count = 8
data.groupscount["maxexp|maxhp"] = {}
data.groupscount["maxexp|maxhp"].count = 8
data.groupscount["maxexp|maxhp"][80] = {}
data.groupscount["maxexp|maxhp"][80].count = 2
data.groupscount["maxexp|maxhp"][20] = {}
data.groupscount["maxexp|maxhp"][20].count = 2
data.groupscount["maxexp|maxhp"][30] = {}
data.groupscount["maxexp|maxhp"][30].count = 2
data.groupscount["maxexp|maxhp"][10] = {}
data.groupscount["maxexp|maxhp"][10].count = 1
data.groupscount["maxexp|maxhp"][50] = {}
data.groupscount["maxexp|maxhp"][50].count = 1
data.groupscount["maxexp|maxhp"][70] = {}
data.groupscount["maxexp|maxhp"][70].count = 2
data.groupscount["maxexp|maxhp"][60] = {}
data.groupscount["maxexp|maxhp"][60].count = 1
data.groupscount["maxexp|maxhp"][40] = {}
data.groupscount["maxexp|maxhp"][40].count = 2
-- 数据内容
-- 等级.xlsx
data.lines = {}
data.lines[99009593] = {99009593,80,6400,[[abc]],10000000029}
data.lines[99027772] = {99027772,20,200,[[abc]],10000000005}
data.lines[99031129] = {99031129,30,200,[[阿达]],10000000007}
data.lines[99041567] = {99041567,10,100,[[abc]],9999999999}
data.lines[99044170] = {99044170,50,800,[[卧槽]],10000000018}
data.lines[99088771] = {99088771,50,800,[[abc]],10000000017}
data.lines[99107298] = {99107298,30,200,[[艹艹艹]],10000000008}
data.lines[99115839] = {99115839,70,1600,[[abc]],10000000023}
data.lines[99117258] = {99117258,30,400,[[草草草草草草草草草草草草草草草]],10000000009}
data.lines[99161966] = {99161966,60,1600,[[阿达]],10000000019}
data.lines[99167398] = {99167398,20,100,[[草草草草草草草草草草草草草草草]],10000000003}
data.lines[99187173] = {99187173,40,400,[[卧槽]],10000000012}
data.lines[99190417] = {99190417,10,100,[[艹艹艹]],10000000002}
data.lines[99190809] = {99190809,40,400,[[阿达]],10000000013}
data.lines[99220246] = {99220246,50,800,[[草草草草草草草草草草草草草草草]],10000000015}
data.lines[99221157] = {99221157,80,3200,[[草草草草草草草草草草草草草草草]],10000000027}
data.lines[99244668] = {99244668,70,3200,[[阿达]],10000000025}
data.lines[99275142] = {99275142,60,1600,[[艹艹艹]],10000000020}
data.lines[99335182] = {99335182,60,1600,[[草草草草草草草草草草草草草草草]],10000000021}
data.lines[99355017] = {99355017,50,800,[[125]],10000000016}
data.lines[99442926] = {99442926,10,100,[[阿达]],10000000001}
data.lines[99451028] = {99451028,40,400,[[abc]],10000000011}
data.lines[99534502] = {99534502,60,1600,[[126]],10000000022}
data.lines[99552053] = {99552053,70,3200,[[艹艹艹]],10000000026}
data.lines[99660948] = {99660948,10,100,[[卧槽]],10000000000}
data.lines[99662153] = {99662153,80,3200,[[127]],10000000028}
data.lines[99673328] = {99673328,30,400,[[124]],10000000010}
data.lines[99696431] = {99696431,20,200,[[123]],10000000004}
data.lines[99723489] = {99723489,40,800,[[艹艹艹]],10000000014}
data.lines[99776958] = {99776958,20,200,[[卧槽]],10000000006}
data.lines[99834239] = {99834239,70,3200,[[卧槽]],10000000024}

return data