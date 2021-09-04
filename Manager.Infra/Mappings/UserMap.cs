using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Infra.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Id)
                .UseMySqlIdentityColumn()
                .HasColumnType("BIGINT UNSIGNED");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnName("Name")
                .HasColumnType("VARCHAR(80)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(180)
                .HasColumnName("Email")
                .HasColumnType("VARCHAR(180)");

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("Password")
                .HasColumnType("VARCHAR(30)");
        }
    }
}
