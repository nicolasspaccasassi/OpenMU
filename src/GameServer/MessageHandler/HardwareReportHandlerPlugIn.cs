// <copyright file="HardwareReportHandlerPlugIn.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the custom hardware-report packet, which a MuPacca client sends once right
/// before its login request, carrying a hash of a hardware fingerprint and an integrity-flags byte.
/// </summary>
/// <remarks>
/// This isn't part of the original MU Online protocol or the generated <c>MUnique.OpenMU.Network.Packets</c>
/// structures - it's hand-parsed here so the client doesn't need a matching NuGet package release. The
/// head code <see cref="Code"/> (0xF4) is unused by the real protocol, see
/// <c>ClientToServerPackets.xml</c>. The packet is: [0xC1][length=20][0xF4][16-byte hash][1 flags byte].
/// A client which never sends this (e.g. an older build) just never sets
/// <see cref="Player.PendingHardwareIdHash"/>, and the hardware-ban check in <c>LoginAction</c> then
/// lets the login through unaffected.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.HardwareReportHandlerPlugIn_Name), Description = nameof(PlugInResources.HardwareReportHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C9D3F1A2-6B4E-4F8A-9C2D-1E7A3B5F8D40")]
internal class HardwareReportHandlerPlugIn : IPacketHandlerPlugIn
{
    private const byte Code = 0xF4;
    private const int HashLength = 16;
    private const int ExpectedPacketLength = 3 + HashLength + 1;

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => Code;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < ExpectedPacketLength)
        {
            return ValueTask.CompletedTask;
        }

        var span = packet.Span;
        var hash = span.Slice(3, HashLength);
        player.PendingHardwareIdHash = Convert.ToHexString(hash);

        // The integrity-flags byte (span[3 + HashLength]) is reserved for future use (e.g. a
        // debugger-detected or known-cheat-module-loaded signal from the client) - not acted
        // upon yet, but already parsed so adding a reaction later doesn't need a protocol change.
        return ValueTask.CompletedTask;
    }
}
