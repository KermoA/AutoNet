using Microsoft.AspNetCore.Http;

namespace AutoNet.Core.Dto
{
	public class EmailDto
	{
		public List<string> To { get; set; } = new();
		public string Subject { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
		public List<IFormFile> Attachments { get; set; }
	}
}
