﻿using Dalamud.Divination.Common.Api.Chat;
using Dalamud.Divination.Common.Api.Dalamud;
using Dalamud.Divination.Common.Api.Dalamud.Payload;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace Dalamud.Divination.Common.Api.Command
{
    internal sealed partial class CommandProcessor
    {
        public object GetCommandInstance()
        {
            return new DefaultCommands(this);
        }

        private class DefaultCommands
        {
            private readonly CommandProcessor processor;

            public DefaultCommands(CommandProcessor processor)
            {
                this.processor = processor;
            }

            [Command("Help", Help = "プラグインのヘルプを表示します。")]
            private void OnHelpCommand()
            {
                processor.chatClient.Print(payloads =>
                {
                    payloads.Add(new TextPayload($"{processor.pluginName} のコマンド一覧:\n"));

                    foreach (var command in processor.Commands)
                    {
                        payloads.AddRange(PayloadUtilities.HighlightAngleBrackets(command.Attribute.Usage));

                        if (!string.IsNullOrEmpty(command.Attribute.Help))
                        {
                            payloads.Add(new TextPayload($"\n {SeIconChar.ArrowRight.AsString()} "));
                            payloads.AddRange(PayloadUtilities.HighlightAngleBrackets(command.Attribute.Help));
                        }

                        payloads.Add(new TextPayload("\n"));
                    }
                });
            }
        }
    }
}