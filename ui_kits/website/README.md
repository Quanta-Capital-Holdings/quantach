# Quanta Capital Holdings — Website UI Kit

## Overview
High-fidelity recreation of the Quanta Capital Holdings website with all sections required for a new, modern launch.

## Design Width
1440px desktop / responsive mobile

## Pages / Screens
1. **Homepage** — Hero, About, Pillars, Stats, Seller CTA, Testimonial, Contact teaser, Footer
2. **Contact Page** — Full contact form with required fields

## Components
- `Header.jsx` — Sticky nav with logo + links + CTA button
- `Sections.jsx` — Hero, About, Pillars, Stats, Seller CTA, Testimonial sections
- `ContactForm.jsx` — Full-page contact form
- `Footer.jsx` — Dark footer with nav + legal

## Notes
- This is a visual prototype — form submission is simulated
- Stack recommendation for production: .NET 10 / Razor Pages on Azure Static Web Apps or GitHub Pages (static export) with a serverless Azure Function for form handling
- Font: Playfair Display + DM Sans via Google Fonts CDN
