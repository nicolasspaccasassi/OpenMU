// <copyright file="SummonChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="SummonChatCommand"/>.
/// </summary>
public class SummonChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the monster name or number.
    /// </summary>
    [Argument("monster")]
    public string? Monster { get; set; }

    /// <summary>
    /// Gets or sets the amount of monsters to summon.
    /// </summary>
    [Argument("quantity", false)]
    public short Quantity { get; set; } = 1;
}
