# AdminWPF

Десктопная админ-панель для платформы посуточной аренды [SutkiVMV](https://sutkivmv.ru). Windows-клиент на WPF, работает с тем же REST API, что сайт и Android-приложение AdminMV.

## Возможности

- Вход по email и паролю (JWT)
- Выход с подтверждением, запрос смены пароля по почте
- Квартиры: список и карточка с вкладками (описание, удобства, правила, рядом)
- Бронирования: внутренние и внешние (iCal)
- Клиенты, вики-места
- Справочники: удобства, правила, места рядом, ценовые правила
- Настройки сайта
- В каждой таблице: поиск по всем полям, фильтр по колонке и значению, сортировка по заголовку, добавление, редактирование, удаление
- График по выбранной колонке (количество, сумма, среднее)
- Экспорт текущей выборки в `.docx`, `.pdf`, `.xlsx`
- Тёмная и светлая темы, выбор сохраняется

## Стек

- .NET 8, WPF
- `HttpClient` + `System.Net.Http.Json`
- ClosedXML (xlsx), DocumentFormat.OpenXml (docx), QuestPDF (pdf)

## Запуск

1. Установить [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) и Visual Studio 2022 с нагрузкой «Разработка классических приложений .NET».
2. Открыть решение, восстановить пакеты:
   ```powershell
   dotnet restore
   ```
3. При необходимости поменять адрес API в `Services/ApiService.cs`:
   ```csharp
   BaseAddress = new Uri("https://sutkivmv.ru")
   ```
4. Запустить проект (F5) и войти под учётной записью администратора.

## Структура проекта

```
AdminWPF
├─ App.xaml / App.xaml.cs        точка входа, загрузка темы, глобальный обработчик ошибок
├─ Shell.xaml                    окно с Frame для навигации
├─ MainPage.xaml                 главная страница с вкладками
├─ Models/
│  └─ Dto.cs                     DTO, совпадающие с серверными
├─ Services/
│  ├─ ApiService.cs              все HTTP-запросы, обработка ошибок
│  ├─ TokenStorage.cs            хранение токена (DPAPI)
│  ├─ ThemeManager.cs            переключение темы
│  └─ TitleBar.cs                цвет системного заголовка окна
├─ Shared/
│  ├─ CrudView.cs                универсальная таблица: поиск, фильтр, CRUD, график, экспорт
│  ├─ EditForm.cs                форма редактирования, поля строятся по свойствам DTO
│  ├─ MultiSelectTab.cs          мультивыбор из справочника
│  ├─ ChartWindow.cs             окно диаграммы
│  ├─ Exporter.cs                экспорт в docx, pdf, xlsx
│  └─ TableData.cs               данные таблицы для экспорта и графика
├─ Views/
│  ├─ Auth.xaml                  вход
│  ├─ ApartmentsListView.cs      список квартир и карточка
│  ├─ ApartmentDetailView.xaml   карточка квартиры с вкладками
│  └─ SettingsView.cs            настройки сайта
└─ Themes/
   ├─ DarkTheme.xaml             палитра тёмной темы
   ├─ LightTheme.xaml            палитра светлой темы
   └─ Styles.xaml                стили элементов управления
```

Для плоских сущностей (клиенты, брони, удобства, правила, места рядом, цены, вики) используется один компонент `CrudView<TRead, TCreate>`. Чтобы добавить новую сущность, достаточно DTO и одной строки в `MainPage`:

```csharp
tabs.Items.Add(new TabItem
{
    Header = "Клиенты",
    Content = new CrudView<ClientDto, CreateClientDto>("api/clients", "Клиенты")
});
```

## Используемые эндпоинты

| Раздел | Путь |
|---|---|
| Вход | `POST api/auth/login` |
| Смена пароля (письмо) | `POST api/auth/change-password-email` |
| Квартиры | `api/apartments/admin-summary`, `api/apartments/admin/{id}`, `api/apartments/{id}` |
| Удобства | `api/amenities` |
| Правила | `api/apartments/rules` |
| Места рядом | `api/apartments/nearby-places` |
| Ценовые правила | `api/admin/price-rules` |
| Брони | `api/bookings` |
| Внешние брони | `api/admin/external-bookings` |
| Клиенты | `api/clients` |
| Вики | `api/wiki-places` |
| Настройки сайта | `GET api/settings`, `PUT api/admin/settings` |

Изменение настроек сайта и часть админских операций требуют роль `Admin`.

## Где хранятся данные

| Что | Путь |
|---|---|
| Токен (зашифрован) | `%AppData%\AdminMV\token.bin` |
| Выбранная тема | `%AppData%\AdminMV\theme.txt` |

## Экспорт

- PDF: QuestPDF с Community-лицензией, включено `Settings.UseSystemFonts`, шрифт Arial
- В отчёт попадают только записи, оставшиеся после поиска и фильтра, в текущей сортировке

## Известные ограничения

- Привязка и отвязка удобств, правил и мест рядом в карточке квартиры работает через отдельные запросы `POST`/`DELETE` на сервере, поэтому сохранение этих вкладок требует доработки под разницу выбранных значений
- Для бронирований нет календаря-таймлайна, как в Android-приложении, только таблицы
- Редактирование брони отправляет полную строку, тогда как сервер может ожидать только смену статуса
- Системные `MessageBox` не подчиняются теме приложения
