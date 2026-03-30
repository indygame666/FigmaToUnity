# Figma To Unity Converter (UPM)

Пакет добавляет в Unity Editor окно `Tools/Figma/Importer` для импорта данных из Figma с последовательностью `Preview -> Import -> Reimport`.

## Installation

### Local Path

Добавьте зависимость в `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "file:../FigmaToUnity/Packages/com.figmatounity.converter"
  }
}
```

### Git Repository

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter"
  }
}
```

Рекомендуется использовать фиксированную ревизию:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter#main"
  }
}
```

## Usage

1. Откройте `Tools/Figma/Importer`.
2. Укажите `Figma Token`.
3. Заполните `File Key` и `Node IDs (csv)` или вставьте Figma URL и нажмите `Parse URL`.
4. Нажмите `Preview`, затем `Import`/`Reimport`.

Формат `Node IDs (csv)`: `0:1,12:34`.  
Для URL вида `node-id=0-1` используйте `0:1`.

## API Integration

Импортер выполняет реальный запрос к Figma REST API:

- `GET https://api.figma.com/v1/files/{file_key}/nodes?ids={node_ids}`
- Header: `X-Figma-Token: <your-token>`

## Package Structure

- `Editor/` - окно импорта, превью и отчет
- `Runtime/` - runtime-assembly для расширения функциональности

## References

- Repository: [github.com/indygame666/FigmaToUnity](https://github.com/indygame666/FigmaToUnity)
- Figma API docs: [figma.com/developers/api](https://www.figma.com/developers/api)
- Files API: [GET /v1/files/:key/nodes](https://www.figma.com/developers/api#get-files-endpoint)
