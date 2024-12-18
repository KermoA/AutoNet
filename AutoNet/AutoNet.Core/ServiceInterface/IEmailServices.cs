using AutoNet.Core.Dto;

namespace AutoNet.Core.ServiceInterface
{
	public interface IEmailServices
	{
		Task SendEmail(EmailDto dto);
	}
}
