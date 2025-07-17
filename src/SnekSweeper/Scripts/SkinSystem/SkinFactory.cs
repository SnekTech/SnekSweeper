namespace SnekSweeper.SkinSystem;

public static class SkinFactory
{
    private static readonly SkinData Default = new(nameof(Default), "res://Art/SnekSweeperSpriteSheet.png");
    private static readonly SkinData Mahjong = new(nameof(Mahjong), "res://Art/SnekSweeperSpriteSheet02.png");
    
    public static List<ISkin> Skins => [Default, Mahjong];

    public static ISkin GetSkinByName(string name) => Skins.First(skin => skin.Name == name);
}