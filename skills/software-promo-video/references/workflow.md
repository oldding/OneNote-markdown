# Production Workflow

## 1. Product Audit

Inspect:

- README and help documents.
- Current source paths implementing the advertised commands.
- CI, release workflow, installers, and architecture targets.
- GitHub release assets when network access is allowed.
- Existing user-facing limitations.

Create a truth table before narration:

| Claim | Evidence | Recording plan | Wording |
| --- | --- | --- | --- |
| Full-page render | Running product | Before/action/after | Direct |
| Live mode | Source plus runtime test | Show timing | Qualify |
| Diagram support | Source only | Do not demo | Exclude |

## 2. Series Shape

Use three to five episodes for a medium-sized desktop plugin:

1. Overview, installation, and value.
2. Core syntax or main workflow.
3. Speed features, shortcuts, and live behavior.
4. Import, export, settings, troubleshooting, and contribution.

Create one 45 to 60 second teaser from the strongest visual proof.

## 3. Real Demonstration

Create a disposable demo page. Keep the product window maximized and remove unrelated apps from view.

Capture:

- Clean starting state.
- Pointer or focus on the command.
- Visible result.
- Constraint or setting when it prevents user mistakes.

Prefer a complete action over a montage of disconnected screenshots.

## 4. Narration

Start with the user's preferred identity line. Use short sentences and one claim per shot.

Write numbers, keyboard shortcuts, product names, and URLs in a way the TTS engine pronounces reliably. Split narration into 10 to 25 second chunks.

Before generating the full episode:

1. Generate one representative chunk.
2. Measure its duration and estimate the complete runtime.
3. Listen for long pauses and slow sentence endings.
4. Compare the result with the intended promotion pace.

For Chinese software promotion, use roughly 210 to 280 spoken Chinese characters per minute as an initial review range, then defer to the presenter's preferred style. VoxCPM speed `1.0` is not automatically the right final pace. A pitch-preserving `atempo` adjustment around `1.08` to `1.25` can be appropriate after preview; this pipeline needed `1.25` for the supplied voice.

Keep inter-chunk gaps concise, commonly `0.15` to `0.30` seconds. Detect pauses longer than `0.6` seconds before final assembly. When audio is time-compressed, divide each subtitle timestamp by the same pace factor and rebuild cumulative offsets.

## 5. Editing

Recommended horizontal structure:

- Hook and identity.
- User problem.
- Real product proof.
- Feature map.
- Compatibility warning.
- Download and next episode.

Use motion cards for orientation, not as substitutes for real proof. Keep CTA links on screen long enough to read.

## 6. Platform Adaptation

Create two distinct vertical outputs:

- A full episode containing the complete narration and feature sequence.
- A 45 to 60 second teaser containing the strongest proof and CTA.

For both vertical outputs, redesign the hierarchy:

- Large title in the upper third.
- One product detail at a time.
- A cropped product close-up that preserves the active area.
- Large CTA in the final six seconds.

Do not simply scale the entire 16:9 frame into a narrow center strip.

For a still-image vertical timeline, prefer looped image inputs followed by `fps`, `scale`, `setsar=1`, and filter-based concatenation. Static-image concat-demuxer durations can produce sparse or blank frames in some FFmpeg builds.
