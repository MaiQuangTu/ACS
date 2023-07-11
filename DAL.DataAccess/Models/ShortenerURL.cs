using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.DataAccess.Models
{
	public class ShortenerURL
	{
		public int Id { get; set; }	
		public string? LongUrl { get; set; }	
		public string? ShortUrl { get; set; }	
	}

	public class ShortenerURLConfiguration : IEntityTypeConfiguration<ShortenerURL>
	{
		public void Configure(EntityTypeBuilder<ShortenerURL> builder)
		{
			builder.Property(e => e.Id).UseIdentityColumn();
			builder.Property(e => e.LongUrl).HasMaxLength(1000);
			builder.Property(e => e.ShortUrl).HasMaxLength(1000);

			builder.HasKey(e => e.Id).HasName("PK_TK_ShortenerURL");
			builder.ToTable("ShortenerURL");
		}
	}

}
