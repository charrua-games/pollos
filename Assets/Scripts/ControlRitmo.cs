using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ControlRitmo : Node
{
   [Signal]
   public delegate void ProcesamientoFinalizadoEventHandler(bool exito);

   [Export] public NodePath RutaRelojRitmo { get; set; }
   [Export] public double VentanaHitSegundos { get; set; } = 0.10;
   [Export] public bool UsarVentanaAsimetrica { get; set; } = true;
   [Export] public double VentanaAntesHitSegundos { get; set; } = 0.12;
   [Export] public double VentanaDespuesHitSegundos { get; set; } = 0.28;
   [Export] public double MaxFallosPermitidos { get; set; } = 3.0;
   [Export] public double PenalizacionTeclaErronea { get; set; } = 0.5;
   [Export] public bool UsarPollingInput { get; set; } = true;
   [Export] public bool UsarFallbackTecladoFisico { get; set; } = true;
   [Export] public bool UsarFallbackTecladoFisicoPorFrame { get; set; } = false;
   [Export] public string AccionTeclaA { get; set; } = "hit_a";
   [Export] public string AccionTeclaS { get; set; } = "hit_s";
   [Export] public string AccionTeclaD { get; set; } = "hit_d";
   [Export] public string AccionTeclaF { get; set; } = "hit_f";

   private readonly List<Nota> _notas = new();
   private readonly HashSet<string> _accionesHit = new();
   private readonly HashSet<string> _accionesFaltantesReportadas = new();
   private int _indiceSiguiente = 0;
   private RelojRitmo _reloj;
   private bool _activo;
   private bool _teclaAPresionada;
   private bool _teclaSPresionada;
   private bool _teclaDPresionada;
   private bool _teclaFPresionada;

   public bool ResultadoExitoso { get; private set; }
   public bool MinijuegoFinalizado { get; private set; }
   public double FallosActuales { get; private set; }

   public IReadOnlyList<Nota> Notas => _notas;
   public int IndiceSiguiente => _indiceSiguiente;
   public bool Activo => _activo;

   public override void _Ready()
   {
      SetProcess(true);
      SetPhysicsProcess(true);
      SetProcessInput(true);
      SetProcessUnhandledInput(true);

      if (RutaRelojRitmo == null || RutaRelojRitmo.IsEmpty)
      {
         GD.PushError("ControlRitmo requiere RutaRelojRitmo asignada.");
         return;
      }

      _reloj = GetNode<RelojRitmo>(RutaRelojRitmo);
   }

   public override void _PhysicsProcess(double delta)
   {
      if (!_activo)
      {
         return;
      }

      if (UsarFallbackTecladoFisicoPorFrame)
      {
         ProcesarInputFisicoPorFrame();
      }

      // Evita doble procesamiento del mismo input cuando hay fallback por tecla fisica.
      if (UsarPollingInput && !UsarFallbackTecladoFisico)
      {
         ProcesarInputPorPolling();
      }

      VerificarFalloPorTiempo();
   }

   public override void _UnhandledInput(InputEvent @event)
   {
      if (UsarPollingInput)
      {
         return;
      }

      if (!_activo || _indiceSiguiente >= _notas.Count)
      {
         return;
      }

      Nota nota = _notas[_indiceSiguiente];
      if (EstaAccionPresionada(@event, nota.AccionHit))
      {
         ProcesarInput(nota.AccionHit);
         return;
      }

      if (EsAccionDeHitPresionada(@event))
      {
         SumarFallo(PenalizacionTeclaErronea);
      }
   }

   public override void _Input(InputEvent @event)
   {
      // Si hay fallback fisico por frame, _Input no debe procesar para evitar duplicados.
      // Si Polling=true pero tambien UsarFallbackTecladoFisico=true, SI debemos procesar en _Input.
      if (UsarFallbackTecladoFisicoPorFrame || (UsarPollingInput && !UsarFallbackTecladoFisico))
      {
         return;
      }

      if (!_activo || _indiceSiguiente >= _notas.Count)
      {
         return;
      }

      if (UsarFallbackTecladoFisico && @event is InputEventKey tecla && tecla.Pressed && !tecla.Echo)
      {
         string accionFallback = MapearAccionDesdeTeclaFisica(tecla);
         if (!string.IsNullOrWhiteSpace(accionFallback))
         {
            ProcesarAccionDetectada(accionFallback);
            return;
         }
      }

      // Camino inmediato por evento: evita depender solo del polling por frame.
      foreach (string accion in _accionesHit)
      {
         if (!EstaAccionPresionada(@event, accion))
         {
            continue;
         }

         ProcesarAccionDetectada(accion);

         return;
      }
   }

   public void CargarNotas(IEnumerable<Nota> notas)
   {
      _notas.Clear();
      _notas.AddRange(notas.OrderBy(n => n.TiempoHitSegundos));
      _accionesHit.Clear();
      foreach (Nota nota in _notas)
      {
         if (!string.IsNullOrWhiteSpace(nota.AccionHit))
         {
            _accionesHit.Add(nota.AccionHit);
         }
      }

      _indiceSiguiente = 0;
   }

   public void IniciarMinijuego()
   {
      if (!RelojDisponible())
      {
         return;
      }

      _indiceSiguiente = 0;
      ResultadoExitoso = false;
      MinijuegoFinalizado = false;
      FallosActuales = 0;
      _teclaAPresionada = false;
      _teclaSPresionada = false;
      _teclaDPresionada = false;
      _teclaFPresionada = false;
      _activo = true;
      _reloj.Iniciar();
   }

   public void FinalizarMinijuego(bool exito)
   {
      if (MinijuegoFinalizado)
      {
         return;
      }

      ResultadoExitoso = exito;
      MinijuegoFinalizado = true;
      _activo = false;

      if (RelojDisponible())
      {
         _reloj.Detener();
      }

      EmitSignal("ProcesamientoFinalizado", ResultadoExitoso);
   }

   public void ProcesarInput(string accion)
   {
      if (!_activo || !RelojDisponible())
      {
         return;
      }

      double tiempoActual = _reloj.ObtenerTiempoCancionSegundos();
      Nota candidata = ObtenerNotaCandidata(accion, tiempoActual);
      if (candidata == null)
      {
         return;
      }

      _indiceSiguiente++;

      if (_indiceSiguiente >= _notas.Count)
      {
         FinalizarMinijuego(FallosActuales < MaxFallosPermitidos);
      }
   }

   public double ObtenerTiempoActualSegundos()
   {
      if (!RelojDisponible())
      {
         return 0.0;
      }

      return _reloj.ObtenerTiempoCancionSegundos();
   }

   private Nota ObtenerNotaCandidata(string accion, double tiempoActual)
   {
      if (_indiceSiguiente >= _notas.Count)
      {
         return null;
      }

      Nota nota = _notas[_indiceSiguiente];
      if (!nota.CorrespondeAccion(accion))
      {
         return null;
      }

      if (!EstaEnVentanaHit(nota, tiempoActual))
      {
         return null;
      }

      return nota;
   }

   private void VerificarFalloPorTiempo()
   {
      if (_indiceSiguiente >= _notas.Count || !RelojDisponible())
      {
         return;
      }

      double tiempoActual = _reloj.ObtenerTiempoCancionSegundos();

      while (_indiceSiguiente < _notas.Count)
      {
         Nota nota = _notas[_indiceSiguiente];
         if (tiempoActual <= nota.TiempoHitSegundos + ObtenerVentanaDespuesSegundos())
         {
            break;
         }

         SumarFallo(1.0);
         _indiceSiguiente++;

         if (MinijuegoFinalizado)
         {
            return;
         }
      }

      if (_indiceSiguiente >= _notas.Count)
      {
         FinalizarMinijuego(true);
      }
   }

   private bool RelojDisponible()
   {
      if (_reloj != null)
      {
         return true;
      }

      GD.PushError("ControlRitmo no tiene un RelojRitmo valido asignado.");
      return false;
   }

   private bool EsAccionDeHitPresionada(InputEvent @event)
   {
      foreach (string accion in _accionesHit)
      {
         if (EstaAccionPresionada(@event, accion))
         {
            return true;
         }
      }

      return false;
   }

   private void ProcesarInputPorPolling()
   {
      if (_indiceSiguiente >= _notas.Count)
      {
         return;
      }

      foreach (string accion in _accionesHit)
      {
         if (!InputMap.HasAction(accion) || !Input.IsActionJustPressed(accion))
         {
            continue;
         }

         ProcesarAccionDetectada(accion);

         // Procesar solo una accion por frame para evitar doble penalizacion.
         return;
      }
   }

   private void ProcesarAccionDetectada(string accion)
   {
      if (!_activo || _indiceSiguiente >= _notas.Count)
      {
         return;
      }

      Nota esperada = _notas[_indiceSiguiente];
      if (accion == esperada.AccionHit)
      {
         ProcesarInput(accion);
      }
      else
      {
         SumarFallo(PenalizacionTeclaErronea);
      }
   }

   private void ProcesarInputFisicoPorFrame()
   {
      bool aActual = TeclaPresionada(Key.A);
      bool sActual = TeclaPresionada(Key.S);
      bool dActual = TeclaPresionada(Key.D);
      bool fActual = TeclaPresionada(Key.F);

      if (aActual && !_teclaAPresionada)
      {
         ProcesarAccionDetectada(AccionTeclaA);
      }
      else if (sActual && !_teclaSPresionada)
      {
         ProcesarAccionDetectada(AccionTeclaS);
      }
      else if (dActual && !_teclaDPresionada)
      {
         ProcesarAccionDetectada(AccionTeclaD);
      }
      else if (fActual && !_teclaFPresionada)
      {
         ProcesarAccionDetectada(AccionTeclaF);
      }

      _teclaAPresionada = aActual;
      _teclaSPresionada = sActual;
      _teclaDPresionada = dActual;
      _teclaFPresionada = fActual;
   }

   private static bool TeclaPresionada(Key key)
   {
      return Input.IsPhysicalKeyPressed(key) || Input.IsKeyPressed(key);
   }

   private bool EstaEnVentanaHit(Nota nota, double tiempoActual)
   {
      double delta = tiempoActual - nota.TiempoHitSegundos;

      if (UsarVentanaAsimetrica)
      {
         return delta >= -ObtenerVentanaAntesSegundos() && delta <= ObtenerVentanaDespuesSegundos();
      }

      return Math.Abs(delta) <= VentanaHitSegundos;
   }

   private double ObtenerVentanaAntesSegundos()
   {
      return VentanaAntesHitSegundos > 0.0 ? VentanaAntesHitSegundos : VentanaHitSegundos;
   }

   private double ObtenerVentanaDespuesSegundos()
   {
      return VentanaDespuesHitSegundos > 0.0 ? VentanaDespuesHitSegundos : VentanaHitSegundos;
   }

   private string MapearAccionDesdeTeclaFisica(InputEventKey tecla)
   {
      if (tecla.PhysicalKeycode == Key.A)
      {
         return AccionTeclaA;
      }

      if (tecla.PhysicalKeycode == Key.S)
      {
         return AccionTeclaS;
      }

      if (tecla.PhysicalKeycode == Key.D)
      {
         return AccionTeclaD;
      }

      if (tecla.PhysicalKeycode == Key.F)
      {
         return AccionTeclaF;
      }

      return string.Empty;
   }

   private bool EstaAccionPresionada(InputEvent @event, string accion)
   {
      if (string.IsNullOrWhiteSpace(accion))
      {
         return false;
      }

      if (!InputMap.HasAction(accion))
      {
         if (_accionesFaltantesReportadas.Add(accion))
         {
            GD.PushWarning($"La accion '{accion}' no existe en InputMap. Configurala en Project Settings > Input Map.");
         }

         return false;
      }

      return @event.IsActionPressed(accion);
   }

   private void SumarFallo(double cantidad)
   {
      if (cantidad <= 0.0 || MinijuegoFinalizado)
      {
         return;
      }

      FallosActuales += cantidad;
      if (FallosActuales >= MaxFallosPermitidos - float.Epsilon)
      {
         FinalizarMinijuego(false);
      }
   }
}
