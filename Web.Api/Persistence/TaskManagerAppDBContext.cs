using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence;

public partial class TaskManagerAppDBContext : DbContext
{
    public TaskManagerAppDBContext()
    {
    }

    public TaskManagerAppDBContext(DbContextOptions<TaskManagerAppDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<SubTask> SubTasks { get; set; }

    public virtual DbSet<TaskItem> TaskItems { get; set; }

    public virtual DbSet<TaskItemNote> TaskItemNotes { get; set; }

    public virtual DbSet<TaskItemStatusHistory> TaskItemStatusHistories { get; set; }

    public virtual DbSet<TaskWithinList> TaskWithinLists { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<List>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.Lists)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lists_Users");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(32);
        });

        modelBuilder.Entity<SubTask>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.SubTasks)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubTasks_Users");

            entity.HasOne(d => d.SubTaskItem).WithMany(p => p.SubTaskSubTaskItems)
                .HasForeignKey(d => d.SubTaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubTasks_TaskItems1");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.SubTaskTaskItems)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubTasks_TaskItems");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.DueDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Title).IsUnicode(false);

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskItems)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItems_Users");
        });

        modelBuilder.Entity<TaskItemNote>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Note).IsUnicode(false);

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskItemNotes)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItemNotes_Users");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.TaskItemNotes)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItemNotes_TaskItems");
        });

        modelBuilder.Entity<TaskItemStatusHistory>(entity =>
        {
            entity.ToTable("TaskItemStatusHistory");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItemStatusHistory_Users");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItemStatusHistory_Statuses");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskItemStatusHistory_TaskItems");
        });

        modelBuilder.Entity<TaskWithinList>(entity =>
        {
            entity.HasKey(e => new { e.TaskListId, e.TaskItemId });

            entity.ToTable("TaskWithinList");

            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskWithinLists)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_Users");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.TaskWithinLists)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_TaskItems");

            entity.HasOne(d => d.TaskList).WithMany(p => p.TaskWithinLists)
                .HasForeignKey(d => d.TaskListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_Lists");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
