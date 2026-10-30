import os
from PIL import Image, ImageDraw, ImageFilter
import numpy as np

src_path = r"C:\Users\Admin\.gemini\antigravity-ide\brain\866fad8a-0184-4550-9549-d7378fc4a62a\dss_store_icon_1793342132285.jpg"
output_dir = r"g:\Decision System\dist\StoreAssets"
os.makedirs(output_dir, exist_ok=True)

img = Image.open(src_path).convert("RGB")
w, h = img.size

# The inner squircle is approximately at box (128, 128, 896, 896)
# Let's crop the squircle exactly:
crop_box = (124, 128, 896, 900)
squircle = img.crop(crop_box)
sq_w, sq_h = squircle.size

# Target 1: Full-bleed square with dark obsidian background (Microsoft Store preferred for StoreLogo)
# Expand squircle onto 1024x1024 dark background (matching the dark rim color #0d1117 / #0b0f19)
full_bleed = Image.new("RGBA", (1024, 1024), (11, 15, 23, 255))
resized_sq = squircle.resize((920, 920), Image.Resampling.LANCZOS)
full_bleed.paste(resized_sq, (52, 52))

# Save 1024x1024 Full Bleed
full_bleed.save(os.path.join(output_dir, "StoreLogo_1024x1024.png"), format="PNG")

# Target 2: Transparent Rounded Squircle
# We can create a rounded rectangle mask for transparent edges
mask = Image.new("L", (sq_w, sq_h), 0)
draw = ImageDraw.Draw(mask)
corner_radius = int(sq_w * 0.22) # Apple/Microsoft standard squircle curvature
draw.rounded_rectangle([(0, 0), (sq_w, sq_h)], radius=corner_radius, fill=255)

transparent_sq = Image.new("RGBA", (sq_w, sq_h), (0, 0, 0, 0))
transparent_sq.paste(squircle, (0, 0), mask=mask)

# Save standard Microsoft Store sizes
sizes = {
    "StoreLogo_512x512.png": (512, 512),
    "StoreLogo_300x300.png": (300, 300),
    "Square150x150Logo.png": (150, 150),
    "Square71x71Logo.png": (71, 71),
    "Square44x44Logo.png": (44, 44),
}

for filename, size in sizes.items():
    resized = transparent_sq.resize(size, Image.Resampling.LANCZOS)
    resized.save(os.path.join(output_dir, filename), format="PNG")

# Target 3: Windows ICO file for DSS_App.exe & Shortcuts
# Contains 256x256, 128x128, 64x64, 48x48, 32x32, 16x16
ico_sizes = [(256, 256), (128, 128), (64, 64), (48, 48), (32, 32), (16, 16)]
ico_images = [transparent_sq.resize(s, Image.Resampling.LANCZOS) for s in ico_sizes]
ico_path = os.path.join(output_dir, "app_icon.ico")
ico_images[0].save(ico_path, format="ICO", sizes=ico_sizes)

# Also copy icon to DSS_App and dist
dss_app_ico = r"g:\Decision System\DSS_App\app_icon.ico"
ico_images[0].save(dss_app_ico, format="ICO", sizes=ico_sizes)

print("Store Assets generated successfully in:", output_dir)
