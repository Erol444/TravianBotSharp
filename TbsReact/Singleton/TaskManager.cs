using System.Collections.Generic;
using System.Linq;
using TbsReact.Models;

namespace TbsReact.Singleton
{
    public class TaskManager
    {
        private static readonly TaskManager instance = new();

        private TaskManager()
        {
        }

        public static TaskManager Instance
        {
            get
            {
                return instance;
            }
        }

        private static void UpdateTaskTable(string username)
        {
            if (!AccountManager.CheckGroup(username)) return;
            AccountManager.SendMessage(username, "task", "reset");
        }

        private List<Task> _getTaskList(string username)
        {
            var acc = AccountManager.Accounts.FirstOrDefault(x => x.AccInfo.Nickname.Equals(username));
            if (acc == null) return null;
            if (acc.Tasks == null) return null;
            List<Task> list = new();
            foreach (var task in acc.Tasks.ToList())
            {
                var tasKOjb = new Task
                {
                    Id = list.Count,
                    Name = task.ToString().Split('.').Last(),
                    VillName = task.Vill?.Name ?? "/",
                    Priority = task.Priority.ToString(),
                    Stage = task.Stage.ToString(),
                    ExecuteAt = task.ExecuteAt
                };
                list.Add(tasKOjb);
            }

            return list;
        }

        public static void AddAccount(TbsCore.Models.AccModels.Account account)
        {
            account.Tasks.OnUpdateTask = UpdateTaskTable;
            if (account.Tasks.Count < 1)
            {
                if (!AccountManager.CheckGroup(account.AccInfo.Nickname)) return;
                AccountManager.SendMessage(account.AccInfo.Nickname, "task", "waiting");
            }
        }

        public static List<Task> GetTaskList(string username)
        {
            return instance._getTaskList(username);
        }
    }
}