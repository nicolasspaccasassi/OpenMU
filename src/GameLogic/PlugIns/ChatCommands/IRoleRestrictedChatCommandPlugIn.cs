// <copyright file="IRoleRestrictedChatCommandPlugIn.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Implemented by chat command plugins which support narrowing their access per role on top of their
/// own <see cref="IChatCommandPlugIn.MinCharacterStatusRequirement"/>, via <see cref="ChatCommandAccessConfiguration"/>.
/// </summary>
public interface IRoleRestrictedChatCommandPlugIn
{
    /// <summary>
    /// Gets a value indicating whether this command has been disabled for players with
    /// <see cref="CharacterStatus.Normal"/> through its <see cref="ChatCommandAccessConfiguration"/>.
    /// </summary>
    bool IsDisabledForNormalPlayers { get; }
}
