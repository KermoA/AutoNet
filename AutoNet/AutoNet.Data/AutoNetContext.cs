using AutoNet.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoNet.Data
{
	public class AutoNetContext : IdentityDbContext<ApplicationUser>
    {
		public AutoNetContext(DbContextOptions<AutoNetContext> options)
		: base(options) { }

        public DbSet<IdentityRole> IdentityRoles { get; set; }
    }
}
