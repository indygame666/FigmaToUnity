# Схема Canonical Scene Graph

Конвертер использует промежуточное представление (`CanonicalSceneGraph`) между ingest из Figma и экспортерами Unity. Схема версионируется и поддерживает обратную совместимость на уровне документа.

## Версионирование

- `schemaVersion`: семантическая версия (текущая `1.0.0`)
- Ломающие изменения требуют migration-адаптеров
- Перед генерацией экспортеры обязаны валидировать схему

## Структура верхнего уровня

```json
{
  "schemaVersion": "1.0.0",
  "documentId": "figma-file-key",
  "rootNodeId": "node-root",
  "nodes": [],
  "styles": [],
  "assets": []
}
```

## Модель узла (основные поля)

- `id`: стабильный идентификатор узла
- `name`: имя узла в исходном Figma-документе
- `type`: `FRAME | TEXT | IMAGE | COMPONENT | INSTANCE`
- `parentId`: ссылка на родителя, допускает `null`
- `children`: массив идентификаторов дочерних узлов
- `layout`: метаданные layout (направление auto layout, интервалы, отступы)
- `constraints`: горизонтальное/вертикальное поведение
- `styleRefs`: ссылки на связанные стили

## Модель адаптивных правил

- `widthBehavior`: `fixed | fill | hug`
- `heightBehavior`: `fixed | fill | hug`
- `anchors`: `min | max | stretch | center`
- `minWidth/maxWidth/minHeight/maxHeight`: опциональные ограничения

## Модель ассетов

- `assetId`: стабильный hash или ссылка на изображение из Figma
- `kind`: `image | font`
- `sourceUri`: исходный URL или ключ в реестре
- `targetPath`: путь назначения в проекте Unity

## Гарантии валидации

- Целостность дерева (`parentId` и `children` согласованы)
- Отсутствие дублирующихся `nodeId`
- `rootNodeId` существует в коллекции узлов
- Все обязательные поля по типам узлов присутствуют
