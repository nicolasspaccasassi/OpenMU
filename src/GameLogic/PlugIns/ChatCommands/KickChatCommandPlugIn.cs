// <copyright file="KickChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles kick commands. It's an alias of <see cref="DisconnectChatCommandPlugIn"/>.
/// </summary>
[Guid("7EE329D5-C75F-4826-88A8-01A5B762DF4D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.KickChatCommandPlugIn_Name), Description = nameof(PlugInResources.KickChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(DisconnectChatCommandArgs), CharacterStatus.GameMaster)]
public class KickChatCommandPlugIn : ChatCommandPlugInBase<DisconnectChatCommandArgs>
{
    private const string Command = "/kick";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, DisconnectChatCommandArgs arguments)
    {
        var player = await this.GetPlayerByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);

        if (!player.Name.Equals(gameMaster.Name))
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandResultPlayerDisconnected), this.Key, player.Name).ConfigureAwait(false);
        }
    }
}
