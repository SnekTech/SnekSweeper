module ComboTests

open Expecto
open CoreFS.ComboDomain

/// 测试工具：在某时刻连续 increment n 次得到的状态（峰值 Level = min(n, MaxLevel)）
let incrementedNTimes (times: int) (atTime: float) : ComboState =
    List.fold (fun s _ -> ComboDefault.increment atTime s) Combo.initial [ 1..times ]

/// 测试工具：直接构造任意状态
let stateAt (now: float) (level: int) : ComboState =
    { Level = level; LastIncrementAt = now }

[<Tests>]
let comboCoreBehavior =
    testList "combo" [
        // ---- 核心行为 ----
        testCase "increment 提升等级" <| fun () ->
            let s = incrementedNTimes 2 0.0
            Expect.equal (ComboDefault.levelAt 0.0 s) 2 "加两次应到 lv2"

        testCase "increment 在 maxLevel 封顶" <| fun () ->
            let s = incrementedNTimes 10 0.0
            Expect.equal (ComboDefault.levelAt 0.0 s) 4 "10 次应在 maxLevel=4 封顶"

        testCase "increment 重置进度条比例" <| fun () ->
            let s1 = ComboDefault.increment 0.0 Combo.initial
            let s2 = ComboDefault.increment 3.0 s1
            Expect.floatClose Accuracy.medium (ComboDefault.progressRatio 3.0 s2) 1.0 "再次加分后比例应为 1"

        testCase "reset 等效于回到 initial" <| fun () ->
            Expect.equal Combo.initial.Level 0 "initial 等级为 0"
            Expect.equal Combo.initial.LastIncrementAt 0.0 "initial 时间为 0"

        testCase "空状态（initial）的进度比例恒为 0" <| fun () ->
            Expect.equal (ComboDefault.progressRatio 100.0 Combo.initial) 0.0 "空状态比例 0"

        // ---- 时间流逝 / 衰减 ----
        testTheory "按时间衰减到预期等级" [
            (4, 12.0, 2)   // lv4, 12s -> lv2
            (4, 5.0, 3)    // lv4, 正好一个间隔 -> lv3
            (4, 4.9, 4)    // lv4, 未满间隔 -> lv4
            (3, 5.0, 2)    // lv3, 一个间隔 -> lv2
        ] <| fun ((level, now, expected): int * float * int) ->
            Expect.equal (ComboDefault.levelAt now (stateAt 0.0 level)) expected "衰减等级不符"

        testCase "衰减到 0 为止" <| fun () ->
            let s = incrementedNTimes 4 0.0
            Expect.equal (ComboDefault.levelAt 100.0 s) 0 "长时间后归 0"

        testCase "等级为 0 时进度归零" <| fun () ->
            let s = incrementedNTimes 4 0.0
            Expect.equal (ComboDefault.progressRatio 100.0 s) 0.0 "归 0 后比例 0"

        testCase "increment 阻止衰减" <| fun () ->
            let s1 = incrementedNTimes 2 0.0            // lv2
            let s2 = ComboDefault.increment 4.5 s1      // 4.5s 加分 -> lv3 并重置时间
            Expect.equal (ComboDefault.levelAt 9.0 s2) 3 "加分后 4.5s 仍未满间隔"

        // ---- progressRatio ----
        testCase "加分后进度从 1 开始" <| fun () ->
            let s = ComboDefault.increment 0.0 Combo.initial
            Expect.floatClose Accuracy.medium (ComboDefault.progressRatio 0.0 s) 1.0 "刚加分比例 1"

        testCase "进度随时间线性下降" <| fun () ->
            let s = ComboDefault.increment 0.0 Combo.initial
            Expect.floatClose Accuracy.medium (ComboDefault.progressRatio 2.5 s) 0.5 "2.5s 比例 0.5"

        testCase "进度趋近 0" <| fun () ->
            let s = ComboDefault.increment 0.0 Combo.initial
            Expect.isLessThan (ComboDefault.progressRatio 4.99 s) 0.01 "4.99s 比例应小于 0.01"

        testCase "负时间差进度不超 1" <| fun () ->
            let s = ComboDefault.increment 1.0 Combo.initial   // 最后加分在 t=1
            Expect.floatClose Accuracy.medium (ComboDefault.progressRatio 0.0 s) 1.0 "负 delta / 时钟回拨时钳制为 1"

        // ---- config 校验 ----
        testCase "config 拒绝非正衰减间隔" <| fun () ->
            Expect.isError (ComboConfig.Create(4, 0.0)) "decayInterval <= 0 应拒绝"

        testCase "config 拒绝 maxLevel < 2" <| fun () ->
            Expect.isError (ComboConfig.Create(1, 5.0)) "maxLevel < 2 应拒绝"

        // ---- 增补：衰减后 increment（旧 C# 测试未覆盖的回归） ----
        testCase "衰减后 increment 从当前等级 +1，而非峰值" <| fun () ->
            let s = incrementedNTimes 2 0.0
            Expect.equal (ComboDefault.levelAt 6.0 s) 1 "6s 后衰减到 lv1"
            let s2 = ComboDefault.increment 6.0 s
            Expect.equal (ComboDefault.levelAt 6.0 s2) 2 "衰减后加分回到 lv2，而不是跳到高位"

        testCase "等很久再 increment 不会冲到 maxLevel" <| fun () ->
            let s = incrementedNTimes 4 0.0
            Expect.equal (ComboDefault.levelAt 100.0 s) 0 "早已衰减到 0"
            let s2 = ComboDefault.increment 100.0 s
            Expect.equal (ComboDefault.levelAt 100.0 s2) 1 "从 0 加分到 1，而不是冲 max"

        // ---- 边界不变量（确定性覆盖，替代 FsCheck 属性测试） ----
        testCase "进度比例恒在 [0,1]：负时间 / 长时间边界" <| fun () ->
            let a = ComboDefault.increment 1.0 Combo.initial
            let r1 = ComboDefault.progressRatio 0.0 a
            let b = incrementedNTimes 4 0.0
            let r2 = ComboDefault.progressRatio 100.0 b
            Expect.isTrue (r1 >= 0.0 && r1 <= 1.0) "负时间差比例越界"
            Expect.isTrue (r2 >= 0.0 && r2 <= 1.0) "长时间比例越界"
    ]
