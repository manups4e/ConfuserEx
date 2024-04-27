# ConfuserEx

[![Build status][img_build]][build]
[![Test status][img_test]][test]
[![CodeFactor][img_codefactor]][codefactor]
[![Gitter Chat][img_gitter]][gitter]
[![MIT License][img_license]][license]

ConfuserEx is a open-source protector for .NET applications.
It is the successor of [Confuser][confuser] project.

ConfuserEx是一个.NET应用程序的开源混淆加密程序。
它是[Confuser][confuser]项目的继承者。

## Features

* Supports .NET Framework 2.0/3.0/3.5/4.0/4.5/4.6/4.7/4.8
* Symbol renaming (Support WPF/BAML)
* Protection against debuggers/profilers
* Protection against memory dumping
* Protection against tampering (method encryption)
* Control flow obfuscation
* Constant/resources encryption
* Reference hiding proxies
* Disable decompilers
* Embedding dependency
* Compressing output
* Extensible plugin API
* Many more are coming!

# 功能支持.NET Framework 2.0/3.0/3.5/4.0/4.5/4.6/4.7/4.8
符号重命名（支持WPF/BAML）
针对调试器/探查器的保护
防止内存转储
防篡改保护（方法加密）
控制流混淆
常量/资源加密
引用隐藏代理
禁用反编译器
嵌入依赖项
压缩输出
可扩展插件API
更多功能等你来
# Usage
# Improved Features 对原版改进功能
english | 中文
:------------ | :-------------
Automatically load existing plug-ins from the plug-in directory | 从插件目录中自动加载存在的插件
Add Chinese language resources | 添加中文语言资源

# Custom Plugins 自定义插件模块清单
protection | english | 中文
:------------ | :------------ | :-------------
AntiDe4dot | Prevents usage of De4Dot. | 防止使用De4Dot反编译
AntiWatermark | Removes the ProtectedBy watermark to prevent Protector detection. | 删除水印，以防止被检测到混淆器
AntiDump | Prevents the assembly from being dumped from memory. | 禁止从内存转储程序集
Constant | This protection encodes and compresses constants in the code. | 对代码中的常量进行编码和压缩
EreaseHeader | Overwrites the whole PE Header. | 清除整个PE标头
OpCodeProt | Protects OpCodes such as Ldlfd. | 混淆操作码，例如Ldlfd
FakeObfuscator | Confuses obfuscators like de4dot by adding types typical to other obfuscators. | 将典型混淆器类型添加到模块中，用于防止反混淆器检测。（如 de4dot）
LocaltoField | This protection marks the module with a attribute that discourage ILDasm from disassembling it. | 用特性来标记模块，禁止ILDasm反编译它
Anti DnSpy | Prevents assembly execution if dnspy is detected on disk | 防止使用DnSpy调试程序集
Anti Virtual Machine | Prevents the assembly from running on a virtual machine. | 禁止程序集在虚拟机上运行。
Mutate Constants | Mutate Contants with sizeofs. | 
New Control Flow | modified version of IntControlFlow from GabTeix. | 新的流程混淆模块
Reduce Metadata Optimization | Reduces the size of assembly by removing unnecessary metadata such as parameter names, duplicate literal strings, etc. | 通过删除不必要的元数据（如参数名、重复的文本字符串等）来减小程序集的大小。

# Usage 用法
```Batchfile
Confuser.CLI.exe <path to project file>
```

The project file is a ConfuserEx Project (`*.crproj`).
The format of project file can be found in [docs\ProjectFormat.md][project_format]

# Advanced function configuration 高级功能配置
#### 声明程序集特性，禁用水印：
```C#
[assembly: Obfuscation(Exclude = False, Feature = "-watermark")]
```
#### 声明类的特性，某个类禁用重命名：
```C#
[Obfuscation(Exclude=false, Feature="-rename")]
```

## 声明单独的规则，以包含或排除需要重命名或不需要重命名的规则：

#### 根据特定的[命名空间]排除[重命名]：
```XML
<rule pattern="true">
  <protection id="rename" />
</rule>
<rule pattern="namespace('Your.Namespace.Here') and is-type('enum')">
  <protection id="rename" action="remove" />
</rule>
```

#### 根据特定的[命名空间]排除对公开类型的[重命名]：
```XML
<rule pattern="inherits('System.Web.UI.Page')" preset="none">
    <protection id="rename">
      <argument name="renPublic" value="false" />
    </protection>
</rule>
```
#### 根据特定的[类名]排除[重命名]：
```XML
<rule pattern="match-type-name('DataObject')" inherit="false">
    <protection id="rename" action="remove" />
</rule>
```
#### 针对[非公开]的，并且是有[可序列化]特性的类进行强制重命名：
```XML
<rule pattern="not is-public() and is-type('serializable')">
  <protection id="rename">
    <argument name="forceRen" value="true" />
  </protection>
</rule>
```
#### 针对所有继承[枚举类型]的，并且是[公开]的类型，不要重命名：
```XML
<rule pattern="inherits('System.Enum') and is-public()" inherit="false">
    <protection id="rename" action="remove" />
</rule>
```

#### 规则可用解析模式列表：
```XML
or
and
not
module           判断模块名称
namespace        判断命名空间完全匹配，支持一个参数，支持正则
member-type      判断成员类型，支持一个参数，可用参数为type、method、propertym getter、propertym setter、eventm add、eventm remove、eventm fire、other、field、property、event、module
name             完全匹配名称(Name)，不支持正则
match            匹配全名称(FullName)，支持正则，非完全匹配
match-name       匹配名称(Name)，支持正则，非完全匹配
match-type-name  匹配类型名称，支持正则，非完全匹配

is-type    判断是否指定类型，支持一个参数，可用参数为enum、interface、valuetype、delegate、abstract、nested、serializable
is-public  指示成员可见性
inherits   指示类型是否从指定类型继承
has-attr   比较是否具有指定特性（attribute）
full-name  比较定义全名（definition）
decl-type  比较声明类型全名（declaring type）
```

```XML
    <protection id="rename">
      <argument name="renameArgs" value="true" />
      <argument name="renPublic" value="true" />
      <argument name="mode" value="decodable" />
      <argument name="password" value="This password is secret" />
    </protection>
    
    重命名相关参数列表：
    renameArgs   是否重命名参数
    forceRen     是否强制重命名
    renPublic    是否重命名公开类型
    renEnum      是否重命名枚举
    password     重命名时生成新名称的密码
```

# Bug Report BUG反馈

See the [Issues Report][issues] section of website.

# License

Licensed under the MIT license. See [LICENSE.md][license] for details.

# Credits

**[0xd4d]** for his awesome work and extensive knowledge!

[0xd4d]: https://github.com/0xd4d
[build]: https://ci.appveyor.com/project/mkaring/confuserex/branch/master
[codefactor]: https://www.codefactor.io/repository/github/mkaring/confuserex/overview/master
[confuser]: http://confuser.codeplex.com
[issues]: https://github.com/mkaring/ConfuserEx/issues
[gitter]: https://gitter.im/ConfuserEx/community
[license]: LICENSE.md
[project_format]: docs/ProjectFormat.md
[test]: https://ci.appveyor.com/project/mkaring/confuserex/branch/master/tests

[img_build]: https://img.shields.io/appveyor/ci/mkaring/ConfuserEx/master.svg?style=flat
[img_codefactor]: https://www.codefactor.io/repository/github/mkaring/confuserex/badge/master
[img_gitter]: https://img.shields.io/gitter/room/mkaring/ConfuserEx.svg?style=flat
[img_license]: https://img.shields.io/github/license/mkaring/ConfuserEx.svg?style=flat
[img_test]: https://img.shields.io/appveyor/tests/mkaring/ConfuserEx/master.svg?style=flat&compact_message
