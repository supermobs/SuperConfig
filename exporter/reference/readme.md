# 不同语言需要放一个基本引用文件到具体项目里面

有些代码并不是其他项目需要的话可以注释掉

## Laya

1 - 把 ConfigBase 丢到 src/config 目录里面，其他生成的 ts 文件也丢进去
2 - 导出 js 文件cd到config目录然后执行命令:
```
tsc --lib es6,dom --removeComments --outFile ../../bin/superconfig.js ./\*.ts
```
3 - js 文件已经编译到 bin 下面然后在 index.js 里面 loadLib 把 js 进来进来

[tsc语法](http://www.typescriptlang.org/docs/handbook/compiler-options.html)

同时可以使用压缩方法压缩一下
[js压缩mini文件](https://javascript-minifier.com/)

### Laya 使用代码

```
    // 设置加载委托
    SuperConfig.LoadJsonFunc = (f:string)=>{
        console.log("加载:"+f);

        return Laya.loader.getRes("res/config_data/" + f);
    }
    let cfg = SuperConfig.GetAddprobabilityTable();
    console.log(cfg);
```
