# Silencer

> 都给我闭嘴！

## 这是什么？

silencer 是一个用于静音非焦点窗口的工具。它可以自动检测焦点窗口切换并按一定规则静音与恢复不同窗口的声音。可以被用于静音一些不懂得在后台静音自己的游戏（例如 Stellaris）。

目前可以在两种规则下运行：

1. 黑名单：只有名单中的程序在后台时会被静音。

2. 白名单：名单中的程序在后台时不会被静音。

## 已知问题

1. 启用 Silencer 后就不能手动调整音量合成器里的静音了。

2. 判断进程仅使用了 ProcessName，在某些特殊情况下可能会撞车。

3. 显然完全不能在 Linux 上跑。




















