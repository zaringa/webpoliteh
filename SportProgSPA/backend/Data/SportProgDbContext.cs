using Microsoft.EntityFrameworkCore;
using SportProg.Api.Models;

namespace SportProg.Api.Data;

public class SportProgDbContext : DbContext
{
    public SportProgDbContext(DbContextOptions<SportProgDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AlgorithmTopic> AlgorithmTopics => Set<AlgorithmTopic>();
    public DbSet<ChallengeTask> ChallengeTasks => Set<ChallengeTask>();
    public DbSet<LearningCollection> LearningCollections => Set<LearningCollection>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
    public DbSet<FavoriteAlgorithm> FavoriteAlgorithms => Set<FavoriteAlgorithm>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<AlgorithmTopic>()
            .HasIndex(topic => topic.Slug)
            .IsUnique();

        modelBuilder.Entity<LearningCollection>()
            .HasIndex(collection => collection.Slug)
            .IsUnique();

        modelBuilder.Entity<FavoriteAlgorithm>()
            .HasKey(favorite => new { favorite.UserId, favorite.AlgorithmTopicId });

        modelBuilder.Entity<FavoriteAlgorithm>()
            .HasOne(favorite => favorite.User)
            .WithMany(user => user.Favorites)
            .HasForeignKey(favorite => favorite.UserId);

        modelBuilder.Entity<FavoriteAlgorithm>()
            .HasOne(favorite => favorite.AlgorithmTopic)
            .WithMany(topic => topic.Favorites)
            .HasForeignKey(favorite => favorite.AlgorithmTopicId);

        modelBuilder.Entity<CollectionItem>()
            .HasKey(item => new { item.LearningCollectionId, item.AlgorithmTopicId });

        modelBuilder.Entity<CollectionItem>()
            .HasOne(item => item.LearningCollection)
            .WithMany(collection => collection.Items)
            .HasForeignKey(item => item.LearningCollectionId);

        modelBuilder.Entity<CollectionItem>()
            .HasOne(item => item.AlgorithmTopic)
            .WithMany(topic => topic.CollectionItems)
            .HasForeignKey(item => item.AlgorithmTopicId);

        modelBuilder.Entity<ChallengeTask>()
            .HasOne(task => task.AlgorithmTopic)
            .WithMany(topic => topic.Tasks)
            .HasForeignKey(task => task.AlgorithmTopicId);

        modelBuilder.Entity<Submission>()
            .HasOne(submission => submission.User)
            .WithMany(user => user.Submissions)
            .HasForeignKey(submission => submission.UserId);

        modelBuilder.Entity<Submission>()
            .HasOne(submission => submission.ChallengeTask)
            .WithMany(task => task.Submissions)
            .HasForeignKey(submission => submission.ChallengeTaskId);
    }
}
