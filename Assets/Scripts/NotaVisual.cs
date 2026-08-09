using Godot;

public partial class NotaVisual : Node2D
{
	[Export] public NodePath RutaSprite { get; set; } = "Sprite2D";

	[Export] public Texture2D TexturaCarril1 { get; set; }
	[Export] public Texture2D TexturaCarril2 { get; set; }
	[Export] public Texture2D TexturaCarril3 { get; set; }
	[Export] public Texture2D TexturaCarril4 { get; set; }

	[Export] public int HFrames { get; set; } = 6;
	[Export] public int VFrames { get; set; } = 6;
	[Export] public int FrameCount { get; set; } = 31;
	[Export] public double FramesPorSegundo { get; set; } = 20.0;
	[Export] public float Escala { get; set; } = 0.04f;
	[Export] public Vector2 FactorEscalaResaltada { get; set; } = new Vector2(1.2f, 1.2f);

	private Sprite2D _sprite;
	private double _tiempoAcumulado;
	private int _frameActual;
	private Vector2 _escalaBase;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>(RutaSprite);
		_sprite.Hframes = HFrames;
		_sprite.Vframes = VFrames;
		_sprite.Frame = 0;
		_escalaBase = new Vector2(Escala, Escala);
		_sprite.Scale = _escalaBase;
	}

	public override void _Process(double delta)
	{
		if (FrameCount <= 0 || FramesPorSegundo <= 0.0)
		{
			return;
		}

		double duracionFrame = 1.0 / FramesPorSegundo;
		_tiempoAcumulado += delta;

		while (_tiempoAcumulado >= duracionFrame)
		{
			_tiempoAcumulado -= duracionFrame;
			_frameActual = (_frameActual + 1) % FrameCount;
			_sprite.Frame = _frameActual;
		}
	}

	public void Configurar(int indiceCarril)
	{
		_sprite ??= GetNode<Sprite2D>(RutaSprite);
		_sprite.Texture = indiceCarril switch
		{
			0 => TexturaCarril1,
			1 => TexturaCarril2,
			2 => TexturaCarril3,
			3 => TexturaCarril4,
			_ => TexturaCarril1,
		};
	}

	public void EstablecerResaltada(bool resaltada)
	{
		_sprite ??= GetNode<Sprite2D>(RutaSprite);
		_sprite.Scale = resaltada ? _escalaBase * FactorEscalaResaltada : _escalaBase;
	}
}
