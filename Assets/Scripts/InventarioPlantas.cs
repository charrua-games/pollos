using Godot;
using System;

public partial class InventarioPlantas
{
   private Planta _plantaActual;

   public void RecibirPlanta(Planta planta)
   {
      _plantaActual = planta;
   }
   public Color ObtenerColorPlantaActual()
   {
      if (_plantaActual == null)
      {
         throw new InvalidOperationException("No hay una planta actual en el inventario.");
      }

      return _plantaActual.ObtenerColor();
   }
}
