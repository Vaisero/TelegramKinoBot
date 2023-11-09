# Добро пожаловать в Telegram-бота, который поможет вам быстро найти информацию об интересующем вас фильме!


Этот проект - [❤️🎬Кино Коды🎬❤️](https://t.me/kinokodi_bot), показывающий вам искомые фильмы. Если вы знаете номер интересующего вас фильма, то бот выдаст название и постер данного фильма, а так же ссылки на сервисы для удобного просмотра без рекламы и очень краткую информацию с сайта "Кинопоиск". Если же вы не знаете какой фильм посмотреть, бот может предложить случайный фильм из своей базы данных. 


## Как пользоваться:

0. При запуске Telegram-бота на своём сервере, требуется ввести в консоль специальный и уникальный [API Token](https://core.telegram.org/bots/api), а так же создать и подключить базу данных PostgreSQL, описание которой приведено ниже
1. Добавьте бота [❤️🎬Кино Коды🎬❤️](https://t.me/kinokodi_bot) в свой телеграмм-аккаунт 
2. Введите в сообщение <код фильма> для поиска
3. Бот выдаст название, постер фильма, ссылки для просмотра, краткую информацию с сайта "Кинопоиск"
4. Если вы не знаете какой фильм посмотреть, введите команду "/random" и бот предложит случайный фильм из своей базы данных
5. При вводе команды "/list" выведется общее количество фильмов, присутствующих в базе данных на данный момент
6. Если в сообщении отправить неверное число, или же любой другой текст, или же другой вид информации, то так же будет выдана соответствующая ошибка, заранее прописанная в обработчике исключений

### Список исключений, обрабатываемых ботом при неверно введённом сообщении:

* Фото
* Файл или документ
* Стикер
* Аудио
* Голосовые сообщения
* Видео
* Видео-кружки
* Gif анимация
* Опросы
* Геолокация
* Местоположение
* Контакты
* Сообщение, не являющееся числом
* <код фильма>, не существующий в базе данных

***На каждое из представленных исключений имеется свой ответ пользователю***
![1](https://user-images.githubusercontent.com/121144432/232340061-bbddbaf7-925c-40e4-9574-8e185481ae66.png)

Помимо этого, бот имеет секретную команду, неизвестную простым пользователям. При вводе команды "GETUSERSINFO", выводится вся база данных с ID пользователей, что уже воспользовались ботом. Данная команда необходима для отслеживания популярности Telegram-бота, а ID сохраняются, для отслеживания уникальных пользователей, а не просто сессий, что могло бы запутать статистику популярности. 
Бот способен мониторить переписку пользователя и показывать его ID, имя и текст сообщений в удобной консоли. Эта функциональность позволяет следить за общением пользователя и более точно понимать его потребности и запросы.

## Состав базы данных:

*kino.kino*
* id (bigint) - уникальный идентификатор для каждого фильма с помощью которого и производится поиск по базе данных 
* name (text) - название фильма
* image (text) - постер фильма, представленный в виде ссылки на изображение
* kino_link (text) - ссылка на ресурс [Кинопоиск](https://www.kinopoisk.ru/) для ознакомления с описанием фильма
* link1, link2, link3, link4 (text) - ссылки на сторонние ресурсы для просмотра фильма онлайн без рекламы

  `
    CREATE TABLE IF NOT EXISTS kino.kino(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name text COLLATE pg_catalog."default" NOT NULL,
    image text COLLATE pg_catalog."default" NOT NULL,
    kino_link text COLLATE pg_catalog."default" NOT NULL,
    link1 text COLLATE pg_catalog."default" default NULL,
    link2 text COLLATE pg_catalog."default" default NULL,
    link3 text COLLATE pg_catalog."default" default NULL,
    link4 text COLLATE pg_catalog."default" default NULL,
    CONSTRAINT kino_pkey PRIMARY KEY (id))
  `

*kino.users_info*
* user_id (bigint) - уникальный идентификатор для каждого пользователя
* user_name (text) - уникальный "ник" пользователя
* user_first_name (text) - имя пользователя
* user_second_name (text) - фамилия пользователя
* first_entry _date (timestamp) - дата первого сообщения пользователя боту
* last_entry _date (timestamp) - дата последнего сообщения пользователя боту

  `
    CREATE TABLE IF NOT EXISTS kino.users_info(
    user_id bigint NOT NULL,
    user_name text COLLATE pg_catalog."default",
    user_first_name text COLLATE pg_catalog."default" NOT NULL,
    user_second_name text COLLATE pg_catalog."default",
    first_entry_date timestamp with time zone NOT NULL,
    last_entry_date timestamp with time zone NOT NULL,
    CONSTRAINT users_info_pkey PRIMARY KEY (user_id))
  `

Основными преимуществами бота являются простота использования, высокая скорость работы, а также возможность автоматизировать некоторые рутинные задачи. Бот может быть использован как для персональных целей, так и для бизнес-нужд.

Бот создан на .NET Core 3.1 (C#) с использованием библиотеки [Telegram.Bots](https://telegrambots.github.io/book/) и имеет базу данных о фильмах, которая постоянно пополняется с помощью [TelegramKinoBot-AdminPanel](https://github.com/Vaisero/TelegramKinoBot-AdminPanel). При создании проекта использовалась база данных [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server), но по завершению разработки было решено перейти на [PostgreSQL](https://www.postgresql.org/).

## Задача проекта - помочь пользователям быстро найти фильмы, которые им понравятся, и сделать просмотр кино более удобным.
