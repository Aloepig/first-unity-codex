from pathlib import Path
from PIL import Image, ImageOps
from collections import deque


OUT_DIR = Path(r"C:\Users\pentasystem\My project\Assets\Art\GeneratedCharacters\groups")

SOURCES = {
    "group_1": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-1339080250093019782.png"),
    "group_2": Path(r"C:\Users\PENTAS~1\AppData\Local\Temp\ai-chat-attachment-12113054643649263251.png"),
}


def to_line_alpha(img: Image.Image) -> Image.Image:
    gray = ImageOps.grayscale(img)
    # Base pencil tone mapping (the previous, preferred look).
    base_alpha = gray.point(lambda p: max(0, min(255, (235 - p) * 4)))

    # Background cutout mask:
    # flood-fill near-white pixels connected to the image border only.
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

    # Final alpha = preferred tone, but force detected outer background to transparent.
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

    walk1.paste(idle, (3, 0))
    walk2.paste(idle, (-3, 1))

    lower = idle.crop((0, int(h * 0.6), w, h))
    walk1.paste(lower, (-2, int(h * 0.6)))
    walk2.paste(lower, (2, int(h * 0.6)))
    return walk1, walk2


def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)

    for name, src_path in SOURCES.items():
        src = Image.open(src_path).convert("RGB")
        idle = to_line_alpha(src)
        walk1, walk2 = make_walk_frames(idle)

        out = OUT_DIR / name
        out.mkdir(parents=True, exist_ok=True)
        idle.save(out / "idle.png")
        walk1.save(out / "walk_1.png")
        walk2.save(out / "walk_2.png")


if __name__ == "__main__":
    main()
