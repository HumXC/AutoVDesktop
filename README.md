# AutoVDesktop

Quickly change files and ICONS on the desktop after switching virtual desktops。

在使用 win+tab 或者 ctrl+win+←/→ 切换虚拟桌面之后，切换对于的桌面文件和图标。

## 声明

程序中实现保存和恢复桌面图标功能的代码(DesktopRestorer)来自[此链接](https://www.codeproject.com/Articles/639486/Save-and-Restore-Icon-Positions-on-Desktop?msg=5864404#xx5864404xx)，我修改并使用了此段代码，DesktopRestorer 代码的原作者不需要对此程序产生的任何后果负责。

## 使用演示

![使用演示](https://raw.githubusercontent.com/HumXC/AutoVDesktop/main/Readme/demo.gif)

## 关于虚拟桌面

由于该程序的核心功能就是通过 "虚拟桌面" 来触发，所以你有必要知道[什么是 "虚拟桌面"](https://sspai.com/post/45594)。

## 你应该知道的事情

一定不要跳过这个

-   程序依赖 .NET 6.0 环境，如果没有则程序会弹出如下提示，点击确定后[跳转到](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime?cid=getdotnetcore)浏览器下载 Run desktop apps 的运行时(Runtime)：
    ![.NET6下载页](https://files.catbox.moe/58e8ee.png)
-   该程序只会修改程序目录下的文件，不会删除你的文件。
-   切换桌面之前刷新桌面可能会导致图标错位。
-   你有必要知道你的 PC 当前桌面的路径，一般在"C:\用户\用户名\Desktop"，程序运行时创建的桌面文件夹会在默认桌面的父文件夹下。如果你在配置文件里添加了一个"Game"桌面，那么这个桌面的路径就会在"C:\用户\用户名\Game"。
-   使用之前建议先修改桌面位置到一个单独的目录下，具体方法：[设置桌面路径](https://zhuanlan.zhihu.com/p/78243921)。比如我的将桌面路径设置为“D:\\\\Desktops\Code"那么在切换到 Game 桌面后生成的 Game 桌面文件夹就会在 Code 文件夹的同级目录下:
    ![设置桌面位置](https://files.catbox.moe/getpk3.png)

## 常见问题

-   Q：怎么快速切换桌面？
    A：按 ctrl+win+左右方向键。
-   Q：我不想用了，我想恢复回原来的桌面该怎么做？
    A: 看这个链接，修改回你以前的桌面路径 [设置桌面路径](https://zhuanlan.zhihu.com/p/78243921)

## 配置说明

-   响应延迟: 在切换虚拟桌面后等待的时间，默认是 1000 毫秒，即在切换桌面后等 1 秒再切换桌面，建议值为 500。
-   恢复图标: 切换虚拟桌面之后是否恢复图标的位置，默认开启，如果不开启的话，切换虚拟桌面之后桌面图标会重新排列。关闭此选项会让程序响应更快。<!>注意，开启此功能后更换桌面之前尽量不要刷新桌面，不然前几列的图标会有概率乱序。(标记一个 bug，但是不会修)。
-   确保恢复准确: 开启之后，如果有图标的位置错误, 会将其修正。当然, 也不能保证完全准确。
-   显示通知栏图标: 开启后会在通知栏显示一个图标。关闭此选项之后如果想关闭此程序则需要打开任务管理器。
-   调试模式: 显示一个黑窗口显示输出，一般来说保持为关闭就行。

## 使用方式

在配置完成之后，就可以开始使用了。
比如我在程序中添加了 "Desktop" 和 "Game" 两个桌面。

![程序界面](https://files.catbox.moe/aqtpvt.png)

现在按下 win+tab 快捷键打开虚拟桌面界面，新建几个虚拟桌面。因为我在程序中添加了 "Desktop" 和 "Game" 两个虚拟桌面，所以我新建了两个虚拟桌面并且重命名成"Desktop" 和 "Game"。
![虚拟桌面配置](https://files.catbox.moe/3csl0e.png)

然后就能切换虚拟桌面来体验了。

你当然可以添加更多。就像我这样：
![虚拟桌面界面](https://files.catbox.moe/6dvers.png)

当从一个 虚拟桌面 切换到另一个 虚拟桌面 后，程序会读取当前虚拟桌面的名称，如果虚拟桌面的名称已经在此程序里添加，就会将桌面的路径替换到另一个文件夹。并且会保存和恢复桌面图标的布局。

你可以在不同的桌面放置不同的文件或图标，将虚拟桌面划分成更加独立的不同的工作区。这是该程序的使命。
