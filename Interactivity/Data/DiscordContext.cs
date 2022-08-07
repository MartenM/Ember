using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Interactivity.Data;

public class DiscordContext : DbContext
{
    public DbSet<EmberGuild> Guilds { get; set; }
    public DbSet<EmberMember> Members { get; set; }
    public DbSet<EmberSanction> Sanctions { get; set; }

    public String DbPath { get; }

    public DiscordContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "discord.db");
    }
    
    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmberGuild>(guild =>
        {
            guild.HasKey(g => g.GuildId);
            guild.Property(g => g.Name).HasDefaultValue("**unknown**");
        }); 

        modelBuilder.Entity<EmberMember>(member =>
        {
            member.HasKey(m => m.Id);
            member.Property(m => m.Id).ValueGeneratedOnAdd();
            
            member.HasIndex(m => new {m.GuildId, MemberId = m.DiscordId}).IsUnique();
            member.HasOne(m => m.Guild).WithMany(g => g.Members).HasForeignKey(m => m.GuildId);
        });

        modelBuilder.Entity<EmberSanction>(sanction =>
        {
            sanction.HasKey(s => s.Id);
            sanction.Property(s => s.Id).ValueGeneratedOnAdd();
            sanction.HasOne(s => s.By).WithMany(m => m.IssuedSanctions).HasForeignKey(m => m.ById);
            sanction.HasOne(s => s.Receiver).WithMany(m => m.ReceivedSanctions).HasForeignKey(m => m.ReceiverId);
            sanction.Property(s => s.Inserted).HasDefaultValue(DateTime.Now);
        });
    }
}

/// <summary>
/// Default guild class. Holds data related to the guild.
/// This is the base for any guild.
/// </summary>
[Table("guilds")]
public class EmberGuild
{
    public ulong GuildId;
    
    // Meta data, not always up-to-date
    public string Name { get; set; }
    
    // Functional data
    public ulong? AuditChannel { get; set; }
    public ulong? LogChannel { get; set; }

    public ulong? ModRole { get; set; }
    public ulong? MutedRole { get; set; }

    public List<EmberMember> Members { get; set; }
}

/// <summary>
/// Represents an Ember member in a guild. Ember members are guild specific.
/// </summary>
[Table("members")]
public class EmberMember
{
    public int Id { get; set; }
    
    // Same as the discord id
    public ulong DiscordId { get; set; }

    // Sanction related data.
    public List<EmberSanction> IssuedSanctions { get; set; }
    public List<EmberSanction> ReceivedSanctions { get; set; }

    public string? ModeratorsNote { get; set; }

    // Foreign keys
    public ulong GuildId { get; set; }
    public EmberGuild Guild { get; set; }
}

/// <summary>
/// History record of a sanction that has been applied to a user.
/// </summary>
[Table("sanctions")]
public class EmberSanction
{
    public int Id { get; set; }

    public EmberMember Receiver { get; set; }
    public int ReceiverId { get; set; }
    public EmberMember By { get; set; }
    public int ById { get; set; }

    public String Reason { get; set; }
    
    public DateTime Inserted { get; set; }
    
}