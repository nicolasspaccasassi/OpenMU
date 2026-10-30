// <copyright file="ChatCommandAccessConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Per-command access configuration for chat commands based on <see cref="ChatCommandPlugInBase{T}"/>,
/// editable in the admin panel's "Chat commands" page (gear icon).
/// </summary>
/// <remarks>
/// This only ever narrows access further, on top of the command's own
/// <see cref="IChatCommandPlugIn.MinCharacterStatusRequirement"/> - it never widens it. A command which
/// already requires <see cref="CharacterStatus.GameMaster"/> is unaffected by this configuration.
/// </remarks>
public class ChatCommandAccessConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether this command is disabled for players with
    /// <see cref="CharacterStatus.Normal"/>, while staying available for game masters.
    /// Use this to turn off a self-service command for everyone except staff, without having
    /// to deactivate it (and lose it for game masters too) with the regular Activate/Deactivate toggle.
    /// </summary>
    public bool DisabledForNormalPlayers { get; set; }
}
