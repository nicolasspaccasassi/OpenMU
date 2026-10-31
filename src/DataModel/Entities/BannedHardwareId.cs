// <copyright file="BannedHardwareId.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A hardware fingerprint which is blocked from logging in, independent of any account.
/// </summary>
/// <remarks>
/// Recorded automatically whenever an account is banned or temporarily banned and has a
/// <see cref="Account.LastKnownHardwareId"/>, so a banned player can't just register a new,
/// free account on the same machine. Not part of the <see cref="Account"/> aggregate, since a
/// hardware ban outlives any single account.
/// </remarks>
[AggregateRoot]
public class BannedHardwareId
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the hash of the hardware fingerprint which is banned.
    /// </summary>
    public string HardwareIdHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the point in time at which the hardware id was banned.
    /// </summary>
    public DateTime BannedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets an optional reason, e.g. the login name of the account whose ban triggered this.
    /// </summary>
    public string? Reason { get; set; }
}
