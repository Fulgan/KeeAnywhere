# Using your own OAuth app credentials

KeeAnywhere talks to cloud providers (OneDrive, Google Drive, Dropbox, Box, HiDrive)
through OAuth. Each provider requires a registered *application* that owns a client id
(and, for most, a client secret).

By default the plugin uses the application registrations compiled into it. If you would
rather use your **own** app registrations — for example to control your own quota, branding,
or because the built-in apps are unavailable — you can supply your own credentials without
recompiling the plugin.

## How it works

On startup the plugin looks for a file named **`KeeAnywhere.appids.json`** in its settings
directory:

- **Portable install:** the same folder as `KeePass.exe`.
- **Installed (per-user):** the KeePass application-data directory
  (e.g. `%APPDATA%\KeePass\`).

For every provider listed in that file, the plugin uses your client id / secret instead of
the built-in ones. Any provider **not** listed keeps using the built-in defaults, so partial
configuration is fine — list only what you want to override.

If the file is missing or malformed, the plugin silently falls back to the built-in keys.

## Setup

1. Copy [`KeeAnywhere.appids.sample.json`](KeeAnywhere.appids.sample.json) to
   `KeeAnywhere.appids.json` in the settings directory above.
2. Keep only the providers you use; delete the rest (and the `_comment` line).
3. Register an application with each provider and paste the values in.

```json
{
  "OneDrive":    { "ClientId": "..." },
  "GoogleDrive": { "ClientId": "...", "ClientSecret": "..." },
  "Dropbox":     { "ClientId": "...", "ClientSecret": "..." },
  "Box":         { "ClientId": "...", "ClientSecret": "..." },
  "HiDrive":     { "ClientId": "...", "ClientSecret": "..." }
}
```

The provider keys are the storage-type names. The "restricted" variants
(`GoogleDriveRestricted`, `DropboxRestricted`) correspond to the limited-scope account types
KeeAnywhere offers; each falls back to its built-in key independently, so set them only if you
use them.

## Where to register an app per provider

| Provider | Developer console | Notes |
|----------|-------------------|-------|
| OneDrive | https://docs.microsoft.com/graph/auth-register-app-v2 | Public client / "Mobile and desktop applications". No secret — `ClientId` only. Scope `Files.ReadWrite` + `offline_access`. |
| Google Drive | https://console.developers.google.com/ | Create an **OAuth client of type "Desktop app"**. Enable the Drive API. `ClientId` + `ClientSecret`. |
| Dropbox | https://www.dropbox.com/developers/apps | App key = `ClientId`, app secret = `ClientSecret`. Use the *Full Dropbox* app for `Dropbox`, an *App folder* app for `DropboxRestricted`. |
| Box | https://developer.box.com/ | Standard OAuth 2.0 app. `ClientId` + `ClientSecret`. |
| HiDrive | https://dev.strato.com/hidrive/get_key | Set **Project App Type = "Native"**. `ClientId` + `ClientSecret`. |

### Redirect URI

KeeAnywhere completes OAuth on a local loopback address (`http://localhost` on a temporary
port). Register your application as a **native / desktop / public client** so loopback
redirects are accepted. Providers that offer a dedicated "Desktop app" / "Native" client type
(Google, HiDrive) handle this automatically; for others add `http://localhost` to the app's
allowed redirect URIs if prompted.

## Security note

`KeeAnywhere.appids.json` is **plain text**. It holds *your own* application credentials, not
your cloud account passwords or tokens — those remain stored securely by KeePass as before.
Still, treat the file like any other secret: keep it out of source control (it is already in
`.gitignore`) and off shared machines. A future plugin version may add an in-app, encrypted
way to enter these.
