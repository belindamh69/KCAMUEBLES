using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Models;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Proyecto_Final.Controllers
{
    public class ContactoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public ContactoController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.TipoMensaje = TempData["TipoMensaje"];
            return View();
        }

        [HttpPost]
        public ActionResult Index(Contacto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var emailSettings = configuration.GetSection("EmailSettings");
                var host = emailSettings["Host"];
                var userName = emailSettings["UserName"];
                var password = emailSettings["Password"];
                var fromEmail = emailSettings["FromEmail"];
                var fromName = emailSettings["FromName"] ?? "KCAMUEBLES";
                var toEmail = emailSettings["ToEmail"];
                var port = emailSettings.GetValue<int>("Port", 587);
                var enableSsl = emailSettings.GetValue<bool>("EnableSsl", true);

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(userName) ||
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromEmail) ||
                    string.IsNullOrWhiteSpace(toEmail) || userName.Contains("PENDIENTE") || fromEmail.Contains("PENDIENTE"))
                {
                    TempData["Mensaje"] = "La configuración de correo no está completa. Falta el correo Gmail completo.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                using var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"Nuevo mensaje de contacto KCAMUEBLES: {model.Asunto}",
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);
                mail.ReplyToList.Add(new MailAddress(model.Correo, model.NombreCompleto));

                var html = CrearCorreoHtml(model);
                var vistaHtml = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
                var logoPath = Path.Combine(environment.WebRootPath, "imagenes", "logo-kcamuebles-redondo.jpeg");

                if (System.IO.File.Exists(logoPath))
                {
                    var logo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = "logoKca",
                        TransferEncoding = TransferEncoding.Base64
                    };
                    vistaHtml.LinkedResources.Add(logo);
                }

                mail.AlternateViews.Add(vistaHtml);

                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = enableSsl
                };

                smtp.Send(mail);

                TempData["Mensaje"] = "Mensaje enviado correctamente.";
                TempData["TipoMensaje"] = "ok";
            }
            catch
            {
                TempData["Mensaje"] = "No se pudo enviar el mensaje. Revisa la configuración del correo.";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("Index");
        }

        private static string CrearCorreoHtml(Contacto model)
        {
            var nombre = WebUtility.HtmlEncode(model.NombreCompleto);
            var correo = WebUtility.HtmlEncode(model.Correo);
            var asunto = WebUtility.HtmlEncode(model.Asunto);
            var mensaje = WebUtility.HtmlEncode(model.Mensaje).Replace("\n", "<br>");

            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='utf-8'>
</head>
<body style='margin:0;padding:0;background:#edd9cc;font-family:Arial,Helvetica,sans-serif;color:#2f211d;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background:#edd9cc;padding:28px 12px;'>
        <tr>
            <td align='center'>
                <table width='620' cellpadding='0' cellspacing='0' style='max-width:620px;background:#fff8f1;border-radius:18px;overflow:hidden;border:1px solid #b8aa97;box-shadow:0 18px 38px rgba(47,33,29,0.14);'>
                    <tr>
                        <td style='background:#8b0909;padding:24px;text-align:center;'>
                            <img src='cid:logoKca' alt='KCAMUEBLES' width='92' height='92' style='border-radius:50%;display:block;margin:0 auto 12px;border:2px solid #edd9cc;'>
                            <h1 style='margin:0;color:#fff8f1;font-family:Georgia,serif;font-size:30px;letter-spacing:2px;'>KCAMUEBLES</h1>
                            <p style='margin:6px 0 0;color:#edd9cc;font-size:15px;letter-spacing:1px;'>Tu espacio, tu estilo</p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:30px;'>
                            <h2 style='margin:0 0 18px;color:#8b0909;font-family:Georgia,serif;font-size:26px;'>Nuevo mensaje de contacto</h2>
                            <p style='margin:0 0 22px;line-height:1.7;color:#5f5148;'>Una persona llenó el formulario de contacto en la página de KCAMUEBLES.</p>

                            <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;'>
                                <tr>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;width:130px;color:#8b0909;font-weight:bold;'>Nombre</td>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;'>{nombre}</td>
                                </tr>
                                <tr>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;color:#8b0909;font-weight:bold;'>Correo</td>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;'><a href='mailto:{correo}' style='color:#8b0909;text-decoration:none;'>{correo}</a></td>
                                </tr>
                                <tr>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;color:#8b0909;font-weight:bold;'>Asunto</td>
                                    <td style='padding:12px 0;border-bottom:1px solid #ead8ca;'>{asunto}</td>
                                </tr>
                            </table>

                            <div style='margin-top:24px;padding:18px 20px;background:#f7efe8;border-left:4px solid #8b0909;border-radius:12px;line-height:1.7;'>
                                {mensaje}
                            </div>

                            <p style='margin:24px 0 0;color:#806350;font-size:13px;'>Puedes responder directamente a este correo para contactar a la persona.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}

