"""
Generates the authored underwater sprites used by the game:
  shark.png     - the predator (gray shark, faces right)
  rock.png      - decorative boulder cluster for the sea floor
  seagrass.png  - decorative swaying grass blades

Run from anywhere:  python LostInTheDepths/Tools/gen_art.py

Sprites are drawn at 4x and downsampled with LANCZOS for clean edges, then
written into Assets/Resources/Art so GameArt.Load() can pick them up. This
folder lives outside Assets/ so Unity does not try to import the script.
"""

import math
import os

from PIL import Image, ImageDraw

SS = 4  # supersampling factor
ART_DIR = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "Art")


def canvas(w, h):
    return Image.new("RGBA", (w * SS, h * SS), (0, 0, 0, 0))


def finish(img, w, h, name):
    img = img.resize((w, h), Image.LANCZOS)
    path = os.path.normpath(os.path.join(ART_DIR, name))
    img.save(path)
    print("wrote", path, img.size)


def s(*vals):
    """Scale point/lengths into supersampled space."""
    return tuple(v * SS for v in vals)


# ---------------------------------------------------------------------------
# Shark - faces right (matches the fish convention; Predator.cs flips for dir)
# ---------------------------------------------------------------------------
def shark():
    W, H = 112, 64
    img = canvas(W, H)
    d = ImageDraw.Draw(img)

    body = (108, 122, 134, 255)      # slate gray
    body_dark = (72, 86, 99, 255)    # back shading / outline
    belly = (206, 214, 220, 255)     # pale underside
    fin = (88, 102, 114, 255)
    cy = H * 0.52

    # Tail (crescent) on the left.
    d.polygon([s(2, cy - 16), s(26, cy - 4), s(26, cy + 4), s(2, cy + 18)], fill=fin)
    d.polygon([s(8, cy - 2), s(30, cy - 6), s(30, cy + 6), s(8, cy + 2)], fill=body_dark)

    # Main body: blunt back tapering to a pointed snout at the right.
    d.polygon(
        [
            s(26, cy - 4),    # tail join top
            s(46, cy - 15),   # back hump
            s(72, cy - 13),
            s(98, cy - 6),    # snout top
            s(108, cy + 1),   # snout tip
            s(96, cy + 9),
            s(70, cy + 14),
            s(44, cy + 14),
            s(26, cy + 4),    # tail join bottom
        ],
        fill=body,
    )

    # Pale belly wedge along the lower body.
    d.polygon(
        [s(40, cy + 13), s(70, cy + 14), s(96, cy + 8), s(104, cy + 2),
         s(72, cy + 7), s(44, cy + 8)],
        fill=belly,
    )

    # Dorsal fin (top) and pectoral fin (bottom).
    d.polygon([s(50, cy - 14), s(66, cy - 32), s(70, cy - 13)], fill=fin)
    d.polygon([s(66, cy + 12), s(78, cy + 26), s(84, cy + 11)], fill=fin)

    # Gills - three short dark strokes behind the head.
    for i in range(3):
        gx = 80 + i * 5
        d.line([s(gx, cy - 8), s(gx - 3, cy + 6)], fill=body_dark, width=SS * 2)

    # Mouth and eye near the snout.
    d.line([s(90, cy + 6), s(106, cy + 2)], fill=(40, 48, 56, 255), width=SS * 2)
    d.ellipse([s(92, cy - 6), s(98, cy)], fill=(28, 32, 38, 255))

    finish(img, W, H, "shark.png")


# ---------------------------------------------------------------------------
# Rock - a small cluster of rounded boulders, lit from the upper-left.
# ---------------------------------------------------------------------------
def rock():
    W, H = 72, 48
    img = canvas(W, H)
    d = ImageDraw.Draw(img)

    base = (92, 96, 104, 255)
    shade = (60, 64, 72, 255)
    light = (132, 136, 144, 255)

    def boulder(cx, cy, rx, ry):
        d.ellipse([s(cx - rx, cy - ry), s(cx + rx, cy + ry)], fill=base)
        # darker lower half
        d.chord([s(cx - rx, cy - ry), s(cx + rx, cy + ry)], 0, 180, fill=shade)
        # upper-left highlight
        d.ellipse([s(cx - rx * 0.7, cy - ry * 0.8),
                   s(cx - rx * 0.1, cy - ry * 0.2)], fill=light)

    boulder(22, 34, 20, 14)   # left
    boulder(50, 36, 22, 16)   # right
    boulder(37, 26, 16, 13)   # top centre

    finish(img, W, H, "rock.png")


# ---------------------------------------------------------------------------
# Seagrass - a tuft of curved blades rising from the floor.
# ---------------------------------------------------------------------------
def seagrass():
    W, H = 56, 80
    img = canvas(W, H)
    d = ImageDraw.Draw(img)

    greens = [(46, 132, 78, 255), (62, 158, 96, 255), (38, 112, 70, 255),
              (74, 174, 108, 255), (52, 142, 84, 255)]
    base_y = H - 2
    blades = [(14, 0.55, 64, 1), (22, 0.0, 78, -1), (30, -0.5, 70, 1),
              (38, 0.4, 60, -1), (44, -0.3, 50, 1)]

    for i, (bx, lean, length, curl) in enumerate(blades):
        pts_l, pts_r = [], []
        steps = 10
        for k in range(steps + 1):
            t = k / steps
            y = base_y - t * length
            sway = curl * math.sin(t * 1.6) * 10 + lean * t * 14
            x = bx + sway
            w = max(0.6, 3.2 * (1 - t))  # taper to the tip
            pts_l.append(s(x - w, y))
            pts_r.append(s(x + w, y))
        d.polygon(pts_l + pts_r[::-1], fill=greens[i % len(greens)])

    finish(img, W, H, "seagrass.png")


if __name__ == "__main__":
    os.makedirs(ART_DIR, exist_ok=True)
    shark()
    rock()
    seagrass()
