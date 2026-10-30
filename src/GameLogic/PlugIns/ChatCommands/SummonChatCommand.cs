// <copyright file="SummonChatCommand.cs" company="MuPacca">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command to summon one or more monsters (identified by name or number)
/// at the position of the game master who issued the command.
/// </summary>
[Guid("0E3CDC72-EDD5-4FED-85B8-3C165B31C902")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SummonChatCommand_Name), Description = nameof(PlugInResources.SummonChatCommand_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(SummonChatCommandArgs), CharacterStatus.GameMaster)]
internal class SummonChatCommand : ChatCommandPlugInBase<SummonChatCommandArgs>
{
    private const string Command = "/summon";

    private const short MaxQuantity = 50;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, SummonChatCommandArgs arguments)
    {
        var identifier = arguments.Monster ?? string.Empty;
        var monsterDef = short.TryParse(identifier, out var number)
            ? gameMaster.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == number)
            : gameMaster.GameContext.Configuration.Monsters.FirstOrDefault(m =>
                m.Designation.GetTranslationAsSpan(gameMaster.Culture).Equals(identifier, StringComparison.OrdinalIgnoreCase)
                || m.Designation.ValueInNeutralLanguageAsSpan.Equals(identifier, StringComparison.OrdinalIgnoreCase));

        if (monsterDef is null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MonsterNotFound), identifier).ConfigureAwait(false);
            return;
        }

        var gameMap = gameMaster.CurrentMap;
        if (gameMap is null)
        {
            return;
        }

        var quantity = Math.Clamp(arguments.Quantity, (short)1, MaxQuantity);
        var area = new MonsterSpawnArea
        {
            GameMap = gameMap.Definition,
            MonsterDefinition = monsterDef,
            SpawnTrigger = SpawnTrigger.OnceAtEventStart,
            Quantity = 1,
            X1 = (byte)Math.Max(gameMaster.Position.X - 3, byte.MinValue),
            X2 = (byte)Math.Min(gameMaster.Position.X + 3, byte.MaxValue),
            Y1 = (byte)Math.Max(gameMaster.Position.Y - 3, byte.MinValue),
            Y2 = (byte)Math.Min(gameMaster.Position.Y + 3, byte.MaxValue),
        };

        for (var i = 0; i < quantity; i++)
        {
            INpcIntelligence intelligence = new BasicMonsterIntelligence();
            var monster = new Monster(area, monsterDef, gameMap, gameMaster.GameContext.DropGenerator, intelligence, gameMaster.GameContext.PlugInManager, gameMaster.GameContext.PathFinderPool);
            intelligence.Npc = monster;

            monster.Initialize();
            await gameMap.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();
        }

        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MonstersSummonedByGameMaster), quantity, identifier).ConfigureAwait(false);
    }
}
