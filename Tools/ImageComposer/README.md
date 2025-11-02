# ImageComposer

Minimal scripts to keep for generating iOS-style rounded icons from PNGs.

## Keep
- `round-corners.ps1` (essential)
  - Draws a white rounded tile and places your PNG inside, centered.
  - Params: `-InputPath`, `-OutputPath`, `-MarginPx` (default 0), `-RadiusPx`.
- `resize-image.ps1` (useful)
  - Resizes a source PNG to a standard canvas (256/512/etc) with high quality.
  - Params: `-InputPath`, `-OutputPath`, `-Width`, `-Height` (defaults to width),
    `-FlattenWhite` (optional), `-NoAlpha` (optional, strips transparency to reduce size).

## Optional
- `info.ps1` (handy)
  - Prints pixel dimensions of a PNG for quick checks.

## Safe to delete (examples)
- `draw-avocado.ps1` (example generator, not required for pipeline)
- `compose.ps1` (not used in current workflow)

## Typical workflow

1) Resize your base icon if needed (optional):

```powershell
# 256x256, keep transparency
powershell -ExecutionPolicy Bypass -File .\resize-image.ps1 \ 
  -InputPath "..\..\Recetas.Infrastructure\Data\Images\Tomate.png" \ 
  -OutputPath "..\..\Recetas.Infrastructure\Data\Images\Tomate_256.png" \ 
  -Width 256 -Height 256
```

2) Generate iOS-style rounded version:

```powershell
# 256x256, margin 8px, corner radius 32px (12.5% of 256)
powershell -ExecutionPolicy Bypass -File .\round-corners.ps1 \ 
  -InputPath "..\..\Recetas.Infrastructure\Data\Images\Tomate_256.png" \ 
  -OutputPath "..\..\Recetas.Infrastructure\Data\Images\Tomate_iOS_256_m8_r32.png" \ 
  -MarginPx 8 -RadiusPx 32
```

Notes:
- For 512x512 use radius 64px to keep the same proportion (12.5%).
- If your source has no transparency and you need a white tile anyway, you can skip `-FlattenWhite` — the rounder already creates a white rounded background.
- If you need smaller file sizes, resize to 256 or use `-FlattenWhite -NoAlpha` in `resize-image.ps1`.
