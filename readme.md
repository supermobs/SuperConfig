![工具预览图](https://github.com/supermobs/SuperConfig/blob/master/buildtool.jpg)
## 教程
***详细在build/tutorial里面有详细教程，请查看！***

## TestCase
如果直接运行build里面的快捷可执行文件的话，需要修改pathconfig文件里面的路径为自己的目录路径，实际上对应exporter.exe界面的路径

例子参考
https://github.com/superzys/SuperConfigDemo.git

## excel表
1、文件名随意 

2、sheet名纯小写字母，不同excel文件中的sheet视为同一张表 

3、不需要导出的sheet在sheet名加前缀"_" 

4、sheet第一行，填写字段名数据分组，可以进行多字段联合分组("|"分隔)，有分组逻辑的数据必须进行分组 

5、sheet第二行，字段英文名，不填写留空的列将被过滤掉，不予导出，第一列不可留空，字母全小写 

6、sheet第三行，字段中文名 

7、sheet第四行，字段类型，int整数、string字符串、float(double)浮点数32位、float64浮点数64位，在类型前加[]表示字段是数组自动，用"|"分隔值 

8、sheet第五行开始是表的数据，首字段不填写或值为0视为无效数据 

9、sheet的首字段必须是int类型，并且是全表的唯一索引 


## excel公式
1、sheet名以(F)开头即标注本页是公式，sheet名的后面部分即为公式名

2、sheet内前三列为输入列，中文名、英文名(纯小写)、默认值

3、sheet内四五六列为输出列，中文名、英文名(纯小写)、公式

4、公式表内的sheet之间不可以互相引用，但可以引用其他数据表里的内容

5、sheet内前六列以外的部分也以任意使用，建议多做中间值，减少输入改变的重算量

6、sheet内只有数字、是非是有效单元格，字符串类型单元格导出时忽略

7、支持列表
	基础运算：加+, 减-, 乘*, 除/, 乘方^
	完整支持公式：IF POWER FACT MOD MAX MIN ROUND ROUNDDOWN ROUNDUP
	受限制的公式：
				SUM			不支持矩阵
				VLOOKUP		只能以数据表的第一列做索引

## excel标签表（多语言）
1、sheet名格式为主表sheet名+下划线+纯大写的标签名，如 "level_CHT"

2、sheet二三四行跟主表一致，第一行留空

3、标签表只需要留需要修改的列

4、导表时可以指定多个标签，导出的数据会依次以首字段为依据，替换标签表的内容导出

5、单元格里填“*”代表以主表为准，不进行替换


## 其他特殊语言
**请看reference里面的语言特殊文件以及说明**

## 导出
osx的导出命令如下：
```
#!/bin/bash
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
mono --arch=32 exporter.exe nowindow cache
#mono --arch=32 exporter.exe nowindow cache label-1

echo 编译完毕
```

如果是window的话
```
pushd %~dp0

set GO_CONFIG_JSON=E:\Repository\project\Assets\config_data
set GO_ORG_JSON=D:\config\output\cs\config_data

RMDIR /S /Q %GO_ORG_JSON%
RMDIR /S /Q %GO_CONFIG_JSON% 

exporter.exe nowindow label-1

MKDIR %GO_CONFIG_JSON%
XCOPY %GO_ORG_JSON%  %GO_CONFIG_JSON% /Y /E
```
