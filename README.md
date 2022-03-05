# AutoVDesktop

Quickly change files and ICONS on the desktop after switching desktops using Win + TAB。

在使用 win+tab 或者 ctrl+win+←/→ 切换虚拟桌面之后，切换绑定的桌面图标。

[使用的库](https://github.com/Grabacr07/VirtualDesktop)  [核心代码来自](https://www.codeproject.com/Articles/639486/Save-and-Restore-Icon-Positions-on-Desktop?msg=5864404#xx5864404xx) 感谢

## 注意事项

- 该程序只会修改程序目录下的文件，不会删除和增加你其他地方的任何文件。
- 如果你不知道 JSON，我不建议你继续使用。
- 使用时尽量不要用桌面右键或者 F5 刷新。
- 该程序对桌面上图标和文件特别多的用户会有帮助。（比如你的桌面全被图标占满了！）
- 你必须知道你的 PC 当前桌面的路径，一般在"C:\用户\用户名\Desktop"，程序运行时创建的桌面文件夹会在默认桌面的父文件夹下。如果你在配置文件里添加了一个"Game"桌面，那么这个桌面的路径就会在"C:\用户\用户名\Game"。如果你不清楚，则不建议使用。

## 常见问题

- Q：怎么关闭这个软件？ A：打开任务管理器，找到"AutoVDesktop"右键点击结束。
- Q：怎么快速切换桌面？ A：按ctrl+win+左右方向键。
- Q：我不想用了，我想恢复回原来的桌面该怎么做？ A:  看这个链接，修改回你以前的桌面路径 [设置桌面路径](https://zhuanlan.zhihu.com/p/78243921)

## 开始配置

1)先运行一次，会生成配置文件。

2)打开程序目录的 config。json 并编辑，默认的配置文件长这样:

```json
{"Desktops": ["Desktop"],"Delay": 1000,"RestoreIcon": true,"ShowInTaskbar": true,"DebugMode": false}
```

格式化之后是这样的:

```json
{
  "Desktops": ["Desktop"],
  "Delay": 1000,
  "RestoreIcon": true,
  "ShowInTaskbar": true,
  "DebugMode": false
}
```

- Desktop: 这个数组记录了程序响应的桌面名称，默认只有一个"Desktop"，可以添加你想要的桌面，比如"Game"，最后会说明他是怎么工作的。

  ```json
  {
    "Desktops": ["Desktop", "Game"]
  }
  ```
- Delay: 在切换桌面后等待的时间，默认是 1000 毫秒，即在切换桌面后等 1 秒再切换桌面，建议值为 100。
- RestoreIcon: 切换桌面之后是否恢复图标的位置，默认开启，如果不开启的话，切换桌面之后桌面图标会重新排列。<!>注意，开启此功能后再更换桌面之前不要用 F5 刷新桌面，不然前三列的图标会有概率乱序。(标记一个 bug，但是不会修)。
- ShowInTaskbar: 设置为 true 会在任务栏显示一个图标(只是显示，目前没啥交互)，开启之后程序会多占 10mb 左右内存呢。
- DebugMode: 显示一个黑窗口显示输出，一般来说保持为 false 就行。

## 使用方式

在配置完 Desktop 之后，就可以开始使用了。我在上面配置的是这样的:

```json
{
  "Desktops": ["Desktop", "Game"]
}
```

现在按下 win+tab 快捷键打开虚拟桌面界面，新建几个虚拟桌面，默认的虚拟桌面名称可能是"桌面 1"，"桌面 2"之类的，点击桌面的名字重命名，我这里设置了"Desktop"和"Game"两个虚拟桌面，所以我新建了两个虚拟桌面并分别重命名成"Desktop"和"Game"。你当然可以添加更多。

当从一个桌面切换到另一个桌面后，程序会读取当前虚拟桌面的名称，如果虚拟桌面的名称包含在配置文件的 Desktop 中，就会将桌面的路径替换到另一个文件夹。并且会保存桌面图标的布局。

你可以在不同的桌面放置不同的文件或图标，将虚拟桌面划分成更加独立的不同的工作区。