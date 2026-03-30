# FigmaToUnity - English Guide

`FigmaToUnity` is a Unity Package Manager (UPM) package that imports selected Figma nodes into Unity through the Editor window `Tools/Figma/Importer`.

## Repository

- Project: [https://github.com/indygame666/FigmaToUnity](https://github.com/indygame666/FigmaToUnity)
- Package path: `Packages/com.figmatounity.converter`

## Installation

Add the package dependency to `Packages/manifest.json` in your Unity project:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter"
  }
}
```

For reproducible builds, pin to a branch/tag/commit:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter#main"
  }
}
```

## Configuration and Usage

1. Open `Tools/Figma/Importer`.
2. Enter your `Figma Token`.
3. Provide either:
   - `File Key` and `Node IDs (csv)`, or
   - a full Figma URL and click `Parse URL`.
4. Click `Preview` to validate resolved nodes.
5. Click `Import` (or `Reimport`) to run the operation.

### Node ID Format

- Accepted format: `0:1,12:34`
- If your URL has `node-id=0-1`, convert it to `0:1`

## Figma API

The importer performs a real request to the Figma REST API:

- Endpoint: `GET https://api.figma.com/v1/files/{file_key}/nodes?ids={node_ids}`
- Header: `X-Figma-Token: <your-token>`

Official documentation:

- Figma API Overview: [https://www.figma.com/developers/api](https://www.figma.com/developers/api)
- Files Endpoint: [https://www.figma.com/developers/api#get-files-endpoint](https://www.figma.com/developers/api#get-files-endpoint)

## Notes

- Keep your Figma token private and do not commit it to source control.
- Prefer using pinned revisions in `manifest.json` for production pipelines.
