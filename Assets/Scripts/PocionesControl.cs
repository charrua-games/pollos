
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

	[Export]
	private DesbloqueoPociones desbloqueoPociones { get; set; }

	private Color colorReferencia;

	private Random _random = new Random();

	// Indica si ya se puede comenzar el desafío
	private bool puedeEvaluar = false;

	public override void _Ready()
	{
		ganasteLabel.Visible = false;
		PerdisteLabel.Visible = false;

		PrepararPlantas();

		// Primero comprobamos si tiene las 3 pociones
		ComprobarPociones();

		evaluarButton.ButtonDown += OnBotonApretado;
	}

	private void PrepararPlantas()
	{
		plantas[0].CambiarColor(Colors.Red);
		plantas[1].CambiarColor(Colors.Blue);
		plantas[2].CambiarColor(Colors.Yellow);
	}

	private void ComprobarPociones()
	{
		if (desbloqueoPociones == null)
		{
			GD.PrintErr("DesbloqueoPociones no está asignado.");
			return;
		}

		bool tieneRojo = desbloqueoPociones.EstaDesbloqueada("rojo");
		bool tieneAmarillo = desbloqueoPociones.EstaDesbloqueada("amarillo");
		bool tieneAzul = desbloqueoPociones.EstaDesbloqueada("azul");

		if (tieneRojo && tieneAmarillo && tieneAzul)
		{
			GD.Print("Tiene las tres pociones. Comenzando desafío.");

			puedeEvaluar = true;

			// AHORA se genera el color de referencia
			CrearColorReferencia();

			GD.Print("Color de referencia: ", colorReferencia);
		}
		else
		{
			GD.Print("Todavía no tiene las tres pociones.");

			puedeEvaluar = false;
		}
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
		// No hacer nada si todavía no tiene las tres pociones
		if (!puedeEvaluar)
		{
			GD.Print("Todavía no puedes evaluar.");
			return;
		}

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
