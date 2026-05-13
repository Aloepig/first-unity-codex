from pathlib import Path
from PIL import Image, ImageOps, ImageChops


SOURCE = Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-7651696329180753647.png")
OUT_DIR = Path(r"C:\Users\pentasystem\My project\Assets\Art\GeneratedCharacters")

# Manually tuned crop boxes from the provided sketch (x1, y1, x2, y2)
CHARACTERS = {
    "bottom_staff": (290, 420, 560, 820),
    "bottom_big_ears": (540, 450, 790, 870),
    "top_horned": (190, 110, 470, 430),
    "top_crown": (460, 55, 760, 360),
    "top_spear": (380, 255, 705, 595),
    "top_small_pair": (355, 400, 560, 615),
}


def to_line_alpha(img: Image.Image) -> Image.Image:
    gray = ImageOps.grayscale(img)
    # Keep dark pencil lines, drop white paper background.
    alpha = gray.point(lambda p: 255 if p < 180 else 0)
    rgba = Image.new("RGBA", img.size, (0, 0, 0, 0))
    line = Image.new("RGBA", img.size, (45, 45, 45, 255))
    rgba.paste(line, mask=alpha)
    # Trim transparent margin
    bbox = rgba.getbbox()
    return rgba.crop(bbox) if bbox else rgba


def make_walk_frames(idle: Image.Image) -> tuple[Image.Image, Image.Image]:
    w, h = idle.size
    # Simple hand-drawn walk feel: slight offset + tiny shear-like crop/pad.
    walk1 = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    walk2 = Image.new("RGBA", (w, h), (0, 0, 0, 0))

    walk1.paste(idle, (3, 0))
    walk2.paste(idle, (-3, 2))

    # Light distortion by offsetting lower body strip
    lower = idle.crop((0, int(h * 0.55), w, h))
    walk1.paste(lower, (-2, int(h * 0.55)))
    walk2.paste(lower, (2, int(h * 0.55) + 1))

    return walk1, walk2


def save_set(name: str, sprite: Image.Image) -> None:
    char_dir = OUT_DIR / name
    char_dir.mkdir(parents=True, exist_ok=True)

    idle = to_line_alpha(sprite)
    walk1, walk2 = make_walk_frames(idle)

    idle.save(char_dir / "idle.png")
    walk1.save(char_dir / "walk_1.png")
    walk2.save(char_dir / "walk_2.png")


def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    src = Image.open(SOURCE).convert("RGB")

    for name, box in CHARACTERS.items():
        cropped = src.crop(box)
        save_set(name, cropped)


if __name__ == "__main__":
    main()
