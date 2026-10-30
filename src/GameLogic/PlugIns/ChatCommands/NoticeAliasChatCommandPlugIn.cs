// <copyright file="NoticeAliasChatCommandPlugIn.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles notice commands. It's an alias of <see cref="NoticeChatCommandPlugIn"/>.
/// </summary>
[Guid("33C1F9D2-D630-4C04-BAF2-5E010847C9FE")]
[PlugIn]
[Display(Name = nameof(PlugInResources.NoticeAliasChatCommandPlugIn_Name), Description = nameof(PlugInResources.NoticeAliasChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(CommandKey, CharacterStatus.GameMaster)]
public class NoticeAliasChatCommandPlugIn : IChatCommandPlugIn
{
    private const string CommandKey = "/notice";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var regex = new Regex(Regex.Escape(CommandKey));
        var message = regex.Replace(command, string.Empty, 1)?.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        await player.GameContext.SendGlobalMessageAsync(message, Interfaces.MessageType.GoldenCenter).ConfigureAwait(false);
        await ChatCommandAuditLog.RecordAsync(player, this.Key, this.MinCharacterStatusRequirement, new { message }).ConfigureAwait(false);
    }
}
