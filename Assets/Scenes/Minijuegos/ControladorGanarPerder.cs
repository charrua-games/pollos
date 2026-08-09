using Godot;
using System;

public partial class ControladorGanarPerder : Node2D
{
	[Export(PropertyHint.File, "*.tscn")]
	public string EscenaDestino;
	public string IdPocionAmarillo = "Amarillo";
	public string IdPocionRojo = "Rojo";
	public string IdPocionAzul = "Azul";


	private SpawnerNotas spawnerNotas;
	private DesbloqueoPociones desbloqueoPociones;

	public override void _Ready()
	{
		spawnerNotas = GetNode<SpawnerNotas>("SpawnerNotas");
		desbloqueoPociones = GetNodeOrNull<DesbloqueoPociones>("/root/DesbloqueoPociones");
	}

	private void _on_spawner_notas_notas_finalizadas(bool exito)
	{
		if (!exito)
		{
			return;
		}
		desbloqueoPociones.Desbloquear(IdPocionAmarillo);
		desbloqueoPociones.Desbloquear(IdPocionAzul);
		desbloqueoPociones.Desbloquear(IdPocionRojo);
		CanvasTransition.Instance.ChangeSceneAsync(EscenaDestino);
	}
}
