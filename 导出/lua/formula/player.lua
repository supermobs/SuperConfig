local sheet = {}
formulasheet.player = sheet
sheet.name = [[player]]
sheet.cellvalue = {}
sheet.cellformula = {}
sheet.cellabout = {}
sheet.totalrow = 1
for i = 1,sheet.totalrow do
	sheet.cellvalue[i] = {}
	sheet.cellformula[i] = {}
	sheet.cellabout[i] = {}
end

-- declares
sheet.input = {
level = 1--[[等级]]
}
sheet.output = {
levelupexp = 1--[[升级经验]]
}

-- all datas
sheet.cellvalue[1][3] = 10
sheet.cellformula[1][6] = function(ins) return (ins:get(1,3) * 999) end

-- cell data relation
sheet.cellabout[1][3] = {{1,6}}

-- enumerator
sheet.enumerator = {}
