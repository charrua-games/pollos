using Godot;
using System;

public partial class Mezclador : Node2D
{
	private float hueActual;
	private float saturacionActual;
	private float valorActual;

	private float hueObjetivo;
	private float saturacionObjetivo;
	private float valorObjetivo;

	private bool activo = false;

	[Export]
	public Sprite2D sprite;

	[Export(PropertyHint.Range, "0,1")]
	public float step { get; set; } = 0.3f; // velocidad de mezcla (por segundo)

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

		hueActual = sample.H;
		saturacionActual = sample.S;
		valorActual = sample.V;

		hueObjetivo = hueActual;
		saturacionObjetivo = saturacionActual;
		valorObjetivo = valorActual;

		ActualizarSprite();
	}

	public override void _Process(double delta)
	{
		if (!activo)
		{
			return;
		}

		float velocidad = step * (float)delta;

		// --- Hue: interpolación circular (0 a 1 da la vuelta) ---
		float diferenciaHue = DiferenciaHue(hueActual, hueObjetivo);

		if (Mathf.Abs(diferenciaHue) <= velocidad)
		{
			hueActual = hueObjetivo;
		}
		else
		{
			hueActual += Mathf.Sign(diferenciaHue) * velocidad;
			hueActual = Mathf.PosMod(hueActual, 1f);
		}

		// --- Saturación y Valor: interpolación lineal normal ---
		saturacionActual = Mathf.MoveToward(saturacionActual, saturacionObjetivo, velocidad);
		valorActual = Mathf.MoveToward(valorActual, valorObjetivo, velocidad);

		ActualizarSprite();

		bool llego =
			Mathf.Abs(diferenciaHue) < 0.001f &&
			Mathf.Abs(saturacionActual - saturacionObjetivo) < 0.001f &&
			Mathf.Abs(valorActual - valorObjetivo) < 0.001f;

		if (llego)
		{
			GD.Print("Mezcla terminada. Hue: ", hueActual);
		}
	}

	private void ActualizarSprite()
	{
		Color colorActual = Color.FromHsv(hueActual, saturacionActual, valorActual);
		sprite.Modulate = colorActual;
	}

	// Diferencia angular entre dos hues, en el rango (-0.5, 0.5]
	private float DiferenciaHue(float actual, float objetivo)
	{
		float diferencia = objetivo - actual;

		if (diferencia > 0.5f)
		{
			diferencia -= 1f;
		}
		else if (diferencia < -0.5f)
		{
			diferencia += 1f;
		}

		return diferencia;
	}

	// Se llama mientras se mantiene click + Space (ver Pocion.cs)
	public void CambiarObjetivo(Color nuevoColor)
	{
		hueObjetivo = nuevoColor.H;
		saturacionObjetivo = nuevoColor.S;
		valorObjetivo = nuevoColor.V;
		activo = true;
	}

	// Se llama al soltar el click o Space
	public void DetenerMezcla()
	{
		activo = false;
		GD.Print("Mezcla detenida en Hue: ", hueActual);
	}
}
