param(
  [string]$InputPath = "",
  [string]$OutputPath = "",
  [int]$RadiusPx = -1,
  [int]$MarginPx = 0,
  [string]$Background = "#FFFFFF"
)

if (-not (Test-Path $InputPath)) { Write-Error "No se encontró el archivo de entrada: $InputPath"; exit 1 }
if ([string]::IsNullOrWhiteSpace($OutputPath)) { Write-Error "Debes indicar OutputPath"; exit 1 }

Add-Type -AssemblyName System.Drawing

function New-RoundedRectPath([System.Drawing.RectangleF]$rect, [single]$radius)
{
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $diam = $radius * 2
    if ($diam -le 0) { $path.AddRectangle($rect); return $path }
    $arc = New-Object System.Drawing.RectangleF($rect.X, $rect.Y, $diam, $diam)
    $path.AddArc($arc, 180, 90)
    $arc.X = $rect.Right - $diam
    $path.AddArc($arc, 270, 90)
    $arc.Y = $rect.Bottom - $diam
    $path.AddArc($arc,   0, 90)
    $arc.X = $rect.X
    $path.AddArc($arc,  90, 90)
    $path.CloseFigure()
    return $path
}

# Cargar imagen original
$img = [System.Drawing.Image]::FromFile($InputPath)
$w = [int]$img.Width
$h = [int]$img.Height

# Calcular radio por defecto estilo iOS si no se pasa
if ($RadiusPx -lt 0) {
  $RadiusPx = [int]([Math]::Round([Math]::Min($w, $h) * 0.1875))
}

# Crear lienzo con alpha
$bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
$g.Clear([System.Drawing.Color]::Transparent)

# Fondo blanco redondeado
$rect = New-Object System.Drawing.RectangleF(0, 0, [single]$w, [single]$h)
$path = New-RoundedRectPath -rect $rect -radius $RadiusPx
$bgColor = [System.Drawing.ColorTranslator]::FromHtml($Background)
$brush = New-Object System.Drawing.SolidBrush($bgColor)
$g.FillPath($brush, $path)
$brush.Dispose()

# Clip redondeado y dibujo del PNG dentro (con margen opcional)
$g.SetClip($path)
$m = [int]$MarginPx
$dw = [int]($w - (2 * $m))
$dh = [int]($h - (2 * $m))
if ($dw -lt 1) { $dw = $w }
if ($dh -lt 1) { $dh = $h }
$dx = [int]$m
$dy = [int]$m
$destRect = New-Object System.Drawing.Rectangle($dx, $dy, $dw, $dh)
$g.DrawImage([System.Drawing.Image]$img, [System.Drawing.Rectangle]$destRect)

# Guardar
$bmp.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Liberar recursos
$g.Dispose(); $bmp.Dispose(); $img.Dispose()

Write-Host "Generado: $OutputPath (radio=$RadiusPx, margen=$MarginPx)"