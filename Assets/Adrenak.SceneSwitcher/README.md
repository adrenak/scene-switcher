# Adrenak.SceneSwitcher

Quick scene switching for the Unity Editor. Configure a scene list once, then open it from the Scene View with a modifier + right-click chord.

## Features

- Scene View context menu with your configured scenes
- Configurable trigger: **Ctrl+Alt+Right Click** or **Ctrl+Shift+Right Click**
- Wizard at **Tools → Scene Switcher** with drag-and-drop scene setup
- Active scene marked with ✓ in the menu
- Save / Don't Save / Cancel dialog before switching scenes
- Play Mode guard

## Requirements

- Unity **2020.1** or newer

## Installation

Add a dependency to your project's `Packages/manifest.json`.

### Install by git tag (recommended)

Version tags match `package.json` (for example `1.0.0`).

```json
"com.adrenak.scene-switcher": "https://github.com/adrenak/scene-switcher.git#1.0.0"
```

Replace `1.0.0` with the version you want. See [Releases](https://github.com/adrenak/scene-switcher/releases) for available tags.

### Install from the `upm` branch

The `upm` branch contains only the package contents and is updated automatically by CI:

```json
"com.adrenak.scene-switcher": "https://github.com/adrenak/scene-switcher.git#upm"
```

### Local development

Point to a checkout on disk with the `file:` prefix (path relative to your Unity project root):

```json
"com.adrenak.scene-switcher": "file:../../SceneSwitcher/Assets/Adrenak.SceneSwitcher"
```

Adjust the relative path for your folder layout.

## Usage

1. Open **Tools → Scene Switcher**
2. Drag scene assets into the info box at the top
3. Choose your Scene View trigger chord
4. In the Scene View, use the trigger chord to open the scene menu
5. Pick a scene to open it, or choose **Settings** to reopen the wizard

Settings are stored at `Assets/Editor/SceneSwitcher/SceneSwitcherSettings.asset` in your project (auto-created on first use).

## Changing versions

To upgrade or downgrade, edit the tag in `Packages/manifest.json`:

```json
"com.adrenak.scene-switcher": "https://github.com/adrenak/scene-switcher.git#1.0.0"
```

## License

MIT
