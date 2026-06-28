# Quality Gates

## Product

- Every narrated feature was verified in source or runtime.
- Known limitations are excluded or stated clearly.
- Download URLs and repository URLs are exact.
- Architecture guidance refers to the application bitness when required.

## Voice

- The voice source is authorized.
- No clipped peaks or distracting noise.
- Target integrated loudness is near `-16 LUFS` for spoken video.
- Pronunciation of product names, shortcuts, and URLs is acceptable.
- Subtitle timing follows speech and uses readable line lengths.
- A representative test chunk was reviewed before batch generation.
- Final pace was measured instead of accepting the TTS default blindly.
- Pauses longer than `0.6` seconds were reviewed, and inter-chunk gaps are deliberate.
- Any speed adjustment was applied with pitch preservation and reflected in subtitle timing.

## Horizontal Video

- `3840x2160` or the agreed master size.
- Constant `30 fps`.
- H.264 video and AAC audio.
- Real product capture remains legible at normal playback size.
- Opening, midpoint, CTA, and final frame were inspected.

## Vertical Video

- `1080x1920`.
- A full vertical episode and a separate 45 to 60 second teaser both exist.
- The full vertical episode covers every narration chunk and has approximately the same duration as the horizontal master.
- Text is readable on a phone.
- The active product area is not cropped away.
- The layout is purpose-built for 9:16.
- CTA remains visible for at least four seconds.
- Accurate frame extraction at multiple timestamps and `blackdetect` show no blank segment.

## Delivery

- Final MP4, SRT, publish copy, scripts, and source assets are organized by episode.
- Temporary cache and browser profile files are excluded from git.
- `ffprobe` output confirms duration, codecs, dimensions, and streams.
- Contact sheets show no overflow, blank frame, or accidental private content.
- The final report names the horizontal master, full vertical episode, and teaser separately.
