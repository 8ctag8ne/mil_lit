using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MIL_LIT;

public partial class MilLitDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public MilLitDbContext()
    {
    }

    public MilLitDbContext(DbContextOptions<MilLitDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookTag> BookTags { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Safe> Saves { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Author).HasMaxLength(50);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.Filepath).HasMaxLength(50);
            entity.Property(e => e.GeneralInfo).HasColumnType("text");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Users");

            // entity.HasMany(e => e.Tags)
            //     .WithMany(e => e.Books);
        });

        modelBuilder.Entity<BookTag>(entity =>
        {
            entity.HasKey(bt => new { bt.BookId, bt.TagId });

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookTags_Books");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookTags_Tags");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.ParentCommentId).HasColumnName("parentCommentID");
            entity.Property(e => e.PostedAt)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Text).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Book).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Books");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_Comments");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(lk => new { lk.BookId, lk.UserId });

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Books");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Users");
        });

        modelBuilder.Entity<Safe>(entity =>
        {
            entity.HasKey(sf => new { sf.BookId, sf.UserId });

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Saves_Books");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Saves_Users");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentTagId).HasColumnName("ParentTagID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Tags)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tags_Users");

            entity.HasOne(d => d.ParentTag).WithMany(p => p.InverseParentTag)
                .HasForeignKey(d => d.ParentTagId)
                .HasConstraintName("FK_Tags_Tags");

            // entity.HasMany(e => e.Books)
            //     .WithMany(e => e.Tags);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
