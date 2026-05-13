from pathlib import Path
from PIL import Image, ImageOps
from collections import deque


PYTHON_OUT = Path(r"C:\Users\pentasystem\My project\Assets\Art\GeneratedCharacters\single_sources")

SOURCES = {
    "bottom_staff": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-11671052062713522040.png"),
    "top_crown_horned": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-1829692815566105513.png"),
    "bottom_big_ears": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-800610817190002372.png"),
    "top_horned": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-293666604200475193.png"),
}


def to_line_alpha(img: Image.Image) -> Image.Image:
    gray = ImageOps.grayscale(img)
    # Preferred pencil tone mapping.
    base_alpha = gray.point(lambda p: max(0, min(255, (235 - p) * 4)))

    # Remove only border-connected paper background.
    w, h = gray.size
    px = gray.load()
    bg = Image.new("L", (w, h), 0)
    bg_px = bg.load()
    q = deque()
    white_threshold = 214

    for x in range(w):
        if px[x, 0] >= white_threshold:
            q.append((x, 0))
        if px[x, h - 1] >= white_threshold:
            q.append((x, h - 1))
    for y in range(h):
        if px[0, y] >= white_threshold:
            q.append((0, y))
        if px[w - 1, y] >= white_threshold:
            q.append((w - 1, y))

    while q:
        x, y = q.popleft()
        if x < 0 or y < 0 or x >= w or y >= h:
            continue
        if bg_px[x, y] == 255:
            continue
        if px[x, y] < white_threshold:
            continue
        bg_px[x, y] = 255
        q.append((x + 1, y))
        q.append((x - 1, y))
        q.append((x, y + 1))
        q.append((x, y - 1))

    alpha = Image.new("L", (w, h), 0)
    a_px = alpha.load()
    b_px = base_alpha.load()
    for y in range(h):
        for x in range(w):
            a_px[x, y] = 0 if bg_px[x, y] == 255 else b_px[x, y]

    rgba = Image.new("RGBA", img.size, (0, 0, 0, 0))
    line = Image.merge("RGBA", (gray, gray, gray, alpha))
    rgba.alpha_composite(line)
    bbox = rgba.getbbox()
    return rgba.crop(bbox) if bbox else rgba


def make_walk_frames(idle: Image.Image) -> tuple[Image.Image, Image.Image]:
    w, h = idle.size
    walk1 = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    walk2 = Image.new("RGBA", (w, h), (0, 0, 0, 0))

    walk1.paste(idle, (2, 0))
    walk2.paste(idle, (-2, 1))

    lower = idle.crop((0, int(h * 0.58), w, h))
    walk1.paste(lower, (-2, int(h * 0.58)))
    walk2.paste(lower, (2, int(h * 0.58)))
    return walk1, walk2


def main() -> None:
    PYTHON_OUT.mkdir(parents=True, exist_ok=True)
    for name, src_path in SOURCES.items():
        src = Image.open(src_path).convert("RGB")
        idle = to_line_alpha(src)
        walk1, walk2 = make_walk_frames(idle)

        out_dir = PYTHON_OUT / name
        out_dir.mkdir(parents=True, exist_ok=True)
        idle.save(out_dir / "idle.png")
        walk1.save(out_dir / "walk_1.png")
        walk2.save(out_dir / "walk_2.png")


if __name__ == "__main__":
    main()
