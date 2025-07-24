using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using U1_evaluacion_sumativa.Models;
using Rotativa.AspNetCore;
using MimeKit;
using System.Net.Mail;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace U1_evaluacion_sumativa.Controllers
{
    public class InicioProyectoesController : Controller
    {
        private readonly DbProyectoRedesContext _context;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IConverter _converter;
        public InicioProyectoesController(IConverter converter, ICompositeViewEngine viewEngine, DbProyectoRedesContext context)
        {
            _context = context;
            _viewEngine = viewEngine;
            _converter = converter;
        }

        // GET: InicioProyectoes
        public async Task<IActionResult> Index()
        {
            var dbProyectoRedesContext = _context.InicioProyectos.Include(i => i.Proyecto);
            return View(await dbProyectoRedesContext.ToListAsync());
        }

        // GET: InicioProyectoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(m => m.Id == id);

            

            if (inicioProyecto == null)
            {
                return NotFound();
            }
            var trabajadores = await _context.Trabajadores
                .Include(u => u.Usuario)
                .Where(p => p.InicioproyectoId == id)
                .ToListAsync();
            if (trabajadores == null)
            {
                return NotFound();
            }
            var direccionamientos = await _context.DireccionamientoIps
                .Where(p => p.InicioProyectoId == id)
                .ToListAsync();


            ViewBag.Trabajadores = trabajadores;
            ViewBag.DireccionamientoIp = direccionamientos;
            return View(inicioProyecto);
        }

        // GET: InicioProyectoes/Create
        public IActionResult Create(int? id)
        {
            ViewBag.Id_Proyecto = id;
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre");
            return View();
        }

        // POST: InicioProyectoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntidadInvolucrada,FechaInicio,FechaFinalizacion,Estado,Observaciones,ProyectoId")] InicioProyecto inicioProyecto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inicioProyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", inicioProyecto.ProyectoId);
            return View(inicioProyecto);
        }

        // GET: InicioProyectoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioProyecto = await _context.InicioProyectos.FindAsync(id);
            if (inicioProyecto == null)
            {
                return NotFound();
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Id", inicioProyecto.ProyectoId);
            return View(inicioProyecto);
        }

        // POST: InicioProyectoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntidadInvolucrada,FechaInicio,FechaFinalizacion,Estado,Observaciones,ProyectoId")] InicioProyecto inicioProyecto)
        {
            if (id != inicioProyecto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inicioProyecto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InicioProyectoExists(inicioProyecto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Id", inicioProyecto.ProyectoId);
            return View(inicioProyecto);
        }

        // GET: InicioProyectoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inicioProyecto == null)
            {
                return NotFound();
            }

            return View(inicioProyecto);
        }

        // POST: InicioProyectoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inicioProyecto = await _context.InicioProyectos.FindAsync(id);
            if (inicioProyecto != null)
            {
                _context.InicioProyectos.Remove(inicioProyecto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InicioProyectoExists(int id)
        {
            return _context.InicioProyectos.Any(e => e.Id == id);
        }

        public async Task<IActionResult> VisualizacionPDF(int? id)
        {
            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inicioProyecto == null)
            {
                return NotFound();
            }

            return View(inicioProyecto);
        }

        public async Task<IActionResult> VerPDF(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inicioProyecto == null)
            {
                return NotFound();
            }
            var trabajadores = await _context.Trabajadores
                .Include(u => u.Usuario)
                .Where(p => p.InicioproyectoId == id)
                .ToListAsync();
            if (trabajadores == null)
            {
                return NotFound();
            }
            var direccionamientos = await _context.DireccionamientoIps
                .Where(p => p.InicioProyectoId == id)
                .ToListAsync();


            ViewBag.Trabajadores = trabajadores;
            ViewBag.DireccionamientoIp = direccionamientos;
            return View(inicioProyecto);
        }
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            ViewData.Model = model;
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            if (!viewResult.Success)
                throw new InvalidOperationException($"No se encontró la vista '{viewName}'.");

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
        public async Task<IActionResult> DescargarPDF(int? id)
        {
            if (id == null) return NotFound();

            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inicioProyecto == null) return NotFound();

            var trabajadores = await _context.Trabajadores
                .Include(u => u.Usuario)
                .Where(p => p.InicioproyectoId == id)
                .ToListAsync();

            var direccionamientos = await _context.DireccionamientoIps
                .Where(p => p.InicioProyectoId == id)
                .ToListAsync();

            ViewBag.Trabajadores = trabajadores;
            ViewBag.DireccionamientoIp = direccionamientos;

            // Renderiza la vista VerPDF a string HTML
            string htmlContent = await RenderViewToStringAsync("VerPDF", inicioProyecto);

            // Configura el PDF con DinkToPdf
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var pdfBytes = _converter.Convert(doc);

            return File(pdfBytes, "application/pdf", "Proyecto.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> EnviarPDF(int id)
        {
            var inicioProyecto = await _context.InicioProyectos
                .Include(i => i.Proyecto)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inicioProyecto == null)
                return NotFound();

            var trabajadores = await _context.Trabajadores
                .Include(t => t.Usuario)
                .Where(t => t.InicioproyectoId == id)
                .ToListAsync();

            // Generar el PDF
            var pdf = await new ViewAsPdf("VerPDF", inicioProyecto)
                .BuildFile(ControllerContext);

            // Crear el mensaje
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TuNombre", "tucorreo@dominio.com"));

            foreach (var t in trabajadores)
            {
                if (!string.IsNullOrEmpty(t.Usuario?.Correo))
                    message.To.Add(MailboxAddress.Parse(t.Usuario.Correo));
            }

            message.Subject = $"Informe del Proyecto: {inicioProyecto.Proyecto.Nombre}";

            var builder = new BodyBuilder
            {
                TextBody = "Estimado(a),\n\nAdjunto encontrará el informe del proyecto.\n\nSaludos."
            };

            builder.Attachments.Add("informe.pdf", pdf, ContentType.Parse("application/pdf"));
            message.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                // Aqui agregar una cuenta gmail y la contraseña de aplicaion
                await smtp.AuthenticateAsync("correo", "Contraseña-aplicacion");
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }

            TempData["Mensaje"] = "PDF enviado correctamente por correo.";
            return RedirectToAction("VisualizacionPDF", new { id });
        }

    }
}
