using System;
using System.Collections.Generic;
using System.IO;
using KeeAnywhere.StorageProviders;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    /// <summary>
    /// Lets users supply their own OAuth application credentials (client id / secret)
    /// per storage provider, instead of relying on the keys compiled into the plugin.
    ///
    /// Credentials are read from an optional JSON file in the settings directory
    /// (see <see cref="FileName"/>). When a provider has no user-supplied entry, the
    /// value compiled into the plugin is used as a fallback, so existing setups keep
    /// working unchanged.
    ///
    /// The file is plain text and hand-edited by the user; it holds the user's own
    /// development keys, never the plugin's production keys. Resolution is keyed by
    /// <see cref="StorageType"/> name, so the secret is never used as a lookup key.
    /// </summary>
    public static class AppCredentials
    {
        public const string FileName = "KeeAnywhere.appids.json";

        private static readonly Lazy<Dictionary<string, AppCredential>> Map =
            new Lazy<Dictionary<string, AppCredential>>(Load);

        public static string FilePath
        {
            get { return Path.Combine(ConfigurationInfo.SettingsDirectory, FileName); }
        }

        public static AppCredential Get(StorageType type)
        {
            AppCredential credential;
            return Map.Value.TryGetValue(type.ToString(), out credential) ? credential : null;
        }

        public static string ClientId(StorageType type, string fallback)
        {
            var credential = Get(type);
            return credential != null && !string.IsNullOrWhiteSpace(credential.ClientId)
                ? credential.ClientId
                : fallback;
        }

        public static string ClientSecret(StorageType type, string fallback)
        {
            var credential = Get(type);
            return credential != null && !string.IsNullOrWhiteSpace(credential.ClientSecret)
                ? credential.ClientSecret
                : fallback;
        }

        private static Dictionary<string, AppCredential> Load()
        {
            var empty = new Dictionary<string, AppCredential>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var path = FilePath;
                if (!File.Exists(path)) return empty;

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return empty;

                var map = JsonConvert.DeserializeObject<Dictionary<string, AppCredential>>(json);
                if (map == null) return empty;

                return new Dictionary<string, AppCredential>(map, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                // Malformed or unreadable file: fall back to compiled-in keys.
                return empty;
            }
        }
    }
}
