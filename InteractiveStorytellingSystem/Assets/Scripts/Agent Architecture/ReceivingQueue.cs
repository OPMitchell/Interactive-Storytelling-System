using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public class ReceivingQueue : EventPriorityQueue
{
    [SerializeField] private TextAsset ResponseListFile;
    public List<Response> ResponseList {get; private set;}
    private MemoryManager memoryManager;

    private void CreateResponseList()
    {
        this.ResponseList = ConfigReader.ReadResponseList(ResponseListFile.name + ".xml");
    }

    void Start()
    {
        CreateResponseList();
        memoryManager = GetComponent<MemoryManager>();
    }

    public override void CheckQueue()
    {
        //Respond to incoming actions from other characters
        if(!queue.IsEmpty())
        {
            Action receivedAction = queue.Remove();
            //analyse action
            //store in memory
            StoreMemory(receivedAction);
            //respond
            foreach (Response r in ResponseList)
            {
                if(r.Name == receivedAction.Name && (r.Sender == receivedAction.Sender || r.Sender == "*") && r.DialogID == receivedAction.DialogID)
                {
                    Action response = r.Action;
                    if (response.Sender == "*")
                        response.Sender = receivedAction.Sender;
                    if (response.Target == "*")
                        response.Target = receivedAction.Sender;
                    GetComponent<ActionQueue>().QueueAction(response);        
                }
            }
            Testing.WriteToLog(transform.name, "Received action: " + Testing.GetActionInfo(receivedAction));
        }
    }

    private void StoreMemory(Action action)
    {
        string description = CreateMemoryDescription(action);
        MemoryPattern newMemory = new MemoryPattern(0, action.Keywords, action.Type, 0.0f, description);
        memoryManager.AddMemoryPattern(newMemory);
    }

    private string CreateMemoryDescription(Action action)
    {
        return "";
    }

}
