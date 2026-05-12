using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Models;
using System.Text.Json;

namespace Proyecto_Final.Controllers
{
    public class ResenasController : Controller
    {
        private static readonly object archivoLock = new object();
        private static readonly string rutaArchivo = Path.Combine(AppContext.BaseDirectory, "App_Data", "resenas.json");
        public static List<Resenas> lista = CargarResenas();

        public IActionResult Index(int? filtro)
        {
            ViewBag.Filtro = filtro;

            var data = lista;

            if (filtro.HasValue)
            {
                if (filtro == 5)
                    data = lista.Where(r => r.Calificacion == 5).ToList();
                else if (filtro == 4)
                    data = lista.Where(r => r.Calificacion == 4).ToList();
                else if (filtro == 3)
                    data = lista.Where(r => r.Calificacion <= 3).ToList();
            }

            ViewBag.Total = lista.Count;
            ViewBag.Promedio = lista.Count > 0
                ? lista.Average(r => r.Calificacion)
                : 0;

            return View(data);
        }

        [HttpPost]
        public IActionResult Index(Resenas model)
        {
            if (string.IsNullOrWhiteSpace(model.Tipo))
                model.Tipo = "General";

            if (model.Calificacion == 0)
                model.Calificacion = 5;

            if (string.IsNullOrWhiteSpace(model.Nombre))
                model.Nombre = "Anónimo";

            if (model.Tipo == "General")
                model.Producto = "";

            model.Fecha = DateTime.Now;

            ModelState.Clear();
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                model.Id = lista.Count == 0 ? 1 : lista.Max(r => r.Id) + 1;
                lista.Add(model);
                GuardarResenas();
                TempData["Mensaje"] = "Reseña enviada";
                return RedirectToAction("Index");
            }

            return View(lista);
        }

        private static List<Resenas> CargarResenas()
        {
            try
            {
                if (!System.IO.File.Exists(rutaArchivo))
                    return new List<Resenas>();

                var json = System.IO.File.ReadAllText(rutaArchivo);
                return JsonSerializer.Deserialize<List<Resenas>>(json) ?? new List<Resenas>();
            }
            catch
            {
                return new List<Resenas>();
            }
        }

        private static void GuardarResenas()
        {
            lock (archivoLock)
            {
                var carpeta = Path.GetDirectoryName(rutaArchivo);
                if (!string.IsNullOrWhiteSpace(carpeta))
                    Directory.CreateDirectory(carpeta);

                var opciones = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(lista, opciones);
                System.IO.File.WriteAllText(rutaArchivo, json);
            }
        }
    }
}
