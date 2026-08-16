namespace SnekSweeperCore.SaveLoad.CustomJsonConverter;

public sealed class Mat2DConverter : Matrix2DConverter<bool>
{
    protected override int ElementToInt(bool value) => value ? 1 : 0;
    protected override bool IntToElement(int value) => value != 0;
}
