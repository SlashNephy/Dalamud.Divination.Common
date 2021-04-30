﻿namespace Dalamud.Divination.Common.Api.Command
{
    /// <summary>
    /// 呼び出されたコマンドのコンテキストを持つクラスです。このクラスは継承できません。
    /// </summary>
    public sealed class CommandContext
    {
        /// <summary>
        /// コマンドのコンテキストを初期化します。
        /// </summary>
        /// <param name="attribute">コマンドの属性。</param>
        /// <param name="arguments">コマンドに与えられた引数の配列。</param>
        internal CommandContext(CommandAttribute attribute, string[] arguments)
        {
            Command = attribute;
            Arguments = arguments;
        }

        /// <summary>
        /// 呼び出されたコマンドの属性。
        /// </summary>
        public CommandAttribute Command { get; }

        /// <summary>
        /// コマンドに与えられた引数の配列。
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// コマンドに与えられた引数を結合した文字列。
        /// </summary>
        public string ArgumentText => string.Join(" ", Arguments);
    }
}