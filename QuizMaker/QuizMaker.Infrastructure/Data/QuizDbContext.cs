using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Infrastructure.Data;
public class QuizDbContext : IdentityDbContext {

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }

    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QuizQuestion>()
            .HasKey(qq => new { qq.QuizId, qq.QuestionId });

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Quiz)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Question)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        //dont query IsDeleted == false
        modelBuilder.Entity<Quiz>()
            .HasQueryFilter(q => !q.IsDeleted);

        //restrictiosn
        modelBuilder.Entity<Quiz>()
            .Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(500); //??

        modelBuilder.Entity<Question>()
            .Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(500);


        modelBuilder.Entity<Question>()
            .Property(q => q.Answer)
            .IsRequired()
            .HasMaxLength(500);

        // indexes
        modelBuilder.Entity<Question>()
            .HasIndex(q => q.Text);


        //modelBuilder.Entity<Question>()
        //    .HasIndex(q => q.Answer);

    }
}
