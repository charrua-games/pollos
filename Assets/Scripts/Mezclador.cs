using Godot;
using System;

public partial class Mezclador : Node2D
{
	private Color colorActual;
	private Color colorObjetivo;
	private bool activo = false;

	[Export]
	public Sprite2D sprite;

	// Qué tan rápido se acerca el color actual al objetivo mientras se mantiene el click.
	[Export(PropertyHint.Range, "0,1")]
	public float step { get; set; } = 0.5f; // velocidad de mezcla (por segundo)

	public override void _Ready()
	{
		if (sprite == null)
		{
			GD.PrintErr("Mezclador: Sprite no asignado.");
			return;
		}

		if (sprite.Texture == null)
		{
			GD.PrintErr("Mezclador: El Sprite no tiene textura.");
			return;
		}

		Image img = sprite.Texture.GetImage();

		if (img.IsCompressed())
		{
			img.Decompress();
		}

		Vector2I size = img.GetSize();

		Color sample = img.GetPixel(
			size.X / 2,
			size.Y / 2
		);

		colorActual = sample;
		colorObjetivo = sample;

		sprite.Modulate = colorActual;
	}

	public override void _Process(double delta)
	{
		if (!activo)
		{
			return;
		}

		// Se acerca gradualmente al color de la poción que se está manteniendo presionada.
		// Como se detiene al soltar (no llega necesariamente al 100%), el resultado
		// queda como una mezcla parcial, y la próxima poción se combina sobre eso.
		colorActual = colorActual.Lerp(colorObjetivo, step * (float)delta);
		sprite.Modulate = colorActual;
	}

	// Se llama en ButtonDown: fija hacia qué color mezclar y arranca el avance.
	public void CambiarObjetivo(Color nuevoColor)
	{
		colorObjetivo = nuevoColor;
		activo = true;

		GD.Print("Mezclando hacia: ", colorObjetivo);
	}

	// Se llama en ButtonUp: congela el color donde haya quedado.
	public void DetenerMezcla()
	{
		activo = false;

		GD.Print("Mezcla detenida en: ", colorActual);
	}
}
