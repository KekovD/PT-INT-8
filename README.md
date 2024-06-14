#  INT-8-Лавренов Дмитрий

### Первое приложение - Initiator

#### Переменные в docker-compose.yml для изменения параметров запуска:
 - NUMBER_OF_LAUNCHES - сколько асинхронных расчетов начать (опционально)
 - START_PREVIOUS - начальное N(0) (опционально)
 - START_CURRENT - начальное N(1) (опционально)
 - MESSAGE_TTL - TTL сообщений в очереди RabbitMQ (работает только в рантайм) (опционально)
 - QUEUE_NAME - имя очереди в RabbitMQ, на которую подписывается приложение
 - CALCULATOR_URL - url контроллера второго приложения
 - RabbitMQ__ConnectionString - строка подключения к RabbitMQ
 - ASPNETCORE_ENVIRONMENT - среду выполнения приложения

### Второе приложение - Calculator

#### Переменные в docker-compose.yml для изменения параметров запуска:
 - RabbitMQ__ConnectionString - строка подключения к RabbitMQ
 - ASPNETCORE_ENVIRONMENT - среду выполнения приложения