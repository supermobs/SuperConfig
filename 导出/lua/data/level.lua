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
data.ids = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31}
-- 分组数据
data.groups = {}
data.groups["maxexp"] = {}
data.groups["maxexp"][10] = {}
data.groups["maxexp|maxhp"] = {}
data.groups["maxexp|maxhp"][10] = {}
data.groups["maxexp|maxhp"][10][100] = {}
data.groups["maxexp"][20] = {}
data.groups["maxexp|maxhp"][20] = {}
data.groups["maxexp|maxhp"][20][100] = {}
data.groups["maxexp|maxhp"][20][200] = {}
data.groups["maxexp"][30] = {}
data.groups["maxexp|maxhp"][30] = {}
data.groups["maxexp|maxhp"][30][200] = {}
data.groups["maxexp|maxhp"][30][400] = {}
data.groups["maxexp"][40] = {}
data.groups["maxexp|maxhp"][40] = {}
data.groups["maxexp|maxhp"][40][400] = {}
data.groups["maxexp|maxhp"][40][800] = {}
data.groups["maxexp"][50] = {}
data.groups["maxexp|maxhp"][50] = {}
data.groups["maxexp|maxhp"][50][800] = {}
data.groups["maxexp"][60] = {}
data.groups["maxexp|maxhp"][60] = {}
data.groups["maxexp|maxhp"][60][1600] = {}
data.groups["maxexp"][70] = {}
data.groups["maxexp|maxhp"][70] = {}
data.groups["maxexp|maxhp"][70][1600] = {}
data.groups["maxexp|maxhp"][70][3200] = {}
data.groups["maxexp"][80] = {}
data.groups["maxexp|maxhp"][80] = {}
data.groups["maxexp|maxhp"][80][3200] = {}
data.groups["maxexp|maxhp"][80][6400] = {}
data.groups["maxexp"][10] = {1,2,3,4}
data.groups["maxexp|maxhp"][10][100] = {1,2,3,4}
data.groups["maxexp"][20] = {5,6,7,8}
data.groups["maxexp|maxhp"][20][100] = {5}
data.groups["maxexp|maxhp"][20][200] = {6,7,8}
data.groups["maxexp"][30] = {9,10,11,12}
data.groups["maxexp|maxhp"][30][200] = {9,10}
data.groups["maxexp|maxhp"][30][400] = {11,12}
data.groups["maxexp"][40] = {13,14,15,16}
data.groups["maxexp|maxhp"][40][400] = {13,14,15}
data.groups["maxexp|maxhp"][40][800] = {16}
data.groups["maxexp"][50] = {17,18,19,20}
data.groups["maxexp|maxhp"][50][800] = {17,18,19,20}
data.groups["maxexp"][60] = {21,22,23,24}
data.groups["maxexp|maxhp"][60][1600] = {21,22,23,24}
data.groups["maxexp"][70] = {25,26,27,28}
data.groups["maxexp|maxhp"][70][1600] = {25}
data.groups["maxexp|maxhp"][70][3200] = {26,27,28}
data.groups["maxexp"][80] = {29,30,31}
data.groups["maxexp|maxhp"][80][3200] = {29,30}
data.groups["maxexp|maxhp"][80][6400] = {31}
-- 分组数据数量
data.groupscount = {}
data.groupscount["maxexp"] = {}
data.groupscount["maxexp"].count = 8
data.groupscount["maxexp|maxhp"] = {}
data.groupscount["maxexp|maxhp"].count = 8
data.groupscount["maxexp|maxhp"][10] = {}
data.groupscount["maxexp|maxhp"][10].count = 1
data.groupscount["maxexp|maxhp"][20] = {}
data.groupscount["maxexp|maxhp"][20].count = 2
data.groupscount["maxexp|maxhp"][30] = {}
data.groupscount["maxexp|maxhp"][30].count = 2
data.groupscount["maxexp|maxhp"][40] = {}
data.groupscount["maxexp|maxhp"][40].count = 2
data.groupscount["maxexp|maxhp"][50] = {}
data.groupscount["maxexp|maxhp"][50].count = 1
data.groupscount["maxexp|maxhp"][60] = {}
data.groupscount["maxexp|maxhp"][60].count = 1
data.groupscount["maxexp|maxhp"][70] = {}
data.groupscount["maxexp|maxhp"][70].count = 2
data.groupscount["maxexp|maxhp"][80] = {}
data.groupscount["maxexp|maxhp"][80].count = 2
-- 数据内容
-- 等级.xlsx
data.lines = {}
data.lines[1] = {1,10,100,[[嘿嘿嘿]],9999999999}
data.lines[2] = {2,10,100,[[卧槽]],10000000000}
data.lines[3] = {3,10,100,[[阿达]],10000000001}
data.lines[4] = {4,10,100,[[艹艹艹]],10000000002}
data.lines[5] = {5,20,100,[[草草草草草草草草草草草草草草草]],10000000003}
data.lines[6] = {6,20,200,[[123]],10000000004}
data.lines[7] = {7,20,200,[[abc]],10000000005}
data.lines[8] = {8,20,200,[[卧槽]],10000000006}
data.lines[9] = {9,30,200,[[阿达]],10000000007}
data.lines[10] = {10,30,200,[[艹艹艹]],10000000008}
data.lines[11] = {11,30,400,[[草草草草草草草草草草草草草草草]],10000000009}
data.lines[12] = {12,30,400,[[124]],10000000010}
data.lines[13] = {13,40,400,[[abc]],10000000011}
data.lines[14] = {14,40,400,[[卧槽]],10000000012}
data.lines[15] = {15,40,400,[[阿达]],10000000013}
data.lines[16] = {16,40,800,[[艹艹艹]],10000000014}
data.lines[17] = {17,50,800,[[草草草草草草草草草草草草草草草]],10000000015}
data.lines[18] = {18,50,800,[[125]],10000000016}
data.lines[19] = {19,50,800,[[abc]],10000000017}
data.lines[20] = {20,50,800,[[卧槽]],10000000018}
data.lines[21] = {21,60,1600,[[阿达]],10000000019}
data.lines[22] = {22,60,1600,[[艹艹艹]],10000000020}
data.lines[23] = {23,60,1600,[[草草草草草草草草草草草草草草草]],10000000021}
data.lines[24] = {24,60,1600,[[126]],10000000022}
data.lines[25] = {25,70,1600,[[abc]],10000000023}
data.lines[26] = {26,70,3200,[[卧槽]],10000000024}
data.lines[27] = {27,70,3200,[[阿达]],10000000025}
data.lines[28] = {28,70,3200,[[艹艹艹]],10000000026}
data.lines[29] = {29,80,3200,[[草草草草草草草草草草草草草草草]],10000000027}
data.lines[30] = {30,80,3200,[[127]],10000000028}
data.lines[31] = {31,80,6400,[[abc]],10000000029}

return data