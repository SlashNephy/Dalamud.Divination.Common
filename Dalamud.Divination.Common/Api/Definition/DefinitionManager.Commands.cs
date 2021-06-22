﻿using System.Collections.Generic;
using System.Linq;
using Dalamud.Divination.Common.Api.Chat;
using Dalamud.Divination.Common.Api.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace Dalamud.Divination.Common.Api.Definition
{
    public partial class DefinitionManager<TContainer>
    {
        public object GetCommandInstance()
        {
            return new Commands(this);
        }

        private class Commands
        {
            private readonly DefinitionManager<TContainer> manager;

            public Commands(DefinitionManager<TContainer> manager)
            {
                this.manager = manager;
            }

            [Command("Def Version", Help = "定義ファイルのバージョンを表示します。")]
            private void OnDefVersionCommand()
            {
                manager.chatClient.Print($"定義ファイル: ゲームバージョン = {manager.Provider.Container.Version}, パッチ = {manager.Provider.Container.Patch}");
            }

            [Command("Def Fetch", Help = "定義ファイルを更新します。")]
            private void OnDefFetchCommand()
            {
                manager.Provider.Update();
                manager.chatClient.Print($"定義ファイルを更新しました。ゲームバージョン = {manager.Provider.Container.Version}, パッチ = {manager.Provider.Container.Patch}");
            }

            [Command("Def Override", "key?", "value?", Help = "定義 <key> を <value?> に上書きします。<key> が null の場合, 利用可能な設定名の一覧を出力します。")]
            private void OnDefOverride(CommandContext context, string? key, string? value)
            {
                if (key == null)
                {
                    var defKeys = manager.EnumerateDefinitionsFields().Select(x => x.Name);

                    manager.chatClient.PrintError(new List<Payload>
                    {
                        new TextPayload("コマンドの構文が間違っています。"),
                        new TextPayload($"Usage: {context.Command.Usage}"),
                        new TextPayload("設定名は Definitions.cs で定義されているフィールド名です。大文字小文字を区別しません。"),
                        new TextPayload("設定値が空白の場合, null として設定します。"),
                        new TextPayload("利用可能な定義名の一覧:"),
                        new TextPayload(string.Join("\n", defKeys))
                    });
                    return;
                }

                manager.TryUpdate(key, value);
            }
        }
    }
}