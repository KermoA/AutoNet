using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Models.Emails;
using Microsoft.AspNetCore.Mvc;

namespace AutoNet.Controllers
{
	public class EmailsController : Controller
	{
		private readonly IEmailServices _emailServices;

		public EmailsController(IEmailServices emailServices)
		{
			_emailServices = emailServices;
		}

		public IActionResult Index()
		{
			ViewData["Title"] = "Send Email";

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendEmail(EmailsIndexViewModel vm)
		{
			try
			{
				var dto = new EmailDto()
				{
					To = vm.To.Contains(",")
						? vm.To.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(email => email.Trim()).ToList()
						: new List<string> { vm.To.Trim() },
					Subject = vm.Subject,
					Body = vm.Body,
					Attachments = vm.Attachments
				};

				await _emailServices.SendEmail(dto); // Ensure SendEmail is async
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Email sending failed: {ex.Message}");
			}

			return RedirectToAction(nameof(Index));
		}

	}
}
