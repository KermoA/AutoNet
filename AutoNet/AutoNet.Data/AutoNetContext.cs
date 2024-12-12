using Microsoft.EntityFrameworkCore;

namespace AutoNet.Data
{
	public class AutoNetContext : DbContext
	{
		public AutoNetContext(DbContextOptions<AutoNetContext> options)
		: base(options) { }
	}
}
