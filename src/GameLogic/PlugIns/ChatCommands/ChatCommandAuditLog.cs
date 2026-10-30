// <copyright file="ChatCommandAuditLog.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Text.Json;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Connects the GameMaster chat commands to the same durable audit log which is used
/// for actions performed through the admin panel / API (see <see cref="IAdminAuditRepository"/>).
/// </summary>
/// <remarks>
/// Chat commands run deep in the game logic, which doesn't use dependency injection and has
/// no reference to the hosting application's service provider. The host (<c>Startup</c>) sets
/// <see cref="Repository"/> once at startup, if the admin panel (and therefore the audit
/// repository) is enabled; otherwise, audit writes are silently skipped.
/// </remarks>
public static class ChatCommandAuditLog
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets or sets the audit repository. Set once by the host at startup; <see langword="null"/>
    /// when no durable audit storage is configured (e.g. a game-server-only deployment).
    /// </summary>
    public static IAdminAuditRepository? Repository { get; set; }

    /// <summary>
    /// Records that a GameMaster executed a chat command, if an audit repository is configured.
    /// Never throws - a failure to write the audit entry must not prevent the command from working.
    /// </summary>
    /// <param name="player">The player (game master) who issued the command.</param>
    /// <param name="command">The command key, e.g. <c>/summon</c>.</param>
    /// <param name="minCharacterStatusRequirement">The minimum character status required to run the command.</param>
    /// <param name="arguments">The parsed arguments of the command, if any.</param>
    public static async ValueTask RecordAsync(Player player, string command, CharacterStatus minCharacterStatusRequirement, object? arguments)
    {
        if (Repository is not { } repository || minCharacterStatusRequirement < CharacterStatus.GameMaster)
        {
            return;
        }

        try
        {
            var entry = new AdminAuditEntry
            {
                Id = Guid.NewGuid(),
                StartedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                PrincipalId = player.Account?.LoginName ?? player.Name,
                PrincipalName = player.Name,
                Frontend = "In-game chat command",
                Operation = command,
                ArgumentsJson = JsonSerializer.Serialize(arguments, SerializerOptions),
                ResultJson = "{}",
                Outcome = "Executed",
            };

            await repository.AddAsync(entry).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Failed to write the audit entry for the chat command '{Command}'.", command);
        }
    }
}
