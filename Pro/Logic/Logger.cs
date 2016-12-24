using System;
using Dapper;

namespace Pro.Logic
{
    public class Logger
    {
        public static void Log(string logActionName, int? currentUserId = null)
        {
            if (currentUserId == null)
                currentUserId = Auth.CurrentUserId();
            int logActionId = SimpleConnector.Connection
                    .QueryFirstOrDefault<int>(
                    "SELECT id FROM LogActions WHERE Action = @Action",
                    new { Action = logActionName }
                    );
            SimpleConnector.Connection
                .Execute(
                "INSERT INTO Logs (Date, UserId, LogActionId) VAlUES (@Date, @UserId, @LogActionId)",
                new { Date = DateTime.Now, UserId = (int)currentUserId, LogActionId = logActionId }
                );
        }
    }
}