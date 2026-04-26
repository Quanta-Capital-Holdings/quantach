# Quanta Capital Holdings — Design System

## Company Overview

**Quanta Capital Holdings Inc.** is a Canadian investment firm and holding company focused on business acquisitions and M&A. Their mission is to drive growth and value creation by identifying high-potential companies with solid fundamentals, unique market positions, and untapped growth opportunities — then partnering with management teams to unlock full potential.

**Primary audience:** Business owners considering selling their businesses — the site must feel welcoming and trustworthy, not cold or transactional.

**Website:** https://quantach.com (currently minimal — two pages: Home and Contact Us)

**Sources used:** Live website crawl of quantach.com and quantach.com/contact-us

---

## CONTENT FUNDAMENTALS

### Tone & Voice
- **Professional yet warm** — speaks to business owners as partners, not targets
- **Confident, not arrogant** — uses phrases like "we partner with," "we collaborate closely," "let's discuss"
- **First-person plural (We/Our)** — positions the firm as a team, not an individual
- **Second-person for CTAs** — "If you're considering selling your business, we'd love to connect"
- **No jargon overload** — M&A terms are used but explained in accessible language
- **No emoji** — purely professional, text-driven communication

### Copy Style
- Sentence case for headings (not Title Case for body)
- Oxford comma used
- Long-form body paragraphs conveying expertise
- CTAs are invitational: "Reach out," "Let's discuss," "We'd love to connect"
- Numbers written out under ten; numerals for larger figures

### Casing
- Navigation: Title Case
- Headings: Sentence case or ALL CAPS for emphasis
- Buttons: Title Case

### Examples
- "Driving Growth by Mergers and Acquisitions"
- "Ready to explore new opportunities? If you're considering selling your business, we'd love to connect."
- "We partner with exceptional companies to unlock their full potential"

---

## VISUAL FOUNDATIONS

### Color System
- **Primary Navy** `#0B1F3A` — deep trust, authority, institutional weight
- **Gold Accent** `#C4922A` — value, wealth, premium quality
- **Off-white** `#F7F5F0` — warm, approachable background
- **Light gray** `#E8E4DC` — subtle separators, secondary backgrounds
- **Medium gray** `#8A8478` — secondary text, captions
- **Dark text** `#1A1612` — primary body text (warm black, not pure black)

### Typography
- **Display:** A geometric or transitional serif — editorial authority (Playfair Display or similar)
- **Body:** Clean humanist sans-serif — readable, approachable (DM Sans or similar)
- **Mono:** For numbers, data, figures (DM Mono or similar)
- Font substitutes sourced from Google Fonts (no proprietary font files provided)

### Backgrounds
- Primarily clean off-white (`#F7F5F0`) for warmth over cold white
- Navy sections for high-impact CTAs and footer
- No gradients — flat, sophisticated color blocks
- Subtle texture possible (fine grain or linen feel) for premium quality

### Spacing & Layout
- Generous white space — premium, unhurried feel
- Max content width: 1200px centered
- Sections: 80–120px vertical padding
- Cards: 24–32px inner padding, subtle border or shadow

### Cards
- Border: 1px solid `#E8E4DC`
- Shadow: `0 2px 12px rgba(11,31,58,0.06)`
- Corner radius: 4px (restrained, not bubbly)
- No colored left-border accent

### Animation
- Subtle fade-in on scroll — no bounces or slides
- Transitions: 200–300ms ease
- Hover states: slight opacity reduction or gold underline reveal for links
- No heavy motion — sophisticated restraint

### Imagery
- Warm color temperature — boardrooms, handshakes, aerial business parks
- Not stock-photo-cheesy — should feel editorial
- Full-bleed hero possible with dark overlay
- Photographs referenced but not stored (no provided assets)

### Borders & Radius
- `border-radius: 4px` for cards and inputs
- `border-radius: 2px` for tags/badges
- `border-radius: 50%` for avatar circles

### Icons
- Minimal usage — decorative only
- Thin-stroke line icons (Lucide style, 1.5px stroke weight)
- No icon fonts; SVG inline or CDN-linked
- No emoji

### Logo Direction
- Flat, geometric — 2–3 colors (Navy + Gold)
- "Q" monogram with abstract upward-trending element suggesting capital growth
- Clean, confident, scalable — works at favicon size and large format

---

## ICONOGRAPHY

Icons sourced from **Lucide Icons** (CDN: `https://unpkg.com/lucide@latest`) — thin-stroke, 24×24, 1.5px stroke weight. No proprietary icon set provided. Key icons used:

- `trending-up` — growth/returns
- `handshake` (custom inline SVG) — partnership
- `shield-check` — trust/due diligence
- `building-2` — business/company
- `users` — management team
- `chart-line` — performance
- `mail`, `phone` — contact

Assets stored in `assets/`.

---

## FILE INDEX

```
README.md                          ← You are here
SKILL.md                           ← Agent skill definition
colors_and_type.css                ← CSS variables: colors, type, spacing
assets/
  logo.svg                         ← Primary logo (navy + gold)
  logo-white.svg                   ← Reversed logo for dark backgrounds
  logo-mark.svg                    ← Q mark only (favicon / small use)
preview/
  colors-primary.html              ← Primary color swatches
  colors-semantic.html             ← Semantic/neutral color swatches
  type-display.html                ← Display type specimen
  type-body.html                   ← Body type specimen
  type-scale.html                  ← Full type scale
  spacing-tokens.html              ← Spacing tokens
  components-buttons.html          ← Button states
  components-inputs.html           ← Form inputs
  components-cards.html            ← Card components
  components-nav.html              ← Navigation bar
  logo-usage.html                  ← Logo on light/dark
ui_kits/
  website/
    README.md                      ← Website UI kit notes
    index.html                     ← Full website prototype
    Header.jsx                     ← Nav + hero
    Sections.jsx                   ← Homepage sections
    ContactForm.jsx                ← Contact form
    Footer.jsx                     ← Footer
```
