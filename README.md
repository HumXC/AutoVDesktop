# AutoVDesktop

Quickly change files and ICONS on the desktop after switching desktops using Win + TAB。

在使用 win+tab 或者 ctrl+win+←/→ 切换虚拟桌面之后，切换绑定的桌面图标。

使用的库:[VirtualDesktop](https://github.com/Grabacr07/VirtualDesktop)

## 声明

程序中实现保存和恢复桌面图标功能的代码(IconsRestorer)来自[此链接](https://www.codeproject.com/Articles/639486/Save-and-Restore-Icon-Positions-on-Desktop?msg=5864404#xx5864404xx)，我修改并使用了此段代码，IconsRestorer 代码的原作者不需要对此程序产生的任何后果负责。

## 注意事项

-   程序依赖 .NET 6.0 环境，如果没有则程序会弹出如下提示，点击确定后[跳转到](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime?cid=getdotnetcore)浏览器下载 Run desktop apps 的运行时(Runtime)：
    ![.NET6下载页](https://files.catbox.moe/8y0pcr.png)
-   该程序只会修改程序目录下的文件，不会删除和增加你其他地方的任何文件。
-   如果你不知道 JSON，我不建议你继续使用。
-   使用时尽量不要用桌面右键或者 F5 刷新。
-   该程序对桌面上图标和文件特别多的用户会有帮助。（比如你的桌面全被图标占满了！）
-   你必须知道你的 PC 当前桌面的路径，一般在"C:\用户\用户名\Desktop"，程序运行时创建的桌面文件夹会在默认桌面的父文件夹下。如果你在配置文件里添加了一个"Game"桌面，那么这个桌面的路径就会在"C:\用户\用户名\Game"。如果你不清楚，则不建议使用。
-   使用之前建议先修改桌面位置到一个单独的目录下，具体方法：[设置桌面路径](https://zhuanlan.zhihu.com/p/78243921)。比如我的将桌面路径设置为“D:\\\\Desktops\Code"那么在切换到 Game 桌面后生成的 Game 桌面文件夹就会在 Code 文件夹的同级目录下:
    ![设置桌面位置](https://files.catbox.moe/udwv1h.png)

## 常见问题

-   Q：怎么快速切换桌面？ A：按 ctrl+win+左右方向键。
-   Q：我不想用了，我想恢复回原来的桌面该怎么做？ A: 看这个链接，修改回你以前的桌面路径 [设置桌面路径](https://zhuanlan.zhihu.com/p/78243921)

## 配置文件说明

从 1.0.0-beta 版本开始加入了配置窗口，右键通知栏的程序图标可以打开“选项”来进行配置，以下是对配置文件编辑的帮助。
程序目录下的 config.json 是程序的配置文件，默认的配置文件长这样(此文档中的 json 内容已被格式化):

```json
{
    "Desktops": ["Desktop"],
    "Delay": 1000,
    "RestoreIcon": true,
    "ShowNotifyIcon": true,
    "DebugMode": false
}
```

运行程序后会在程序目录下生成 Desktops 文件夹，是桌面图标的位置信息。

-   桌面(Desktop): 这个数组记录了程序响应的桌面名称，默认只有一个"Desktop"，可以添加你想要的桌面，比如"Game"，最后会说明他是怎么工作的。

    ```json
    {
        "Desktops": ["Desktop", "Game"]
    }
    ```

-   响应延迟(Delay): 在切换桌面后等待的时间，默认是 1000 毫秒，即在切换桌面后等 1 秒再切换桌面，建议值为 500。
-   恢复图标(RestoreIcon): 切换桌面之后是否恢复图标的位置，默认开启，如果不开启的话，切换桌面之后桌面图标会重新排列。关闭此选项会让程序响应更快。<!>注意，开启此功能后更换桌面之前不要用 F5 刷新桌面，不然前三列的图标会有概率乱序。(标记一个 bug，但是不会修)。
-   显示通知栏图标(ShowNotifyIcon): 设置为 true 会在通知栏显示一个图标。如果设置为 false 将不会显示。关闭此选项之后如果想关闭此程序则需要打开任务管理器。
-   调试模式(DebugMode): 显示一个黑窗口显示输出，一般来说保持为 false 就行。

## 使用方式

在配置完成之后，就可以开始使用了。我在上面配置的是这样的:

```json
{
    "Desktops": ["Desktop", "Game"]
}
```

现在按下 win+tab 快捷键打开虚拟桌面界面，新建几个虚拟桌面，我这里设置了"Desktop"和"Game"两个虚拟桌面，所以我新建了两个虚拟桌面会自动重命名成"Desktop"和"Game"。你当然可以添加更多。就像我这样：
![虚拟桌面界面](https://files.catbox.moe/6dvers.png)

当从一个桌面切换到另一个桌面后，程序会读取当前虚拟桌面的名称，如果虚拟桌面的名称已经在此程序里添加，就会将桌面的路径替换到另一个文件夹。并且会保存和恢复桌面图标的布局。

你可以在不同的桌面放置不同的文件或图标，将虚拟桌面划分成更加独立的不同的工作区。
