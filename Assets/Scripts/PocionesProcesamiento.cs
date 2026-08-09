using Godot;

public partial class PocionesProcesamiento : Node2D
{
	[Signal]
	public delegate void MinijuegoPerdidoEventHandler();

	[Export] public NodePath RutaControlRitmo { get; set; } = "MiniJuegoProc/ControlRitmo";
	[Export] public string EscenaAlPerder { get; set; } = "res://Assets/Scenes/Pociones.tscn";

	private ControlRitmo _controlRitmo;

	public override void _Ready()
	{
		_controlRitmo = GetNodeOrNull<ControlRitmo>(RutaControlRitmo);
		if (_controlRitmo == null)
		{
			GD.PushError("PocionesProcesamiento no pudo resolver ControlRitmo.");
			return;
		}

		_controlRitmo.ProcesamientoFinalizado += OnProcesamientoFinalizado;
	}

	private async void OnProcesamientoFinalizado(bool exito)
	{
		if (exito)
		{
			return;
		}

		EmitSignal(SignalName.MinijuegoPerdido);

		if (string.IsNullOrEmpty(EscenaAlPerder))
		{
			return;
		}

		if (CanvasTransition.Instance == null || !IsInstanceValid(CanvasTransition.Instance))
		{
			GD.PrintErr("PocionesProcesamiento: CanvasTransition.Instance no es válido.");
			return;
		}

		await CanvasTransition.Instance.ChangeSceneAsync(EscenaAlPerder);
	}
}
