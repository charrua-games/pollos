using Godot;
using System;

public partial class RelojRitmo : Node
{
   [Export] public double OffsetGlobalSegundos { get; set; }

   private double _inicioMonotonicoSegundos;
   private bool _corriendo;

   public void Iniciar()
   {
      _inicioMonotonicoSegundos = ObtenerTiempoMonotonicoSegundos();
      _corriendo = true;
   }

   public void Reiniciar()
   {
      Iniciar();
   }

   public void Detener()
   {
      _corriendo = false;
   }

   public double ObtenerTiempoCancionSegundos()
   {
      if (!_corriendo)
      {
         return 0.0;
      }

      double tiempo = ObtenerTiempoMonotonicoSegundos() - _inicioMonotonicoSegundos + OffsetGlobalSegundos;
      return Math.Max(0.0, tiempo);
   }

   public static double ObtenerTiempoMonotonicoSegundos()
   {
      return Time.GetTicksUsec() / 1_000_000.0;
   }
}
