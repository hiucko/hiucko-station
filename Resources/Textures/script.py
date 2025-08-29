import os
from PIL import Image

# Path to the replacement PNG
REPLACEMENT_PATH = "pozitiv.png"

def process_png(path, replacement_img):
    try:
        with Image.open(path) as img:
            w, h = img.size

            # Only proceed if both dimensions divisible by 32
            if w % 32 != 0 or h % 32 != 0:
                return

            # Determine tiling factors
            x_tiles = w // 32
            y_tiles = h // 32

            # Create new blank image with same size and mode
            new_img = Image.new("RGBA", (w, h))

            # Tile replacement image across
            for y in range(y_tiles):
                for x in range(x_tiles):
                    new_img.paste(replacement_img, (x * 32, y * 32))

            # Save over original
            new_img.save(path)
            print(f"Replaced: {path}")

    except Exception as e:
        print(f"Error processing {path}: {e}")

def main():
    replacement_img = Image.open(REPLACEMENT_PATH).convert("RGBA")

    if replacement_img.size != (32, 32):
        raise ValueError("Replacement image must be 32x32 pixels.")

    start_dir = os.getcwd()
    for root, _, files in os.walk(start_dir):
        for f in files:
            if f.lower().endswith(".png"):
                full_path = os.path.join(root, f)
                process_png(full_path, replacement_img)

if __name__ == "__main__":
    main()
