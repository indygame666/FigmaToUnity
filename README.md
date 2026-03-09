# FigmaToUnity

Репозиторий содержит UPM-пакет конвертера UI из Figma в Unity.

## Где находится пакет

- `Packages/com.figmatounity.converter`

## Подключение пакета в Unity через Git

В `Packages/manifest.json` вашего Unity-проекта:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/<owner>/<repo>.git?path=/Packages/com.figmatounity.converter"
  }
}
```

После установки откройте `Tools/Figma/Importer`.

## Быстрый чек перед публикацией репозитория

1. Убедитесь, что в репозитории нет `node_modules` и временных папок Unity.
2. Проверьте корректность `Packages/com.figmatounity.converter/package.json`.
3. Обновите `CHANGELOG.md` при изменениях версии.
4. Запушьте в удаленный Git-репозиторий.
