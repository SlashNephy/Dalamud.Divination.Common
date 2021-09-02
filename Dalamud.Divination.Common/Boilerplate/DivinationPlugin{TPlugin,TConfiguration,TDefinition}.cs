﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dalamud.Configuration;
using Dalamud.Divination.Common.Api;
using Dalamud.Divination.Common.Api.Dalamud;
using Dalamud.Divination.Common.Api.Definition;
using Dalamud.Divination.Common.Api.Logger;
using Dalamud.Plugin;
using Serilog.Core;

namespace Dalamud.Divination.Common.Boilerplate
{
    /// <summary>
    /// Divination プラグインのボイラープレートを提供します。
    /// このクラスを継承することで Dalamud 互換のプラグインを作成できます。
    /// </summary>
    /// <typeparam name="TPlugin">プラグインのクラス。</typeparam>
    /// <typeparam name="TConfiguration">Dalamud.Configuration.IPluginConfiguration を実装したプラグイン設定クラス。</typeparam>
    /// <typeparam name="TDefinition">プラグインの外部定義クラス。</typeparam>
    public abstract class DivinationPlugin<TPlugin, TConfiguration, TDefinition> : IDivinationPluginApi<TConfiguration, TDefinition>
        where TPlugin : DivinationPlugin<TPlugin, TConfiguration, TDefinition>
        where TConfiguration : class, IPluginConfiguration, new()
        where TDefinition : DefinitionContainer, new()
    {
        /// <summary>
        /// プラグインのインスタンスの静的プロパティ。
        /// </summary>
#pragma warning disable 8618
        public static TPlugin Instance { get; private set; }
#pragma warning restore 8618

        public string Name => Instance.GetType().Name;
        public bool IsDisposed { get; private set; }
        public Logger Logger { get; }
        public TConfiguration Config => Divination.Config.Config;
        public IDalamudApi Dalamud { get; }
        public IDivinationApi<TConfiguration, TDefinition> Divination { get; }
        public Assembly Assembly => Instance.GetType().Assembly;

        protected DivinationPlugin(DalamudPluginInterface pluginInterface)
        {
            if (this is not IDalamudPlugin)
            {
                throw new TypeAccessException("インタフェース: IDalamudPlugin を実装していません。");
            }

            Instance = this as TPlugin ?? throw new TypeAccessException("クラス インスタンスが型パラメータ: TPlugin と一致しません。");
            IsDisposed = false;
            Logger = DivinationLogger.File(Name);
            Dalamud = new DalamudApi(pluginInterface);
            Divination = new DivinationApi<TConfiguration, TDefinition>(Dalamud, Assembly, this);

            Divination.Chat.Print("プラグインを読み込みました！");
            Logger.Information("プラグイン: {Name} の初期化に成功しました。バージョン = {Version}", Name, Divination.Version.Plugin.InformationalVersion);
        }

        #region IDisposable

        /// <summary>
        /// Divination プラグイン内で確保されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void ReleaseManaged()
        {
        }

        protected virtual void ReleaseUnmanaged()
        {
        }

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;

            if (disposing)
            {
                ReleaseManaged();

                Divination.Chat.Print("プラグインを停止しました。");
                Logger.Dispose();
            }

            ReleaseUnmanaged();
        }

        ~DivinationPlugin()
        {
            Dispose(false);
        }

        #endregion
    }
}