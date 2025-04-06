using DiscordRPC;
using StarZInjector.Classes;

namespace StarZFinance.Classes
{
    public static class DiscordRichPresenceManager
    {
        private static readonly string ClientId = "1358258166386790510";
        private static DiscordRpcClient? discordClient;

        public static DiscordRpcClient DiscordClient
        {
            get
            {
                discordClient ??= new DiscordRpcClient(ClientId);
                return discordClient;
            }
        }

        public static void SetPresence()
        {
            try
            {
                string state = ConfigManager.GetDiscordRPCStatus();
                DiscordClient.SetPresence(new RichPresence
                {
                    State = state,
                    Timestamps = Timestamps.Now,
                    Assets = new Assets
                    {
                        LargeImageKey = "starz",
                        LargeImageText = "StarZ Injector",
                        SmallImageKey = "dll"
                    }
                });
            }
            catch (Exception ex)
            {
                LogsManager.Log($"{ex.Message}", "DiscordClient.txt");
            }
        }

        public static void SetState(string state)
        {
            try
            {
                DiscordClient.UpdateState(state);
            }
            catch (Exception ex)
            {
                LogsManager.Log($"{ex.Message}", "DiscordClient.txt");
            }
        }

        public static void TerminatePresence()
        {
            if (DiscordClient.IsDisposed) return;
            try
            {
                DiscordClient.ClearPresence();
                DiscordClient.Deinitialize();
                DiscordClient.Dispose();
            }
            catch (Exception ex)
            {
                LogsManager.Log($"{ex.Message}", "DiscordClient.txt");
            }
        }
    }
}
