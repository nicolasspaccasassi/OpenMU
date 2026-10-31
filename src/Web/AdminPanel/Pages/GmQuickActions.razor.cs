// <copyright file="GmQuickActions.razor.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// A small set of one-click GM actions on the live server: spawn a monster, give an item.
/// </summary>
/// <remarks>
/// Both buttons call the same <see cref="LiveGameOperations"/> used by the MCP tools
/// (<c>monsters_spawn</c>, <c>players_give_item</c>), so a standalone companion app or ChatGPT can
/// do the exact same thing this page does, with the same authorization and audit trail.
/// </remarks>
public partial class GmQuickActions : ComponentBase
{
    private int _serverId;
    private ushort _mapNumber;
    private string _monsterIdentifier = string.Empty;
    private byte _spawnX;
    private byte _spawnY;
    private int _quantity = 1;

    private string _itemCharacterName = string.Empty;
    private byte _itemGroup;
    private short _itemNumber;
    private byte _itemLevel;
    private bool _itemSkill;
    private bool _itemLuck;
    private byte _itemOptionLevel;
    private byte _itemExcellentOptions;

    private bool _isBusy;

    /// <summary>Gets or sets the shared live game operations.</summary>
    [Inject]
    public LiveGameOperations Operations { get; set; } = null!;

    /// <summary>Gets or sets the server registry, to list the hosted game servers.</summary>
    [Inject]
    public IServerProvider Servers { get; set; } = null!;

    /// <summary>Gets or sets the modal service.</summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>Gets or sets the toast service.</summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>Gets or sets the logger.</summary>
    [Inject]
    public ILogger<GmQuickActions> Logger { get; set; } = null!;

    private IReadOnlyList<IGameServer> GameServers => this.Servers.Servers.OfType<IGameServer>().OrderBy(s => s.Id).ToList();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        this._serverId = this.GameServers.FirstOrDefault()?.Id ?? 0;
    }

    private async Task OnSpawnMonsterClickAsync()
    {
        if (string.IsNullOrWhiteSpace(this._monsterIdentifier))
        {
            this.ToastService.ShowError("Enter a monster number or name.");
            return;
        }

        await this.RunAsync((apply, token) => this.Operations.SpawnMonsterAsync(
            this._serverId, this._mapNumber, this._monsterIdentifier, this._spawnX, this._spawnY, this._quantity, apply, token, default)).ConfigureAwait(true);
    }

    private async Task OnGiveItemClickAsync()
    {
        if (string.IsNullOrWhiteSpace(this._itemCharacterName))
        {
            this.ToastService.ShowError("Enter the exact online character name.");
            return;
        }

        var item = new ItemSpecification(this._itemGroup, this._itemNumber, this._itemLevel, this._itemSkill, this._itemLuck, this._itemOptionLevel, this._itemExcellentOptions);
        await this.RunAsync((apply, token) => this.Operations.GiveItemAsync(this._itemCharacterName, item, apply, token, default)).ConfigureAwait(true);
    }

    /// <summary>
    /// Previews the change, asks the GM to confirm it, then applies it with the preview's token -
    /// the same preview/apply/confirmationToken contract every admin write operation uses.
    /// </summary>
    private async Task RunAsync(Func<bool, string?, ValueTask<JsonObject>> change)
    {
        if (this._isBusy)
        {
            return;
        }

        this._isBusy = true;
        try
        {
            var preview = await change(false, null).ConfigureAwait(true);
            var isConfirmed = await this.ModalService.ShowQuestionAsync(
                    "GM Quick Actions",
                    "Apply this action now?")
                .ConfigureAwait(true);
            if (!isConfirmed)
            {
                return;
            }

            var token = preview["confirmationToken"]?.GetValue<string>();
            await change(true, token).ConfigureAwait(true);
            this.ToastService.ShowSuccess("Done.");
        }
        catch (AdminOperationException ex)
        {
            this.ToastService.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred on a GM quick action: {Message}", ex.Message);
            this.ToastService.ShowError(string.Create(CultureInfo.InvariantCulture, $"Unexpected error: {ex.Message}"));
        }
        finally
        {
            this._isBusy = false;
        }
    }
}
