# Как пользоваться конвертером Figma -> Unity

Этот документ описывает базовый поток работы с конвертером в Unity Editor.

## 0. Как появляется меню `Tools/Figma/Importer`

Пункт меню появляется из атрибута `MenuItem` в скрипте пакета `Packages/com.figmatounity.converter/Editor/FigmaImporterWindow.cs`.

Чтобы он реально появился в Unity, код должен быть импортирован в ваш Unity-проект как Editor-скрипты:

### Установка как UPM-пакет (рекомендуется)

1. Откройте `Packages/manifest.json` вашего Unity-проекта.
2. Добавьте зависимость:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "file:../FigmaToUnity/Packages/com.figmatounity.converter"
  }
}
```

Если ставите из Git, используйте:

```json
{
  "dependencies": {
    "com.figmatounity.converter": "https://github.com/<owner>/<repo>.git?path=/Packages/com.figmatounity.converter"
  }
}
```

После импорта Unity перекомпилирует скрипты, и меню `Tools/Figma/Importer` станет доступно.

Если меню не появилось:

- Проверьте, что пакет `com.figmatounity.converter` виден в `Window -> Package Manager`.
- Проверьте, что файл действительно компилируется в Editor assembly (лежит в `Editor` папке пакета).
- Проверьте консоль Unity на compile errors (при любой ошибке меню может не зарегистрироваться).
- Убедитесь, что класс `FigmaImporterWindow` и метод с `[MenuItem("Tools/Figma/Importer")]` не исключены define-символами.

## 1. Подготовка

- Убедитесь, что Figma-макет использует поддерживаемые возможности из `docs/figma-supported-features.md`.
- Подготовьте `fileKey` Figma-документа и список `nodeId` нужных фреймов.
- Проверьте, что backend-пайплайн доступен (ingest -> normalize -> responsive -> export).

## 2. Открытие окна конвертера

1. Откройте Unity.
2. Перейдите в меню `Tools/Figma/Importer`.
3. Введите:
   - `File Key` — ключ Figma-файла
   - `Node IDs (csv)` — список `nodeId` через запятую

## 3. Режимы работы

### Preview

- Нажмите `Preview`, чтобы посмотреть план изменений без записи артефактов.
- Используйте этот режим перед каждым импортом для проверки операций `create/update`.

### Import

- Нажмите `Import`, чтобы создать UI-артефакты в Unity.
- Применяется для первого переноса экрана или новых узлов.

### Reimport

- Нажмите `Reimport`, чтобы обновить уже импортированные узлы.
- Режим опирается на стабильные `nodeId/componentId`, обеспечивая идемпотентное обновление.

## 4. Просмотр результатов

- В блоке `Preview / Diff` отображается список операций:
  - `CREATE` — будет создан новый объект
  - `UPDATE` — будет обновлен существующий объект
- В блоке `Import Report` показываются:
  - предупреждения (`warning`)
  - технические детали выполнения пайплайна

## 5. Обработка ошибок и предупреждений

- Если `File Key` пустой, конвертер покажет предупреждение и отчет.
- Неподдерживаемые элементы Figma попадают в отчет как `warning` или `error` (по правилам fallback).
- Для сложных несовпадений макета проверьте:
  - `docs/canonical-schema.md`
  - `docs/target-selection-scorecard.md`

## 6. Рекомендуемый рабочий цикл команды

1. Дизайнер обновляет макет в Figma.
2. Разработчик запускает `Preview`.
3. После проверки выполняет `Import` или `Reimport`.
4. Сравнивает результат с golden-экранами и прогоняет тесты.
5. Фиксирует изменения только после успешной валидации.

## 7. Проверка качества перед релизом

- Проверьте импорт 2-3 реальных экранов через `Preview -> Import -> Reimport`.
- Убедитесь, что повторный `Reimport` не создает дубликаты и обновляет существующие артефакты.
- Сверьтесь с KPI из `docs/quality-metrics.md`.
