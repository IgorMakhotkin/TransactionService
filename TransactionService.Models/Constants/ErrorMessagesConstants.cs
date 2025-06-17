namespace TransactionService.Models.Constants
{
    public static class ErrorMessagesConstants
    {
        public const string ClientNotFound = "Не найден клиент с id = {0}";

        public const string BalanceNotFound = "Не найден баланс клиент с id = {0}";

        public const string TransactionAlreadyProcessed = "Транзакция уже выполнена";

        public const string TransactionNotFound = "Транзакция не найдена";

        public const string NoMoney = "Не достаточная сумма у клиента, баланс = {0}";
    }
}
