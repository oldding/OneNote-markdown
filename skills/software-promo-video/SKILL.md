---
name: software-promo-video
description: Produce a truthful multi-episode software promotion video series from a local project, including product audit, real app demonstrations, authorized voice cloning through local VoxCPM, 4K horizontal and 9:16 vertical edits, captions, publishing copy, and final media QA. Use when the user asks to introduce, demo, market, or create social videos for a completed software project.
---

# Software Promo Video

Create promotion videos from verified product behavior, not only from README claims.

## Required Inputs

Collect or infer:

- Project root and repository URL.
- Download or product page URL.
- Target platforms.
- Presenter identity and preferred opening line.
- Voice source and explicit authorization to use it.
- Apps available for real demonstrations.
- Any previous series or visual style reference.

If a missing answer would materially change the result, ask once. Otherwise choose a conservative default and proceed.

## Workflow

1. Audit the repository, README, releases, installer artifacts, screenshots, and current git state.
2. Compare documentation claims with source code and the running product.
3. Record a truth boundary: implemented, partially implemented, timing-sensitive, or unsupported.
4. Choose the series length based on feature density. Prefer one overview plus focused workflow episodes.
5. Create a dedicated demo document or workspace so recording never exposes private content.
6. Capture real before, action, and after states in the product.
7. Write narration only after the behavior is verified.
8. Generate voice in short chunks using only an authorized voice.
9. Measure the synthesized narration pace before editing. Adjust speech speed and inter-chunk gaps when the preview feels slow.
10. Build the horizontal master first, then make both a purpose-built full vertical episode and a separate vertical teaser.
11. Add captions, publishing copy, links, and a pinned comment.
12. Run the quality gates in [quality-gates.md](references/quality-gates.md).

Read [workflow.md](references/workflow.md) for detailed production decisions.

## Default Deliverables

- `series-plan.md`
- One narration file and chunk file per episode.
- One shot list per episode.
- Real product captures.
- `3840x2160`, 30 fps horizontal MP4.
- `1080x1920`, 30 fps full vertical episode.
- Separate `1080x1920`, 30 fps, 45 to 60 second vertical teaser.
- UTF-8 SRT captions.
- Platform-specific publishing copy.
- Reproducible rendering scripts.
- Media probe report and visual contact sheets.

## Non-Negotiable Rules

- Never advertise a feature based only on stale documentation.
- Clearly state compatibility constraints such as application bitness.
- Do not imply that a render replaces source content if the product appends output.
- Do not hide timing-sensitive behavior behind edited cuts that imply instant response.
- Do not clone or synthesize a person's voice without explicit authorization.
- Keep real product footage central. Motion cards support the demonstration; they do not replace it.
- Use a separate vertical composition. A tiny horizontal video centered on a blurred background is only an emergency draft.
- Never label a teaser as the completed vertical episode. A full vertical deliverable must cover the complete narration and feature sequence.
- Do not accept the TTS engine's default pace without listening or measuring. Re-time captions whenever speech speed changes.

## VoxCPM

Use the bundled `scripts/voxcpm-generate.mjs` with a local VoxCPM Gradio server.

```powershell
node scripts/voxcpm-generate.mjs `
  --input voice-chunks.txt `
  --output-dir voice `
  --from 01
```

Set `VOXCPM_URL`, `VOXCPM_PRESET`, and `VOXCPM_PROMPT` when the defaults do not match the current project.

Generate one test chunk first. Check identity, pronunciation, pauses, duration, loudness, and subtitle timing before generating the full episode. If default-speed speech is slow, regenerate faster or apply pitch-preserving time compression; do not wait until final export to evaluate pace.

## Completion

Do not mark the project complete until:

- The final files exist.
- Duration, dimensions, frame rate, codecs, and audio streams are verified.
- Representative frames from beginning, middle, end, and vertical output were visually inspected.
- The full vertical duration matches the horizontal narration duration within normal muxing tolerance.
- The vertical teaser exists as a separate file and is not substituted for the full vertical episode.
- Links and product claims match the current repository and running software.
