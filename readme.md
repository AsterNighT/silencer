# Silencer

> Silence!

## What is this?

Silencer can mute unfocused window for you. It detects foreground window changing and mute/unmute them automatically. It is intended for muting certain applications that are not smart enough to mute themselves in background. (Stellaris, for example.)

Silencer now works in two modes:

1. Blacklist: Only listed applications will be muted.
2. Whitelist: Listed applications will not be muted.

## Know issues

1. Silencer would override your manual settings in the volume mixer.
2. The application are identified by `ProcessName`. In some cases there could be conflicts.
3. It only work on windows, and I only tested it on a few versions of windows 11.
-----
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




















