using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WizLib_Model.Models;

namespace WizLib_DataAccess.FluentConfig
{
    public class FluentBookAuthorConfig : IEntityTypeConfiguration<Fluent_BookAuthor>
    {
        public void Configure(EntityTypeBuilder<Fluent_BookAuthor> modelBuilder)
        {
            //many to many
            modelBuilder.HasKey(ba => new { ba.Author_Id, ba.Book_Id });
            modelBuilder.HasOne(b => b.Fluent_Book)
                .WithMany(b => b.Fluent_BookAuthors).HasForeignKey(x => x.Book_Id);
            modelBuilder.HasOne(b => b.Fluent_Author)
                 .WithMany(b => b.Fluent_BookAuthors).HasForeignKey(x => x.Author_Id);
        }
    }
}
