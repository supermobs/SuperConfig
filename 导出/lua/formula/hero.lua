local sheet = {}
formulasheet.hero = sheet
sheet.name = [[hero]]
sheet.cellvalue = {}
sheet.cellformula = {}
sheet.cellabout = {}
sheet.totalrow = 2
for i = 1,sheet.totalrow do
	sheet.cellvalue[i] = {}
	sheet.cellformula[i] = {}
	sheet.cellabout[i] = {}
end

-- declares
sheet.input = {
level = 1--[[等级]],
ratio = 2--[[成长]]
}
sheet.output = {
maxhp = 1--[[最大血量]]
}

-- all datas
sheet.cellvalue[1][3] = 20
sheet.cellformula[1][6] = function(ins) return (formulasheet.vlookup(ins:get(1,3), [[level]], 3) + ins:get(2,8)) end
sheet.cellvalue[2][3] = 5
sheet.cellformula[2][8] = function(ins) return formulasheet.fact(ins:get(2,3)) end

-- cell data relation
sheet.cellabout[1][3] = {{1,6}}
sheet.cellabout[2][8] = {{1,6}}
sheet.cellabout[2][3] = {{2,8},{1,6}}

-- enumerator
sheet.enumerator = {}
