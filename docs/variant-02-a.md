# Картка варіанта 2-A

## Предметна область

Навчальний трекер кібербезпекових інцидентів. Reporter створює повідомлення,
Analyst класифікує його та змінює стан, Administrator виконує лише окремо
визначені адміністративні операції.

## Дані

- основний owned resource — `Incident`;
- пов'язані сутності — `IncidentComment`, `IncidentStatusHistory`;
- власник — `Incident.OwnerUserId`;
- текстові поля — `Title`, `Description`, `IncidentComment.Text`;
- workflow — `New → Triaged → InProgress → Resolved → Closed`;
- навчальні користувачі — Alice, Bob, Morgan (Analyst), Admin;
- відтворювані UUID Alice/Bob та їхніх інцидентів визначені у `DbSeeder`.

## Профіль A «Пошук»

| ЛР | Точка розвитку |
|---:|---|
| 1 | Підсумок за severity; готовий фільтр за status є близьким прикладом |
| 2 | Пошук у title/description, параметризація значень і allowlist сортування |
| 3 | Сервер призначає власника інциденту з principal |
| 4 | Горизонтальний IDOR читання/зміни чужого інциденту; вертикальна зміна severity/status |
| 5 | Stored XSS у description/comment; безпечний list/details render |
| 6 | Rate limit створення/пошуку, аудит залежностей і мінімізація журналів |

Навмисно вразливі стани для ЛР 2, 4 і 5 не входять до baseline. Студент створює
й виправляє потрібний контрольований стан під час відповідної роботи у власному
проєкті та лише в дозволеному локальному стенді.
