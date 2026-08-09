using Godot;
using System.Collections.Generic;
using System;

public partial class SpawnerNotas : Node
{
   public enum RitmoPreset
   {
	  Manual = 0,
	  Basico4x4 = 1,
	  Alternado = 2,
	  Escalera = 3,
	  SincopaSuave = 4
   }

   [Export] public NodePath RutaControlRitmo { get; set; }
   [Export] public bool AutoIniciarMinijuego { get; set; } = true;
   [Export] public RitmoPreset PresetActual { get; set; } = RitmoPreset.Manual;
   [Export] public double TiempoInicioSegundos { get; set; } = 1.5;
   [Export] public double IntervaloBaseSegundos { get; set; } = 0.45;
   [Export] public int RepeticionesPreset { get; set; } = 2;
   [Export(PropertyHint.MultilineText)] public string PatronManualAcciones { get; set; } = "hit_a,hit_s,hit_d,hit_f";
   [Export] public string AccionCarril1 { get; set; } = "hit_a";
   [Export] public string AccionCarril2 { get; set; } = "hit_s";
   [Export] public string AccionCarril3 { get; set; } = "hit_d";
   [Export] public string AccionCarril4 { get; set; } = "hit_f";

   private ControlRitmo _controlRitmo;

   public override void _Ready()
   {
	  if (RutaControlRitmo == null || RutaControlRitmo.IsEmpty)
	  {
		 GD.PushError("SpawnerNotas requiere RutaControlRitmo asignada.");
		 return;
	  }

	  _controlRitmo = GetNode<ControlRitmo>(RutaControlRitmo);
	  NormalizarAccionesSegunInputMap();
	  List<Nota> notas = PresetActual == RitmoPreset.Manual ? GenerarNotasDesdeManual() : GenerarNotasDesdePreset();

	  if (notas.Count == 0)
	  {
		 GD.PushWarning("SpawnerNotas no encontro notas para cargar.");
	  }

	  _controlRitmo.CargarNotas(notas);

	  if (AutoIniciarMinijuego)
	  {
		 _controlRitmo.IniciarMinijuego();
	  }
   }

   private List<Nota> GenerarNotasDesdeManual()
   {
	  List<Nota> notas = new();
	  if (string.IsNullOrWhiteSpace(PatronManualAcciones))
	  {
		 return notas;
	  }

	  string[] tokens = PatronManualAcciones.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
	  double tiempo = TiempoInicioSegundos;
	  foreach (string token in tokens)
	  {
		 Nota nota = new();
		 nota.Configurar(token, tiempo);
		 notas.Add(nota);
		 tiempo += IntervaloBaseSegundos;
	  }

	  return notas;
   }

   private List<Nota> GenerarNotasDesdePreset()
   {
	  List<Nota> notas = new();
	  List<(int carril, double factorIntervalo)> pasos = ObtenerPasosDelPreset(PresetActual);
	  if (pasos.Count == 0)
	  {
		 return notas;
	  }

	  int repeticiones = RepeticionesPreset < 1 ? 1 : RepeticionesPreset;
	  double tiempo = TiempoInicioSegundos;

	  for (int r = 0; r < repeticiones; r++)
	  {
		 foreach ((int carril, double factorIntervalo) paso in pasos)
		 {
			string accion = ObtenerAccionPorCarril(paso.carril);
			Nota nota = new();
			nota.Configurar(accion, tiempo);
			notas.Add(nota);

			tiempo += IntervaloBaseSegundos * paso.factorIntervalo;
		 }
	  }

	  return notas;
   }

   private List<(int carril, double factorIntervalo)> ObtenerPasosDelPreset(RitmoPreset preset)
   {
	  return preset switch
	  {
		 RitmoPreset.Basico4x4 => new List<(int, double)>
		 {
			(1, 1.0), (2, 1.0), (3, 1.0), (4, 1.0),
			(1, 1.0), (2, 1.0), (3, 1.0), (4, 1.0)
		 },
		 RitmoPreset.Alternado => new List<(int, double)>
		 {
			(1, 1.0), (3, 1.0), (2, 1.0), (4, 1.0),
			(1, 1.0), (3, 1.0), (2, 1.0), (4, 1.0)
		 },
		 RitmoPreset.Escalera => new List<(int, double)>
		 {
			(1, 1.0), (2, 1.0), (3, 1.0), (4, 1.0),
			(4, 1.0), (3, 1.0), (2, 1.0), (1, 1.0)
		 },
		 RitmoPreset.SincopaSuave => new List<(int, double)>
		 {
			(1, 1.0), (2, 0.5), (3, 1.5), (4, 1.0),
			(2, 0.5), (1, 1.0), (4, 0.5), (3, 1.5)
		 },
		 _ => new List<(int, double)>()
	  };
   }

   private string ObtenerAccionPorCarril(int carril)
   {
	  return carril switch
	  {
		 1 => AccionCarril1,
		 2 => AccionCarril2,
		 3 => AccionCarril3,
		 4 => AccionCarril4,
		 _ => AccionCarril1
	  };
   }

   private void NormalizarAccionesSegunInputMap()
   {
	  AccionCarril1 = ResolverAccion("Carril1", AccionCarril1, "hit_a", "hit_1");
	  AccionCarril2 = ResolverAccion("Carril2", AccionCarril2, "hit_s", "hit_2");
	  AccionCarril3 = ResolverAccion("Carril3", AccionCarril3, "hit_d", "hit_3");
	  AccionCarril4 = ResolverAccion("Carril4", AccionCarril4, "hit_f", "hit_4");
   }

   private static string ResolverAccion(string etiqueta, string configurada, string preferida, string legacy)
   {
	  if (!string.IsNullOrWhiteSpace(configurada) && InputMap.HasAction(configurada))
	  {
		 return configurada;
	  }

	  if (InputMap.HasAction(preferida))
	  {
		 GD.PushWarning($"SpawnerNotas ajusto {etiqueta} a '{preferida}' porque '{configurada}' no existe en InputMap.");
		 return preferida;
	  }

	  if (InputMap.HasAction(legacy))
	  {
		 GD.PushWarning($"SpawnerNotas ajusto {etiqueta} a '{legacy}' porque '{configurada}' no existe en InputMap.");
		 return legacy;
	  }

	  return configurada;
   }
}
