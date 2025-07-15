# Snek Sweeper

A minesweeper game with modern game features & nice pixel art assets.


## GamePlay

- [x] different difficulties
    - [x] use difficulty to init grid
    - [x] add difficulty setting UI
- [x] init grid after first click
- [x] game loop
    - [x] scene change on game win or over
- [ ] cell reveal combo
    - [x] standalone combo test scene
    - [x] combo component
- [ ] charging reveal
- [x] undo ability
- [x] PRNG using PCG-random
    - [x] save random seed for each game history record
- [ ] collectable cheat-code like Doom Eternal
- [ ] *moving bombs every click, show the moving direction
- [ ] 闪卡
- [ ] 夜光，黑暗环境下某些元素发光提示
- [ ] 拍卖机制
- [ ] bingo game

## User Input

- [ ] refactor InputListener autoload to an event bus

## Animation

- [x] cell sprite animation
    - [x] cover animation
    - [x] flag animation
- [x] fade out animation on scene switch

## Visual Effects

- [ ] 2D lighting
- [ ] scrolling background
    - [x] using shader
    - [ ] random generated like Balatro

## Save & Load

- [ ] what to save
    - [x] current difficulty
    - [x] current skin
- [x] architecture using Godot Resource
- [ ] Steam cloud save

## UI

- [ ] HUD
    - [ ] half screen damage / healing effect
- [ ] circle process challenge like Apple Watch
- [x] highlight the hovering cell
    - [ ] glow effect
- [x] grid status display
    - [x] bombs remaining
- [ ] pixel art theming
    - [x] default pixel font
    - [x] button
- [x] message queue

## Skin

- [ ] skin assets
    - [x] default
    - [x] 麻将
- [ ] UI
    - [x] change skin
- [ ] change skin on all cell components
    - [x] content
    - [ ] cover
    - [ ] flag

## Game Modes

- [x] original
- [ ] zen mode, immersive minesweeper experience
- [ ] timing
- [ ] endless
- [ ] no guess

## History Records

- [x] history screen
- [x] save/load

## Health / Damage System

## Tutorial

- [ ] interactable grid & cell
- [ ] floating indicator

## Achievement

- [ ] Steam integration

## Digital Museum

- show off game assets

## Cheat Code

- [ ] 半透明单元格几秒
- [ ] 15% 概率不受伤

### 新游戏
- 自动售货机