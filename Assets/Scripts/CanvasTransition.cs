using Godot;
using System;
using System.Threading.Tasks;

public partial class CanvasTransition : CanvasLayer
{
	public static CanvasTransition Instance { get; private set; }

	[Export]
	private ColorRect colorRect;

	public override void _Ready()
	{
		Instance = this;
	}

	public async Task ChangeSceneAsync(string path)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(colorRect, "color:a", 1.0f, 0.3f);
		await ToSignal(tween, Tween.SignalName.Finished);

		GetTree().ChangeSceneToFile(path);

		tween = CreateTween();
		tween.TweenProperty(colorRect, "color:a", 0.0f, 1.0f);
		await ToSignal(tween, Tween.SignalName.Finished);
	}
}
