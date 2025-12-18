using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubnauticaClone
{
    public class TaskManager : SingletonBase<TaskManager>
    {
        public List<GameTask> allTasks = new List<GameTask>();
        
        private HashSet<string> completedTaskIDs = new HashSet<string>();
        private GameTask currentActiveTask;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (allTasks.Count > 0)
            {
                NotificationManager.Instance.ShowNotification($"Mission Started: {allTasks[0].title}");
            }
        }

        public void CompleteTask(string taskID)
        {
            if (completedTaskIDs.Contains(taskID)) return;

            GameTask task = allTasks.FirstOrDefault(t => t.ID == taskID);
            
            if (task != null)
            {
                Debug.Log($"Attempting to complete task: {taskID}");
                completedTaskIDs.Add(taskID);
                Debug.Log($"Task Completed: {task.title}");

                NotificationManager.Instance.ShowNotification($"Mission Updated: {task.title}");

                // CheckForNextTask();
            }
        }

        public bool IsTaskComplete(string taskID)
        {
            return completedTaskIDs.Contains(taskID);
        }
    }
}