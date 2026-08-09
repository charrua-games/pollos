
using Godot;
using System;

public partial class PocionesControl : Node2D
{
	[Export]
	private PocionReferencia pocionReferencia;

	[Export]
	public Pocion[] plantas;

	[Export]
	public TextureButton evaluarButton { get; set; }

	[Export]
	private Contenedor contenedor { get; set; }

	[Export]
	private CalcularPorcentajeAciertoColor calcularPuntaje { get; set; }

	[Export]
	private Label ganasteLabel { get; set; }
	[Export]
	private Label PerdisteLabel { get; set; }

	private Color colorReferencia;

	private Random _random = new Random();

	public override void _Ready()
	{
		ganasteLabel.Visible = false;
		PerdisteLabel.Visible = false;

		PrepararPlantas();
		CrearColorReferencia();

		GD.Print("Color de referencia: ", colorReferencia);

		evaluarButton.ButtonDown += OnBotonApretado;
	}

	private void PrepararPlantas()
	{
		plantas[0].CambiarColor(Colors.Red);
		plantas[1].CambiarColor(Colors.Blue);
		plantas[2].CambiarColor(Colors.Yellow);
	}

	private void CrearColorReferencia()
	{
		float tono = ColorRandomRaw();

		colorReferencia = Color.FromHsv(
			tono,
			1.0f,
			1.0f
		);

		pocionReferencia.CambiarColor(colorReferencia);
	}

	public Color ObtenerColorReferencia()
	{
		return colorReferencia;
	}

	private float ColorRandomRaw()
	{
		return _random.NextSingle();
	}

	private void OnBotonApretado()
	{
		Color colorReferenciaActual = pocionReferencia.ObtenerColor();
		Color colorContenedor = contenedor.ObtenerColor();

		if (calcularPuntaje == null)
		{
			GD.PrintErr("CalcularPuntaje no está asignado.");
			return;
		}

		double porcentajeAcierto =
			calcularPuntaje.EvaluarColor(
				colorContenedor,
				colorReferenciaActual
			);

		GD.Print("Porcentaje de acierto: ", porcentajeAcierto, "%");

		if (porcentajeAcierto < 95)
		{
			contenedor.RecibirColor(Colors.Black);
			PerdisteLabel.Visible = true;

			ganasteLabel.Visible = false;
		}
		else
		{
			
			ganasteLabel.Visible = true;
			PerdisteLabel.Visible = false;

		}
	}
}
