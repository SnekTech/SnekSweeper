namespace CoreFS.ComboDomain

type ComboTier =
    | None
    | Good
    | Great
    | Excellent

type ComboConfig =
    private
        { MaxLevel: int
          DecayInterval: float }

    static member Create(maxLevel: int, decayInterval: float) =
        if maxLevel < 2 then
            Error "maxLevel must be at least 2 to reach the Good/Great/Excellent tiers"
        elif decayInterval <= 0.0 then
            Error "decayInterval must be positive"
        else
            Ok
                { MaxLevel = maxLevel
                  DecayInterval = decayInterval }

    static member Default = { MaxLevel = 4; DecayInterval = 5.0 }

type ComboState = { Level: int; LastIncrementAt: float }

[<RequireQualifiedAccess>]
module Combo =
    let initial = { Level = 0; LastIncrementAt = 0.0 }

    /// 距最后一次加分经过的时间（负时间差按 0 处理：保护负 delta / 时钟回拨）
    let elapsedAt now state =
        max 0.0 (now - state.LastIncrementAt)

    let decayedLevels config now state =
        int (elapsedAt now state / config.DecayInterval)

    let levelAt config now state =
        max 0 (state.Level - decayedLevels config now state)

    let increment config now state =
        let currentLevel = levelAt config now state

        { Level = min (currentLevel + 1) config.MaxLevel
          LastIncrementAt = now }

    let progressRatio config now state =
        let level = levelAt config now state

        if level = 0 then
            0.0
        else
            let frac =
                elapsedAt now state / config.DecayInterval
                - float (decayedLevels config now state)

            1.0 - frac

    let getTier =
        function
        | 0
        | 1 -> None
        | 2 -> Good
        | 3 -> Great
        | 4 -> Excellent
        | _ -> Excellent

[<RequireQualifiedAccess>]
module ComboDefault =
    let private config = ComboConfig.Default

    let initial = Combo.initial
    let increment now state = Combo.increment config now state
    let levelAt now state = Combo.levelAt config now state
    let progressRatio now state = Combo.progressRatio config now state
    let getTier level = Combo.getTier level
