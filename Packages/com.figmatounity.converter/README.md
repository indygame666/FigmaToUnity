# Figma To Unity Converter (UPM)

Пакет добавляет окно `Tools/Figma/Importer` в Unity Editor для потока `Preview -> Import -> Reimport`.

## Установка

### Локально (из папки)

В `Packages/manifest.json` Unity-проекта добавьте:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "file:../FigmaToUnity/Packages/com.figmatounity.converter"
  }
}
```

### Из Git-репозитория

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/<owner>/<repo>.git?path=/Packages/com.figmatounity.converter"
  }
}
```

## Проверка установки

1. Дождитесь завершения компиляции скриптов Unity.
2. Откройте меню `Tools/Figma/Importer`.
3. Убедитесь, что окно конвертера открывается без ошибок.

## Структура пакета

- `Editor/` - окно, превью и отчет импорта
- `Runtime/` - runtime-assembly (зарезервировано для будущих компонентов)

## Примечание

Текущая версия пакета содержит editor-пайплайн и заглушку интеграции backend. Реальный вызов backend можно подключить в `FigmaImporterService`.
