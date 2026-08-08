using Godot;
using System;

public partial class Mezclador : Node2D
{
	private float posicionActual;
	private Color colorInicial;
	[Export]
	private Color colorFinal = Color.Color8(0, 0, 125);
	[Export(PropertyHint.Range, "0,1")]
	public float step { get; set; } = 0.05f;
	[Export]
	public Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("TestCube");
		Image img = sprite.Texture.GetImage();
		if (img.IsCompressed())
		{
			img.Decompress();
		}

		Vector2I size = img.GetSize();
		Color sample = img.GetPixel(size.X / 2, size.Y / 2);
		colorInicial = Color.FromHsv(sample.H, 1f, 1f);
		sprite.Modulate = colorInicial;

		posicionActual = colorInicial.H;
	}

	public override void _Process(double delta)
	{

		if (!Input.IsMouseButtonPressed(MouseButton.Left))
		{
			return;
		}

		var direccion = Mezclar(colorInicial, colorFinal, delta);
		GD.Print("[?] posicionActual vieja ", posicionActual);
		posicionActual += step * direccion * (float)delta;

		posicionActual = Mathf.PosMod(posicionActual, 1);
		GD.Print("[?] posicionActual nueva ", posicionActual);

		colorInicial.H = posicionActual;
		sprite.Modulate = colorInicial;
	}


	public float Mezclar(Color color1, Color color2, double delta)
	{
		double color1Rad = color1.H * 2 * Math.PI;
		GD.Print("[?] Color1Rad = ", color1Rad);
		double color2Rad = color2.H * 2 * Math.PI;
		GD.Print("[?] Color2Rad = ", color2Rad);
		double diferencia = color1Rad - color2Rad;
		double direccion = -Math.Atan2(Math.Sin(diferencia), Math.Cos(diferencia)); // Nos dice en que sentido deberiamos movernos, sentido horario si es positivo y anti-horario si es negativo
		GD.Print("[?] Direccion = ", direccion);
		if (direccion >= 0) { return 1; }
		return -1;
	}
}
