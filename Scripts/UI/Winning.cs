using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI;

public partial class Winning : Control
{
	private Button _back2MainBtn = null!;
	
	public override void _Ready()
	{
		_back2MainBtn = GetNode<Button>("%Back2MainBtn");

		_back2MainBtn.Pressed += () => SceneManager.Instance.GotoScene(ScenePaths.MainScene);
	}
}