# FigmaToUnity

Официальный репозиторий UPM-пакета для импорта UI-структур из Figma в Unity.

## Package Location

- `Packages/com.figmatounity.converter`

## Установка через Git (Unity Package Manager)

Добавьте зависимость в `Packages/manifest.json` вашего Unity-проекта:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter"
  }
}
```

Рекомендуется фиксировать версию через ветку, тег или commit hash:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/indygame666/FigmaToUnity.git?path=/Packages/com.figmatounity.converter#main"
  }
}
```

## Быстрый старт

1. Дождитесь завершения импорта пакета и компиляции скриптов.
2. Откройте `Tools/Figma/Importer`.
3. Введите `Figma Token`, `File Key` и `Node IDs (csv)` или вставьте полную Figma-ссылку и нажмите `Parse URL`.
4. Нажмите `Preview`, затем `Import` или `Reimport`.

## Полезные ссылки

- Репозиторий: [github.com/indygame666/FigmaToUnity](https://github.com/indygame666/FigmaToUnity)
- Пакет: [Packages/com.figmatounity.converter](https://github.com/indygame666/FigmaToUnity/tree/main/Packages/com.figmatounity.converter)
- Figma REST API (официальная документация): [developers.figma.com](https://www.figma.com/developers/api)
- Endpoint `GET /v1/files/:key/nodes`: [Files API](https://www.figma.com/developers/api#get-files-endpoint)

## English Documentation

English guide is available here: [`docs/README.en.md`](docs/README.en.md).
