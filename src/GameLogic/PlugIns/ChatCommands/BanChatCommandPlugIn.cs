// <copyright file="BanChatCommandPlugIn.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles ban commands. It's an alias of <see cref="BanAccChatCommandPlugIn"/>.
/// </summary>
[Guid("9145B293-5A07-4BD3-AB7F-A7E6742A31F0")]
[PlugIn]
[Display(Name = nameof(PlugInResources.BanChatCommandPlugIn_Name), Description = nameof(PlugInResources.BanChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(BanAccChatCommandArgs), CharacterStatus.GameMaster)]
public class BanChatCommandPlugIn : ChatCommandPlugInBase<BanAccChatCommandArgs>
{
    private const string Command = "/ban";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, BanAccChatCommandArgs arguments)
    {
        await this.ChangeAccountStateByLoginNameAsync(gameMaster, arguments.AccountName ?? string.Empty, AccountState.Banned).ConfigureAwait(false);

        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AccountHasBeenBanned), this.Key, arguments.AccountName).ConfigureAwait(false);
    }
}
