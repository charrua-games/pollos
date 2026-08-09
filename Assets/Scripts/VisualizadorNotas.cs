using Godot;

public partial class VisualizadorNotas : Node2D
{
   [Export] public NodePath RutaControlRitmo { get; set; }
   [Export] public NodePath RutaHitLine { get; set; }
   [Export] public bool UsarYHitDesdeHitLine { get; set; } = true;
   [Export] public float OffsetYHit { get; set; } = 0f;

   [Export] public float XCarril1 { get; set; } = 220f;
   [Export] public float XCarril2 { get; set; } = 340f;
   [Export] public float XCarril3 { get; set; } = 460f;
   [Export] public float XCarril4 { get; set; } = 580f;

   [Export] public string AccionCarril1 { get; set; } = "hit_a";
   [Export] public string AccionCarril2 { get; set; } = "hit_s";
   [Export] public string AccionCarril3 { get; set; } = "hit_d";
   [Export] public string AccionCarril4 { get; set; } = "hit_f";

   [Export] public float YSpawn { get; set; } = -80f;
   [Export] public float YHit { get; set; } = 620f;
   [Export] public double LeadTimeSegundos { get; set; } = 1.5;

   [Export] public float RadioNota { get; set; } = 12f;
   [Export] public Color ColorNota { get; set; } = new Color(0.95f, 0.85f, 0.20f, 1f);
   [Export] public Color ColorNotaActual { get; set; } = new Color(0.30f, 0.95f, 0.40f, 1f);
   [Export] public bool OcultarNotasConsumidas { get; set; } = true;

   [Export] public bool DibujarHitLine { get; set; } = true;
   [Export] public float MitadAnchoHitLine { get; set; } = 260f;
   [Export] public Color ColorHitLine { get; set; } = new Color(0.85f, 0.15f, 0.15f, 1f);

   private ControlRitmo _controlRitmo;

   public override void _Ready()
   {
      if (RutaControlRitmo == null || RutaControlRitmo.IsEmpty)
      {
         GD.PushError("VisualizadorNotas requiere RutaControlRitmo asignada.");
         return;
      }

      _controlRitmo = GetNodeOrNull<ControlRitmo>(RutaControlRitmo);
      if (_controlRitmo == null)
      {
         GD.PushError("VisualizadorNotas no pudo resolver ControlRitmo.");
      }

      if (UsarYHitDesdeHitLine && RutaHitLine != null && !RutaHitLine.IsEmpty)
      {
         Node2D hitLine = GetNodeOrNull<Node2D>(RutaHitLine);
         if (hitLine != null)
         {
            YHit = hitLine.GlobalPosition.Y + OffsetYHit;
         }
         else
         {
            GD.PushWarning("VisualizadorNotas no pudo resolver RutaHitLine; se mantiene YHit manual.");
         }
      }
   }

   public override void _Process(double delta)
   {
      if (_controlRitmo != null)
      {
         QueueRedraw();
      }
   }

   public override void _Draw()
   {
      if (_controlRitmo == null)
      {
         return;
      }

      if (DibujarHitLine)
      {
         Vector2 desde = new(XCarril1 - 40f, YHit);
         Vector2 hasta = new(XCarril4 + 40f, YHit);
         DrawLine(desde, hasta, ColorHitLine, 3f);
      }

      var notas = _controlRitmo.Notas;
      if (notas.Count == 0)
      {
         return;
      }

      int indiceActual = _controlRitmo.IndiceSiguiente;
      double tiempoActual = _controlRitmo.ObtenerTiempoActualSegundos();
      float velocidadY = ObtenerVelocidadY();

      for (int i = 0; i < notas.Count; i++)
      {
         if (OcultarNotasConsumidas && i < indiceActual)
         {
            continue;
         }

         Nota nota = notas[i];
         float x = ObtenerXCarrilPorAccion(nota.AccionHit);
         double tiempoRestante = nota.TiempoHitSegundos - tiempoActual;
         float y = YHit - (float)(tiempoRestante * velocidadY);

         if (y < YSpawn - 120f || y > YHit + 180f)
         {
            continue;
         }

         Color color = i == indiceActual ? ColorNotaActual : ColorNota;
         DrawCircle(new Vector2(x, y), RadioNota, color);
      }
   }

   private float ObtenerVelocidadY()
   {
      if (LeadTimeSegundos <= 0.0)
      {
         return 1f;
      }

      return (YHit - YSpawn) / (float)LeadTimeSegundos;
   }

   private float ObtenerXCarrilPorAccion(string accion)
   {
      if (accion == AccionCarril1) return XCarril1;
      if (accion == AccionCarril2) return XCarril2;
      if (accion == AccionCarril3) return XCarril3;
      if (accion == AccionCarril4) return XCarril4;
      return XCarril1;
   }
}
