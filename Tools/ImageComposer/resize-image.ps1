param(
    [Parameter(Mandatory=$true)][string]$InputPath,
    [Parameter(Mandatory=$true)][string]$OutputPath,
    [int]$Width = 256,
    [int]$Height,
    [switch]$FlattenWhite,
    [switch]$NoAlpha
)

if (-not $Height) { $Height = $Width }

Add-Type -AssemblyName System.Drawing

if (-not (Test-Path $InputPath)) {
    throw "Input file not found: $InputPath"
}

$src = [System.Drawing.Image]::FromFile($InputPath)
try {
    # Siempre usamos 32bpp ARGB para componer y, si se solicita, convertimos a 24bpp RGB al final
    $bmp = New-Object System.Drawing.Bitmap $Width, $Height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

    # Calcular escala para mantener aspecto dentro del lienzo destino
    $scaleX = $Width / $src.Width
    $scaleY = $Height / $src.Height
    $scale = [Math]::Min($scaleX, $scaleY)

    $drawW = [int]([Math]::Round($src.Width * $scale))
    $drawH = [int]([Math]::Round($src.Height * $scale))

    $offsetX = [int](($Width - $drawW) / 2)
    $offsetY = [int](($Height - $drawH) / 2)

    if ($FlattenWhite) { $g.Clear([System.Drawing.Color]::White) } else { $g.Clear([System.Drawing.Color]::Transparent) }
    $destRect = New-Object System.Drawing.Rectangle $offsetX, $offsetY, $drawW, $drawH
    $g.DrawImage($src, $destRect, 0, 0, $src.Width, $src.Height, [System.Drawing.GraphicsUnit]::Pixel)

    $dir = [System.IO.Path]::GetDirectoryName($OutputPath)
    if (-not [string]::IsNullOrEmpty($dir) -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }

    if ($NoAlpha) {
        # Convertir a 24bpp (sin canal alfa) para reducir tamaño
        $rect = New-Object System.Drawing.Rectangle 0,0,$Width,$Height
        $bmp24 = $bmp.Clone($rect, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        try {
            $bmp24.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            if ($bmp24) { $bmp24.Dispose() }
        }
    }
    else {
        $bmp.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    Write-Output ("Redimensionado: {0} -> {1} ({2}x{3})" -f $InputPath, $OutputPath, $Width, $Height)
}
finally {
    if ($g) { $g.Dispose() }
    if ($bmp) { $bmp.Dispose() }
    $src.Dispose()
}
