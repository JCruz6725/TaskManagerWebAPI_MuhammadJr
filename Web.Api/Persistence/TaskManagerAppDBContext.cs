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

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TodoTaskScheduler");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<List>(entity =>
        {
            entity.ToTable("List");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Name).IsUnicode(false);

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.Lists)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_List_Users");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
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
                .HasConstraintName("FK_SubTasks_SubTasks1");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.SubTaskTaskItems)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubTasks_TaskItem");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("TaskItem");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.DueDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Title).IsUnicode(false);

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskItems)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_TaskItem");
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
                .HasConstraintName("FK_TaskItemNotes_TaskItem");
        });

        modelBuilder.Entity<TaskItemStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TaskItemkStatusHistory");

            entity.ToTable("TaskItemStatusHistory");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Created).HasColumnType("smalldatetime");

            entity.HasOne(d => d.CreatedUser).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskStatusHistory_Users");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskStatusHistory_Statuses");

            entity.HasOne(d => d.TaskItem).WithMany(p => p.TaskItemStatusHistories)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskStatusHistory_TaskItem");
        });

        modelBuilder.Entity<TaskWithinList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TaskWithinList");

            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.CreatedUser).WithMany()
                .HasForeignKey(d => d.CreatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_Users");

            entity.HasOne(d => d.TaskItem).WithMany()
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_TaskItem");

            entity.HasOne(d => d.TaskList).WithMany()
                .HasForeignKey(d => d.TaskListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskWithinList_List");
        });

        modelBuilder.Entity<User>(entity =>
        {


            entity.Property(e => e.Id).HasDefaultValueSql("(newid())"); 
            entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.FirstName).IsUnicode(false);
            entity.Property(e => e.LastName).IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
