import os
import shutil
import subprocess
from PIL import Image

layout_dir = r"g:\Decision System\dist\MsixLayout"
assets_dir = os.path.join(layout_dir, "Assets")
os.makedirs(assets_dir, exist_ok=True)

# 1. Copy application files from SingleFile publish
single_file_exe = r"g:\Decision System\dist\SingleFile\DSS_App.exe"
shutil.copy2(single_file_exe, os.path.join(layout_dir, "DSS_App.exe"))

# Copy splash_bg
app_assets = os.path.join(layout_dir, "Assets")
splash_src = r"g:\Decision System\dist\SingleFile\Assets\splash_bg.jpg"
if os.path.exists(splash_src):
    shutil.copy2(splash_src, os.path.join(app_assets, "splash_bg.jpg"))

# 2. Generate all required MSIX Visual Assets from our 1024x1024 store logo
src_logo = r"g:\Decision System\dist\StoreAssets\StoreLogo_1024x1024.png"
logo_img = Image.open(src_logo).convert("RGBA")

# StoreLogo (50x50)
logo_img.resize((50, 50), Image.Resampling.LANCZOS).save(os.path.join(assets_dir, "StoreLogo.png"))

# Square150x150Logo
logo_img.resize((150, 150), Image.Resampling.LANCZOS).save(os.path.join(assets_dir, "Square150x150Logo.png"))

# Square44x44Logo
logo_img.resize((44, 44), Image.Resampling.LANCZOS).save(os.path.join(assets_dir, "Square44x44Logo.png"))

# Square71x71Logo
logo_img.resize((71, 71), Image.Resampling.LANCZOS).save(os.path.join(assets_dir, "Square71x71Logo.png"))

# Square310x310Logo
logo_img.resize((310, 310), Image.Resampling.LANCZOS).save(os.path.join(assets_dir, "Square310x310Logo.png"))

# Wide310x150Logo: 310x150 with background #0b0f19
wide_tile = Image.new("RGBA", (310, 150), (11, 15, 25, 255))
icon_130 = logo_img.resize((130, 130), Image.Resampling.LANCZOS)
wide_tile.paste(icon_130, (90, 10), mask=icon_130)
wide_tile.save(os.path.join(assets_dir, "Wide310x150Logo.png"))

# 3. Create AppxManifest.xml with exact Store identity
manifest_content = """<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities">

  <Identity
    Name="VibioTechnology.DSSRetailStudio2026"
    Publisher="CN=DD86DAD5-0FBA-48BC-80E9-EC74B158F343"
    Version="1.0.0.0"
    ProcessorArchitecture="x64" />

  <Properties>
    <DisplayName>DSS Retail Studio 2026</DisplayName>
    <PublisherDisplayName>Vibio Technology</PublisherDisplayName>
    <Logo>Assets\\StoreLogo.png</Logo>
  </Properties>

  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.22621.0" />
  </Dependencies>

  <Resources>
    <Resource Language="en-US" />
    <Resource Language="vi-VN" />
  </Resources>

  <Applications>
    <Application
      Id="App"
      Executable="DSS_App.exe"
      EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
        DisplayName="DSS Retail Studio 2026"
        Description="Hệ thống hỗ trợ ra quyết định bán hàng đa mục tiêu theo mô hình Saaty AHP &amp; Knapsack Solver"
        BackgroundColor="#0b0f19"
        Square150x150Logo="Assets\\Square150x150Logo.png"
        Square44x44Logo="Assets\\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Assets\\Wide310x150Logo.png" Square310x310Logo="Assets\\Square310x310Logo.png" Square71x71Logo="Assets\\Square71x71Logo.png">
          <uap:ShowNameOnTiles>
            <uap:ShowOn Tile="square150x150Logo" />
            <uap:ShowOn Tile="wide310x150Logo" />
          </uap:ShowNameOnTiles>
        </uap:DefaultTile>
      </uap:VisualElements>
    </Application>
  </Applications>

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>
</Package>
"""

manifest_path = os.path.join(layout_dir, "AppxManifest.xml")
with open(manifest_path, "w", encoding="utf-8") as f:
    f.write(manifest_content.strip())

print("MSIX Layout prepared at:", layout_dir)

# 4. Pack into MSIX using makeappx.exe
output_msix = r"g:\Decision System\dist\DSS_Retail_Studio_2026.msix"
makeappx = r"g:\tools\makeappx.exe"

cmd = [makeappx, "pack", "/d", layout_dir, "/p", output_msix, "/o"]
print("Running makeappx pack...")
res = subprocess.run(cmd, capture_output=True, text=True)
print("Return code:", res.returncode)
print("STDOUT:\n", res.stdout)
print("STDERR:\n", res.stderr)
